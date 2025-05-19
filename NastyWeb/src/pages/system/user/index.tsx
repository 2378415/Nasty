import { removeRule, rule } from '@/services/ant-design-pro/api';
import type { ActionType, ProColumns, ProDescriptionsItemProps } from '@ant-design/pro-components';
import {
  FooterToolbar,
  PageContainer,
  ProDescriptions,
  ProTable,
} from '@ant-design/pro-components';
import { FormattedMessage, useIntl, useRequest } from '@umijs/max';
import { Avatar, Button, Drawer, Input, Modal, message } from 'antd';
import React, { useCallback, useRef, useState } from 'react';
import { HttpClient } from '@/@nasty/Axios';
import { util } from '@/@nasty/Util';
import { AddModel, UpdateModel, SetUserRoleModel } from './index.model';

async function getPageApi(
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
        HttpClient.post("/Portal/User/DeleteUser", { id: data.Id }).then((res) => {
          if (res.IsSuccess) {
            message.success('提交成功');
          } else {
            message.success(res.Message);
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
      title: "用户名称",
      dataIndex: 'Name',
    },
    {
      title: "用户账号",
      dataIndex: 'Account'
    },
    {
      title: "用户头像",
      dataIndex: 'Avatar',
      render: (_, record) => {
        let url = util.getFileUrl(record.Avatar);
        return <Avatar src={<img src={url} alt="avatar" />} />
      }
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
        <UpdateModel
          trigger={<a>{"编辑"}</a>}
          key="update"
          values={record}
          reload={reload}
        />,
        <SetUserRoleModel
          trigger={<a>{"角色"}</a>}
          key="role"
          values={record}
          reload={reload}
        />,
        <a key="delete" onClick={() => { handleRemove(record); }}>
          {"删除"}
        </a>,
      ],
    },
  ];



  return (
    <PageContainer>
      <ProTable<any, any>
        headerTitle={"用户列表"}
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
