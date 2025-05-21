import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import Upload from '@/@nasty/components/Upload';
import { PlusOutlined } from '@ant-design/icons';
import { ActionType, ModalForm, ProFormInstance, ProFormItem, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Button, message } from 'antd';
import { FC, cloneElement, useCallback, useRef, useState } from 'react';
import { roleType } from '../role/common';

async function SaveUser(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/User/SaveUser", { ...options })
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

    const { run, loading } = useRequest(SaveUser, {
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
                title={"新增用户"}
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
                <ProFormText label="用户名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "用户名称必填",
                        },
                    ]}

                />

                <ProFormItem
                    label="头像"
                    name="Avatar"

                >
                    <Upload
                        defaultFileList={[]}
                        maxCount={1}
                        listType={"picture-circle"}
                    >
                    </Upload>
                </ProFormItem>

                <ProFormText label="用户账号" name="Account" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "用户账号必填",
                        },
                    ]}
                />

                <ProFormText label="用户密码" name="Password" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "用户密码必填",
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
    const { run, loading } = useRequest(SaveUser, {
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
        HttpClient.post("/Portal/User/GetUser", { id: values.Id }).then((res) => {
            if (res.Id) {
                formRef.current?.setFieldsValue(res);
            } else {
                message.error("用户数据加载失败");
            }

        }).catch((error) => {
            message.error(error);
        })

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"修改用户"}
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

                <ProFormText label="用户账号" name="Account" width="xl" disabled={true} />

                <ProFormText label="用户名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "用户名称必填",
                        },
                    ]}

                />

                <ProFormItem
                    label="头像"
                    name="Avatar"

                >
                    <Upload
                        maxCount={1}
                        listType={"picture-circle"}
                    >
                    </Upload>
                </ProFormItem>
            </ModalForm>
        </>
    );
};


async function SaveUserRole(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/User/SaveUserRole", { ...options })
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

async function getRolesApi(
    params: any,
    options?: any,
) {
    return new Promise((resolve, reject) => {
        HttpClient.post("/Portal/Role/GetRoles", { ...params, Types: [roleType.Normal, roleType.System] })
            .then((data) => {
                let items = data.map((t: any) => { return { label: t.Name, value: t.Id } });
                resolve(items);
            });
    });
}

const SetUserRoleModel: FC<any> = (props) => {
    const { reload } = props;
    const { values, trigger } = props;
    const formRef = useRef<ProFormInstance>();
    const [messageApi, contextHolder] = message.useMessage();


    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();
    const { run, loading } = useRequest(SaveUserRole, {
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
        HttpClient.post("/Portal/User/GetUser", { id: values.Id }).then((res) => {
            if (res.Id) {
                res.UserId = res.Id;
                res.UserName = res.Name;

                let roles = res.Roles || [];
                res.RoleIds = roles.map((t: any) => t.Id);
                formRef.current?.setFieldsValue(res);
            } else {
                message.error("角色数据加载失败");
            }

        }).catch((error) => {
            message.error(error);
        });

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"设置用户角色"}
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
                <ProFormText hidden={true} name="UserId" />

                <ProFormText label="用户名称" name="UserName" width="xl" disabled
                    rules={[
                        {
                            required: true,
                            message: "用户名称必填",
                        },
                    ]}

                />

                <ProFormSelect
                    request={getRolesApi}
                    mode="multiple"
                    label="角色"
                    name="RoleIds"
                    rules={[{ required: true }]}
                />


            </ModalForm>
        </>
    );
};



export { AddModel, UpdateModel, SetUserRoleModel }