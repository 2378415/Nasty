import React from 'react';
import { UploadOutlined } from '@ant-design/icons';
import { Button, Upload as _Upload } from 'antd';
import type { UploadFile } from 'antd';
import { Principal } from '../Principal';
import { UploadListType } from 'antd/es/upload/interface';
import { util } from '../Util';
import { UploadProps as _UploadProps } from 'antd/lib/upload';
import { baseUrl } from '../Axios';

interface UploadProps extends Omit<_UploadProps, 'defaultFileList'> {
  defaultFileList?: string[];
  value?: string[] | string
}

const Upload: React.FC<UploadProps> = (props) => {
  let defaultFileList = util.getWebFiles(props.value || props.defaultFileList);
  const count = props.maxCount || 1;
  const listType = props.listType || "picture";

  return <_Upload
    action={`${baseUrl}/Portal/File/SaveFile`}
    {...props}
    defaultFileList={defaultFileList}
    listType={listType}
    headers={{ authorization: `Bearer ${Principal.getToken()}` }}
    withCredentials={!util.isDev}
    onChange={(v) => {
      let data: any = util.getServiceFiles(v);
      data = count == 1 ? (data.length > 0 ? data[0] : null) : data;
      if (props.onChange) props.onChange(data);
    }}
  >
    <Button type="primary" icon={<UploadOutlined />}>
      {"上传"}
    </Button>
  </_Upload >
}

export default Upload;