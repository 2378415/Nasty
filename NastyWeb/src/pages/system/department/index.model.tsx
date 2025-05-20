import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import Upload from '@/@nasty/components/Upload';
import { PlusOutlined } from '@ant-design/icons';
import { ActionType, ModalForm, ProFormInstance, ProFormItem, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Button, Checkbox, Divider, message } from 'antd';
import { FC, cloneElement, useCallback, useRef, useState } from 'react';

async function SaveDepartment(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Department/SaveDepartment", { ...options })
            .then((data) => {
                if (data.IsSuccess !== true)
                    reject(data.Message);
                else
                    resolve(data);
            }).catch((e) => {
                reject(e)
            })
    });
}

const AddModel: FC<any> = (props) => {
    const { reload, values, trigger } = props;
    const [messageApi, contextHolder] = message.useMessage();
    const formRef = useRef<ProFormInstance>();
    const [showParent, setShowParent] = useState(false);

    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();

    const { run, loading } = useRequest(SaveDepartment, {
        manual: true,
        onSuccess: () => {
            messageApi.success('提交成功');
            reload?.();
        },
        onError: (e: any) => {
            messageApi.error(e);
        },
    });

    const load = () => {
        if (values.parentId) {
            HttpClient.post("/Portal/Department/GetDepartment", { id: values.parentId }).then((res) => {
                if (res.Id) {
                    setShowParent(true);
                    formRef.current?.setFieldsValue({ ParentId: res.Id, ParentName: res.Name });
                } else {
                    message.error("父级部门数据加载失败");
                }

            }).catch((error) => {
                message.error(error);
            })
        } else {
            setShowParent(false);
        }

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"新增部门"}
                trigger={trigger}
                width="552px"
                formRef={formRef}
                modalProps={{ okButtonProps: { loading } }}
                onFinish={async (value) => {
                    await run({ ...value });
                    return true;
                }}
                onOpenChange={(v) => {
                    if (v) load();
                }}
            >
                <ProFormText hidden={true} name="ParentId" />
                {!showParent ? null : <ProFormText disabled label="父级部门" name="ParentName" width="xl" />}

                <ProFormText label="部门名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "部门名称必填",
                        },
                    ]}

                />

                <ProFormText label="部门编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "部门编码必填",
                        },
                    ]}
                />


            </ModalForm>
        </>
    );
};


const UpdateModel: FC<any> = (props) => {
    const { reload } = props;
    const { values, trigger } = props;
    const formRef = useRef<ProFormInstance>();
    const [messageApi, contextHolder] = message.useMessage();
    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();
    const { run, loading } = useRequest(SaveDepartment, {
        manual: true,
        onSuccess: () => {
            messageApi.success('提交成功');
            reload?.();
        },
        onError: (e: any) => {
            messageApi.error(e);
        },
    });


    const load = () => {
        if (values.id) {
            HttpClient.post("/Portal/Department/GetDepartment", { id: values.id }).then((res) => {
                if (res.Id) {
                    formRef.current?.setFieldsValue(res);
                } else {
                    message.error("部门数据加载失败");
                }

            }).catch((error) => {
                message.error(error);
            })
        } else {
            message.error("请选择部门");
        }

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"修改部门"}
                formRef={formRef}
                trigger={trigger}
                width="552px"
                modalProps={{ okButtonProps: { loading } }}
                onFinish={async (value) => {
                    await run({ ...value });
                    return true;
                }}
                onOpenChange={(v) => {
                    if (v) load();
                }}

            >
                <ProFormText hidden={true} name="Id" />

                <ProFormText label="部门名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "部门名称必填",
                        },
                    ]}
                />

                <ProFormText label="部门编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "部门编码必填",
                        },
                    ]}
                />
            </ModalForm>
        </>
    );
};




export { AddModel, UpdateModel }