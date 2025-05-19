import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import Upload from '@/@nasty/components/Upload';
import { PlusOutlined } from '@ant-design/icons';
import { ActionType, ModalForm, ProFormInstance, ProFormItem, ProFormSelect, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Button, Checkbox, Divider, message } from 'antd';
import { FC, cloneElement, useCallback, useRef, useState } from 'react';

async function SaveRole(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Role/SaveRole", { ...options })
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
                let items = data.map((t: any) => { return { label: t.Name, value: t.Id } });
                resolve(items);
            });
    });
}


async function SaveRolePermission(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Role/SaveRolePermission", { ...options })
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

    const { run, loading } = useRequest(SaveRole, {
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
                title={"新增角色"}
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
                <ProFormText label="角色名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "角色名称必填",
                        },
                    ]}

                />

                <ProFormText label="角色编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "角色编码必填",
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
    const { run, loading } = useRequest(SaveRole, {
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
        HttpClient.post("/Portal/Role/GetRole", { id: values.Id }).then((res) => {
            if (res.Id) {
                formRef.current?.setFieldsValue(res);
            } else {
                message.error("角色数据加载失败");
            }

        }).catch((error) => {
            message.error(error);
        })

    }


    return (
        <>
            {contextHolder}
            <ModalForm
                title={"修改角色"}
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

                <ProFormText label="角色名称" name="Name" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "角色名称必填",
                        },
                    ]}

                />

                <ProFormText label="角色编码" name="Code" width="xl"
                    rules={[
                        {
                            required: true,
                            message: "角色编码必填",
                        },
                    ]}
                />
            </ModalForm>
        </>
    );
};



const SetRolePermissionModel: FC<any> = (props) => {
    const { reload } = props;
    const { values, trigger } = props;
    const formRef = useRef<ProFormInstance>();
    const [messageApi, contextHolder] = message.useMessage();

    const [permissions, SetPermissions] = useState<string[]>([]);
    const [groupPermissions, SetGroupPermissions] = useState<any[]>([]);
    const groupPermissionIds = groupPermissions.map((t) => t.value);

    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();
    const { run, loading } = useRequest(SaveRolePermission, {
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
        HttpClient.post("/Portal/Role/GetRole", { id: values.Id }).then((res) => {
            if (res.Id) {
                res.RoleId = res.Id;
                res.RoleName = res.Name;

                let items = res.Permissions || [];
                items = items.map((t: any) => t.Id);
                SetPermissions(items);

                formRef.current?.setFieldsValue(res);
            } else {
                message.error("角色数据加载失败");
            }

        }).catch((error) => {
            message.error(error);
        });

    }

    const loadGroupPermissions = (groupId: any) => {
        HttpClient.post("/Portal/Permission/GetPermissions", { GroupId: groupId }).then((res) => {

            let items = res || [];
            items = items.map((t: any) => { return { label: t.Name, value: t.Id } });
            SetGroupPermissions(items);
        }).catch((error) => {
            message.error(error);
        });
    }

    const permissionsChange = (v: string[]) => {
        let diff = util.difference(groupPermissionIds, v);
        let items = [...new Set([...permissions, ...v])];
        items = util.remove(items, diff);
        SetPermissions(items);
    }

    return (
        <>
            {contextHolder}
            <ModalForm
                title={"设置角色权限"}
                formRef={formRef}
                trigger={trigger}
                width="552px"
                modalProps={{ okButtonProps: { loading } }}
                onFinish={async (value) => {
                    if (permissions.length == 0) {
                        message.error("最少选择一个权限");
                        return false;

                    }
                    await run({ ...value, PermissionIds: permissions });
                    return true;
                }}
                onOpenChange={(v) => {
                    if (v) load();
                }}


            >
                <ProFormText hidden={true} name="RoleId" />

                <ProFormText label="角色" name="RoleName" width="xl" disabled
                    rules={[
                        {
                            required: true,
                            message: "角色名称必填",
                        },
                    ]}

                />

                <ProFormSelect
                    request={getGroupsApi}
                    label="权限分组"
                    name="GroupId"
                    onChange={(v) => { loadGroupPermissions(v); }}
                    rules={[{ required: true }]}
                />

                {groupPermissionIds.length == 0 ? null : <Checkbox checked={util.contains(groupPermissionIds, permissions)} onChange={() => {
                    permissionsChange(groupPermissionIds);
                }}>
                    全选
                </Checkbox>}
                {groupPermissionIds.length == 0 ? null : <Divider style={{ marginTop: 17, marginBottom: 17 }} />}
                <Checkbox.Group
                    value={permissions}
                    //defaultValue={permissions}
                    options={groupPermissions}
                    //defaultValue={permissions}
                    onChange={(v) => { permissionsChange(v); }}
                />

            </ModalForm>
        </>
    );
};


export { AddModel, UpdateModel, SetRolePermissionModel }