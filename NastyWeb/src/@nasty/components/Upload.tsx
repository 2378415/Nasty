import React, { useEffect, useState } from 'react';
import { UploadOutlined } from '@ant-design/icons';
import { Button, Upload as _Upload } from 'antd';
import type { UploadFile } from 'antd';
import { Principal } from '../Principal';
import { UploadListType } from 'antd/es/upload/interface';
import { util } from '../Util';
import { UploadChangeParam, UploadProps as _UploadProps } from 'antd/lib/upload';
import { baseUrl } from '../Axios';

interface UploadProps extends Omit<_UploadProps, 'defaultFileList'> {
  defaultFileList?: string[];
  value?: string[] | string
}

const Upload: React.FC<UploadProps> = (props) => {
  const [fileList, setFileList] = useState<any[]>([]);
  const count = props.maxCount || 1;
  const listType = props.listType || "picture";

  useEffect(() => {
    let defaultFileList = util.getWebFiles(props.value || props.defaultFileList);
    let d_uids = defaultFileList.map((t) => t.uid);
    let uids = util.getFileListUIDs(fileList);
    let isAny = util.equal(d_uids, uids);
    if (!isAny) {
      setFileList(defaultFileList);
    }

    return () => {

    };
  }, [props.value, props.defaultFileList]);


  return <_Upload
    action={`${baseUrl}/Portal/File/SaveFile`}
    {...props}
    fileList={fileList}
    listType={listType}
    headers={{ authorization: `Bearer ${Principal.getToken()}` }}
    withCredentials={!util.isDev}
    onChange={(v) => {
      let status = v.file.status;
      if (status == "uploading" || status == "error") {
        setFileList(v.fileList);
        return;
      }

      if (status == "done" || status == "removed") {
        setFileList(v.fileList);
        let data: any = util.getServiceFiles(v);
        data = count == 1 ? (data.length > 0 ? data[0] : null) : data;
        if (props.onChange) props.onChange(data);
        return;
      }
    }}
  >
    <Button type="primary" icon={<UploadOutlined />}>
      {"上传"}
    </Button>
  </_Upload >
}

export default Upload;