import { HttpClient } from "@/@nasty/Axios";
import { util } from "@/@nasty/Util";
import { GridContent } from "@ant-design/pro-components";
import { Card, Col, Modal, Row, Tree, message } from "antd";
import { useEffect, useState } from "react";
import { AddModel, UpdateModel } from "./index.model";
import Search from "antd/es/input/Search";

const updateTreeData = (list: any[], key: any, children: any[]): any[] =>
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
                children: updateTreeData(node.children, key, children),
            };
        }
        return node;
    });


const Index: React.FC = () => {
    const [depts, setDepts] = useState<any[]>([]);
    const [tiledDepts, setTiledDepts] = useState<any[]>([]);
    const [currentDept, setCurrentDept] = useState<any>({});


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
                        updateTreeData(origin, key, items),
                    );

                    resolve();
                }, () => {
                    resolve();
                }
            );

        });

    const handleRemove = () => {
        if (!currentDept.key) {
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

    return (
        <GridContent>
            <Row gutter={24}>
                <Col lg={7} md={24}>
                    <Card bordered={false} title="部门列表" extra={[
                        <AddModel values={{ parentId: currentDept?.key }} trigger={<a>{"新增"}</a>} key="add" />,
                        <UpdateModel values={{ id: currentDept?.key }} trigger={<a style={{ marginLeft: 5 }}>{"编辑"}</a>} key="update" />,
                        <a style={{ marginLeft: 5 }} key="delete" onClick={() => { handleRemove(); }}>{"删除"}</a>
                    ]}>
                        {/* <Search style={{ marginBottom: 25 }} placeholder="搜索" onChange={onSearchChange} /> */}
                        <Tree treeData={depts} loadData={onLoadDeptData} onSelect={(t) => { selectDept(t[0]); }} />
                    </Card>
                </Col>
                <Col lg={17} md={24} >
                    <Card bordered={false} title="部门成员" extra={<a href="#">刷新</a>}>

                    </Card>
                </Col>
            </Row>
        </GridContent>
    );
};
export default Index;
