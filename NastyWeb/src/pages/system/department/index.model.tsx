import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import Upload from '@/@nasty/components/Upload';
import { PlusOutlined } from '@ant-design/icons';
import { ActionType, ModalForm, ProColumns, ProFormInstance, ProFormItem, ProFormSelect, ProFormText, ProFormTextArea, ProTable } from '@ant-design/pro-components';
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
    const { onChange, values, trigger } = props;
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
        formatResult: (res) => {
            return res?.Data || {};
        },
        onSuccess: (res) => {
            messageApi.success('提交成功');
            onChange?.({ title: `${res.Name}（${res.Code}）`, key: res.Id, parentKey: res.ParentId, isLeaf: res.IsLeaf });
        },
        onError: (e: any) => {
            messageApi.error(e);
        },
    });

    const load = () => {
        formRef.current?.resetFields();
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
    const { onChange, values, trigger } = props;
    const formRef = useRef<ProFormInstance>();
    const [messageApi, contextHolder] = message.useMessage();
    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();
    const { run, loading } = useRequest(SaveDepartment, {
        manual: true,
        onSuccess: (data, params) => {
            messageApi.success('提交成功');
            let item = params[0];
            if (item) onChange?.({ title: `${item.Name}（${item.Code}）`, key: item.Id });
        },
        onError: (e: any) => {
            messageApi.error(e);
        },
    });


    const load = () => {
        formRef.current?.resetFields();
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



async function getUserPageApi(
    params: any,
    options?: any,
) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/User/GetUserPage", { ...params })
            .then((data) => {
                data = util.toLowerCaseKeys(data);
                resolve(data);
            }).catch((e) => {
                reject(e)
            })
    });

}

async function SaveDepartmentUser(options: any) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Department/SaveDepartmentUser", { ...options })
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

const AddUserModel: FC<any> = (props) => {
    const { reload, departmentId } = props;
    const [messageApi, contextHolder] = message.useMessage();
    const formRef = useRef<ProFormInstance>();
    const actionRef = useRef<ActionType>();
    const [selectedRowKeys, setSelectedRowKeys] = useState<any[]>([]);
    const [selectedRows, setSelectedRows] = useState<any[]>([]);
    /**
     * @en-US International configuration
     * @zh-CN 国际化配置
     * */
    const intl = useIntl();

    const { run, loading } = useRequest(SaveDepartmentUser, {
        manual: true,
        formatResult: (res) => {
            return res?.Data || {};
        },
        onSuccess: (res) => {
            messageApi.success('提交成功');
            reload?.();
        },
        onError: (e: any) => {
            messageApi.error(e);
        },
    });


    const columns: ProColumns<any>[] = [
        {
            title: "用户名称",
            dataIndex: 'Name',
        },
        {
            title: "用户账号",
            dataIndex: 'Account'
        },
        {
            title: "角色",
            dataIndex: 'Group',
            hideInSearch: true,
            render: (_, record) => {
                let roles = record.Roles || [];
                roles = roles.map((t: any) => t.Name);
                if (roles.length == 0) return "-";
                return roles.join(" , ");
            }
        },
        {
            title: "创建时间",
            dataIndex: 'CreateTime',
            hideInSearch: true,
        },
        {
            title: "创建人",
            dataIndex: 'CreateUser',
            hideInSearch: true,
        }
    ];

    return (
        <>
            {contextHolder}
            <ModalForm
                title={"新增部门用户"}
                trigger={<Button type="primary" icon={<PlusOutlined />}>
                    {"新增"}
                </Button>}

                formRef={formRef}
                modalProps={{ okButtonProps: { loading } }}
                onFinish={async (value) => {
                    if (selectedRowKeys.length === 0) {
                        message.warning('请至少选择一项');
                        return false;
                    }

                    if (!departmentId) {
                        message.warning('请先选择部门');
                        return false;
                    }

                    await run({ DepartmentId: departmentId, UserIds: selectedRowKeys });
                    return true;
                }}

                onOpenChange={(v) => {
                    if (v) {
                        setSelectedRowKeys([]);
                        setSelectedRows([]);
                    }
                }}
            >

                <ProFormText hidden={true} name="DepartmentId" />
                <ProTable<any, any>
                    headerTitle={"用户列表"}
                    actionRef={actionRef}
                    rowKey="Id"
                    search={{
                        labelWidth: 'auto',
                        span: 6, // 根据弹窗宽度调整
                        defaultCollapsed: true, // 默认折叠搜索区域
                    }}

                    rowSelection={{
                        selectedRowKeys,
                        onChange: (keys, rows) => {
                            setSelectedRowKeys(keys);
                            setSelectedRows(rows);
                        },
                    }}
                    tableAlertOptionRender={({ selectedRowKeys, selectedRows, onCleanSelected }) => (
                        <a onClick={onCleanSelected}>取消选择</a>
                    )}

                    request={getUserPageApi}
                    columns={columns}
                />
            </ModalForm>
        </>
    );
};



export { AddModel, UpdateModel, AddUserModel }