import { HttpClient } from "@/@nasty/Axios";
import { util } from "@/@nasty/Util";
import { ActionType, GridContent, ProColumns, ProTable } from "@ant-design/pro-components";
import { Avatar, Button, Card, Col, Modal, Row, Tree, message } from "antd";
import { useEffect, useRef, useState } from "react";
import { AddModel, AddUserModel, UpdateModel } from "./index.model";
import Search from "antd/es/input/Search";
import { PlusOutlined } from "@ant-design/icons";

const addChildrenTreeData = (list: any[], key: any, children: any[]): any[] =>
    list.map((node) => {
        if (node.key === key) {
            return {
                ...node,
                children,
            };
        }
        if (node.children) {
            return {
                ...node,
                children: addChildrenTreeData(node.children, key, children),
            };
        }
        return node;
    });

function removeTreeData(list: any[], key: string) {
    for (let i = 0; i < list.length; i++) {
        const member = list[i];

        if (member.key === key) {
            list.splice(i, 1);
            // 删除后不需要处理isLeaf，因为被删除的成员已经不在树中了
            return true; // 返回true表示已删除
        }

        if (member.children && member.children.length > 0) {
            const foundInChildren = removeTreeData(member.children, key);
            if (foundInChildren) {
                // 检查当前成员的children是否为空
                if (member.children.length === 0) {
                    member.isLeaf = true;
                    // 可以选择是否将children设置为null
                    // member.children = null;
                }
                return true;
            }
        }
    }

    return false;
}

function updateTreeData(list: any[], key: string, item: any) {
    return list.map((t) => {
        if (t.key == key) {
            t.title = item.title;
            return t;
        }

        if (t.children) {
            return {
                ...t,
                children: addChildrenTreeData(t.children, key, item),
            };
        }

        return t;
    });
}

function addTreeData(list: any, parentKey: string, item: any) {
    // 如果 parentKey 为空，直接添加到顶层
    if (parentKey == null) {
        list.push(item);
        return true;
    }

    // 否则，递归查找 parentKey 并添加到其 children
    for (const member of list) {
        if (member.key === parentKey) {
            // 如果找到匹配的 parentKey，添加到其 children
            if (!member.children) {
                member.children = [];
                member.isLeaf = false; // 原本是叶子节点，现在不是了
            }
            member.children.push(item);
            return true;
        }

        // 如果当前成员有 children，递归查找
        if (member.children && member.children.length > 0) {
            const added = addTreeData(member.children, parentKey, item);
            if (added) return true; // 如果已添加，提前终止
        }
    }

    return false; // 未找到 parentKey，添加失败
}

async function getPageApi(
    params: any,
    options?: any,
) {
    return new Promise<any>((resolve, reject) => {
        HttpClient.post("/Portal/Department/GetDepartmentUserPage", { ...params })
            .then((data) => {
                data = util.toLowerCaseKeys(data);
                resolve(data);
            }).catch((e) => {
                reject(e)
            })
    });

}



