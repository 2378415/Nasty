import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import Upload from '@/@nasty/components/Upload';
import { PlusOutlined } from '@ant-design/icons';
import { ActionType, ModalForm, ProFormInstance, ProFormItem, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Button, message } from 'antd';
import { FC, cloneElement, useCallback, useRef, useState } from 'react';

async function SavePermissionGroup(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Permission/SavePermission", { ...options })
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

async function getGroupsApi(
    params: any,
    options?: any,
) {
    return new Promise((resolve, reject) => {
        HttpClient.post("/Portal/Permission/GetPermissionGroups", { ...params })
            .then((data) => {
                let items = data.map((t:any) => { return { label: t.Name, value: t.Id } });
                resolve(items);
            });
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
                title={"新增权限"}
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

                <ProFormSelect
                    request={getGroupsApi}
                    label="所属分组"
                    name="GroupId"
                    rules={[{ required: true }]}
                />

                <ProFormText label="权限名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "权限名称必填",
                        },
                    ]}

                />

                <ProFormText label="权限编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "权限编码必填",
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
        HttpClient.post("/Portal/Permission/GetPermission", { id: values.Id }).then((res) => {
            if (res.Id) {
                formRef.current?.setFieldsValue(res);
            } else {
                message.error("权限数据加载失败");
            }

        }).catch((error) => {
            message.error(error);
        })

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"修改权限"}
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

                <ProFormSelect
                    request={getGroupsApi}
                    label="所属分组"
                    name="GroupId"
                    rules={[{ required: true }]}
                />
                
                <ProFormText label="权限名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "权限名称必填",
                        },
                    ]}

                />

                <ProFormText label="权限编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "权限编码必填",
                        },
                    ]}
                />
            </ModalForm>
        </>
    );
};





export { AddModel, UpdateModel }