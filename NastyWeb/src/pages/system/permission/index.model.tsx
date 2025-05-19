import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import Upload from '@/@nasty/components/Upload';
import { PlusOutlined } from '@ant-design/icons';
import { ActionType, ModalForm, ProFormInstance, ProFormItem, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Button, message } from 'antd';
import { FC, cloneElement, useCallback, useRef, useState } from 'react';

async function SavePermissionGroup(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Permission/SavePermissionGroup", { ...options })
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
    const { reload } = props;

    const [messageApi, contextHolder] = message.useMessage();
    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();

    const { run, loading } = useRequest(SavePermissionGroup, {
        manual: true,
        onSuccess: () => {
            messageApi.success('提交成功');
            reload?.();
        },
        onError: (e: any) => {
            messageApi.error(e);
        },
    });

    return (
        <>
            {contextHolder}
            <ModalForm
                title={"新增分组"}
                trigger={
                    <Button type="primary" icon={<PlusOutlined />}>
                        {"新增"}
                    </Button>
                }
                width="552px"
                modalProps={{ okButtonProps: { loading } }}
                onFinish={async (value) => {
                    await run({ ...value });
                    return true;
                }}
            >
                <ProFormText label="分组名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "分组名称必填",
                        },
                    ]}

                />

                <ProFormText label="分组编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "分组编码必填",
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
    const { run, loading } = useRequest(SavePermissionGroup, {
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
        HttpClient.post("/Portal/Permission/GetPermissionGroup", { id: values.Id }).then((res) => {
            if (res.Id) {
                formRef.current?.setFieldsValue(res);
            } else {
                message.error("分组数据加载失败");
            }

        }).catch((error) => {
            message.error(error);
        })

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"修改分组"}
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

                <ProFormText label="分组名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "分组名称必填",
                        },
                    ]}

                />

                <ProFormText label="分组编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "分组编码必填",
                        },
                    ]}
                />
            </ModalForm>
        </>
    );
};





export { AddModel, UpdateModel }