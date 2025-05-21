import { removeRule, rule } from '@/services/ant-design-pro/api';
import type { ActionType, ProColumns, ProDescriptionsItemProps } from '@ant-design/pro-components';
import {
  FooterToolbar,
  PageContainer,
  ProDescriptions,
  ProTable,
} from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Button, Drawer, Input, Modal, message } from 'antd';
import React, { useCallback, useRef, useState } from 'react';
import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import { AddModel, SetRolePermissionModel, UpdateModel } from './index.model';
import { getRoleTypeText, isNormalRole } from './common';

async function getPageApi(
  params: any,
  options?: any,
) {
  return new Promise<any>((resolve, reject) => {
    HttpClient.post("/Portal/Role/GetRolePage", { ...params })
      .then((data) => {
        data = util.toLowerCaseKeys(data);
        resolve(data);
      }).catch((e) => {
        reject(e)
      })
  });

}


const TableList: React.FC = () => {
  const actionRef = useRef<ActionType>();
  /**
   * @en-US International configuration
   * @zh-CN 国际化配置
   * */
  const intl = useIntl();

  const reload = () => {
    actionRef.current?.reloadAndRest?.();
  }

  const handleRemove = (data: any) => {
    Modal.confirm({
      title: '删除',
      content: '确定删除吗？',
      okText: '确认',
      cancelText: '取消',
      onOk: () => {
        HttpClient.post("/Portal/Role/DeleteRoles", { items: [data.Id] }).then((res) => {
          if (res.IsSuccess) {
            message.success('提交成功');
          } else {
            message.error(res.Message);
          }

          reload();
        }).catch((error) => {
          message.error('提交失败');
        })

      },
      onCancel: () => { },
    });
  }

  const columns: ProColumns<any>[] = [
    {
      title: "角色名称",
      dataIndex: 'Name',
    },
    {
      title: "角色编码",
      dataIndex: 'Code'
    },
    {
      title: "角色类型",
      dataIndex: 'Type',
      hideInSearch: true,
      render: (_, record) => getRoleTypeText(record.Type)
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
      render: (_, record) => {
        let actions = [
          <SetRolePermissionModel
            trigger={<a>{"权限"}</a>}
            key="permission"
            values={record}
            reload={reload}
          />
        ]

        if (isNormalRole(record.Type)) {
          actions.push(<UpdateModel
            trigger={<a>{"编辑"}</a>}
            key="update"
            values={record}
            reload={reload}
          />)
          actions.push(<a key="delete" onClick={() => { handleRemove(record); }}>
            {"删除"}
          </a>)
        }
        return actions;
      },
    },
  ];



  return (
    <PageContainer>
      <ProTable<any, any>
        headerTitle={"角色"}
        actionRef={actionRef}
        rowKey="Id"
        search={{
          labelWidth: 120,
        }}
        toolBarRender={() => [<AddModel key="create" reload={reload} />]}
        request={getPageApi}
        columns={columns}
      />

    </PageContainer>
  );
};

export default TableList;