const Index: React.FC = () => {
    const [depts, setDepts] = useState<any[]>([]);
    const [tiledDepts, setTiledDepts] = useState<any[]>([]);
    const [currentDept, setCurrentDept] = useState<any>({});
    const actionRef = useRef<ActionType>();

    useEffect(() => {
        loadDept(null, null, null);
        return () => {
        };
    }, []);

    const loadDept = (parentId: string | null, success: any, error: any) => {
        HttpClient.post("/Portal/Department/GetDepartments", { ParentId: parentId })
            .then((data) => {
                let items = data || [];
                items = items.map((t: any) => { return { title: `${t.Name}（${t.Code}）`, key: t.Id, isLeaf: t.IsLeaf, parentKey: t.ParentId } })

                let tileds = [...tiledDepts, ...items];
                tileds = util.filterByField(tileds, "key");

                setTiledDepts(tileds);
                if (success) {
                    success(items);
                } else {
                    if (error)
                        error()
                    else
                        setDepts(items);
                }

            }).catch((e) => {
                error();
                message.error("加载部门数据失败");
            })
    }

    const selectDept = (id: string) => {
        let currentDept = tiledDepts.find((t) => t.key == id);
        setCurrentDept(currentDept);

        actionRef.current?.reload();
    }

    const onLoadDeptData = ({ key, children }: any) =>
        new Promise<void>((resolve) => {
            if (children) {
                resolve();
                return;
            }

            loadDept(key,
                (items: any) => {
                    setDepts((origin) =>
                        addChildrenTreeData(origin, key, items),
                    );

                    resolve();
                }, () => {
                    resolve();
                }
            );

        });



    const handleDeptRemove = () => {
        if (!currentDept?.key) {
            message.error("请选择部门");
            return;
        }


        Modal.confirm({
            title: '删除',
            content: `确定删除${currentDept.title}部门吗？`,
            okText: '确认',
            cancelText: '取消',
            onOk: () => {
                HttpClient.post("/Portal/Department/DeleteDepartments", { items: [currentDept.key] }).then((res) => {
                    if (res.IsSuccess) {
                        message.success('提交成功');

                        let items = tiledDepts.filter(item => item.key !== currentDept.key);
                        setTiledDepts(items);

                        items = [...depts];
                        removeTreeData(items, currentDept.key);
                        setDepts(items);

                        setCurrentDept({});
                    } else {
                        message.success(res.Message);
                    }


                }).catch((error) => {
                    message.error('提交失败');
                })

            },
            onCancel: () => { },
        });
    }

    const onAddModelChange = (data: any) => {
        let items = [...depts];
        addTreeData(items, data.parentKey, data);
        setDepts(items);

        items = [...tiledDepts, data];
        setTiledDepts(items);
    }

    const onUpdateModelChange = (data: any) => {
        let items = updateTreeData([...depts], data.key, data);
        setDepts(items);
        items = [...tiledDepts];
        items = items.filter(item => item.key !== data.key);
        items.push(data);
        setTiledDepts(items);
    }

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
        },
        {
            title: "操作",
            dataIndex: 'option',
            valueType: 'option',
            render: (_, record) => [
                <a key="delete" onClick={() => { handleRemove(record); }}>
                    {"删除"}
                </a>,
            ],
        },
    ];

    const handleRemove = (data: any) => {
        Modal.confirm({
            title: '删除',
            content: `确定删除 ${data.Name} 成员吗？`,
            okText: '确认',
            cancelText: '取消',
            onOk: () => {
                HttpClient.post("/Portal/Department/DeleteDepartmentUser", { DepartmentId: currentDept.key, UserIds: [data.Id] }).then((res) => {
                    if (res.IsSuccess) {
                        message.success('提交成功');
                        reload();
                    } else {
                        message.success(res.Message);
                    }


                }).catch((error) => {
                    message.error('提交失败');
                })

            },
            onCancel: () => { },
        });
    }


    const reload = () => {
        actionRef.current?.reload?.();
    }

    const listTitle = currentDept?.title ? `部门成员 - ${currentDept?.title}` : `部门成员 - 请选择部门`;
    return (
        <GridContent>
            <Row gutter={24}>
                <Col lg={7} md={24}>
                    <Card bordered={false} title="部门列表" extra={[
                        <AddModel onChange={(data: any) => { onAddModelChange(data); }} values={{ parentId: currentDept?.key }} trigger={<a>{"新增"}</a>} key="add" />,
                        <UpdateModel onChange={(data: any) => { onUpdateModelChange(data); }} values={{ id: currentDept?.key }} trigger={<a style={{ marginLeft: 5 }}>{"编辑"}</a>} key="update" />,
                        <a style={{ marginLeft: 5 }} key="delete" onClick={() => { handleDeptRemove(); }}>{"删除"}</a>
                    ]}>
                        <Tree treeData={depts} loadData={onLoadDeptData} onSelect={(t) => { selectDept(t[0]); }} />
                    </Card>
                </Col>
                <Col lg={17} md={24} >
                    <Card bordered={false} title={listTitle}>
                        <ProTable<any, any>
                            headerTitle={"成员列表"}
                            actionRef={actionRef}
                            rowKey="Id"
                            search={{
                                labelWidth: 120,
                            }}
                            toolBarRender={() => [<AddUserModel departmentId={currentDept?.key} key="add" reload={reload} />]}
                            request={(p, o) => {
                                p.DepartmentId = currentDept?.key;
                                return getPageApi(p, o);
                            }}
                            columns={columns}
                        />
                    </Card>
                </Col>
            </Row>
        </GridContent>
    );
};
export default Index;
