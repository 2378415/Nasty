import { baseUrl } from "./Axios";

function getWebFiles(data: string[] | string | undefined) {
    if (data == null) return [];

    let isString = typeof data === 'string' ? true : false;

    let items = isString ? [data] : [...data];

    items = items || [];
    let list: any[] = [];


    items.forEach((t) => {
        let keyName = getFileKeyName(t);
        let url = `${baseUrl}/Portal/File/Preview?key=${t}`
        let data = {
            uid: keyName.key,
            name: keyName.name,
            fileName: keyName.name,
            url: url,
            response: t,
            status: "done"
        };

        list.push(data);
    });

    return list;
}

function getServiceFiles(data: any | undefined, isArray: boolean = true) {
    let items = data?.fileList || [];
    let list: string[] = [];

    items.forEach((t: any) => {
        if (t.status == "done") {
            let url = t.response;
            list.push(url);
        }
    });

    if (isArray) {
        return list;
    } else {
        return list.length == 0 ? null : list[0];
    }

}

function getFileKeyName(str: string) {
    const firstUnderscoreIndex = str.indexOf('_');
    const result = {
        key: str.substring(0, firstUnderscoreIndex),
        name: str.substring(firstUnderscoreIndex + 1)
    }
    return result;
}

function toLowerCaseKeys(obj: any) {
    const newObj = {};
    Object.keys(obj).forEach(key => {
        newObj[key.toLowerCase()] = obj[key];
    });
    return newObj;
}

function getFileUrl(key: any) {
    if (key == null) return "";
    return `${baseUrl}/Portal/File/Preview?key=${key}`
}

const equal = (arr1: string[], arr2: string[]) => {
    if (arr1.length !== arr2.length) return false;

    const set1 = new Set(arr1);
    const set2 = new Set(arr2);

    // 检查每个元素是否都在另一个集合中
    return [...set1].every(item => set2.has(item));
}

const getFileListUIDs = (fileList:any[]) => {
    let uids: string[] = [];
    fileList.forEach((t) => {
        if (t.response) {
            let img = t.response as string;
            let uid = img.replace(`_${t.name}`, '');
            uids.push(uid);
        } else {
            uids.push(t.uid);
        }
    });

    return uids;
}

const isDev = process.env.NODE_ENV === 'development';

export const util = {
    getWebFiles,
    getServiceFiles,
    toLowerCaseKeys,
    getFileUrl,
    getFileListUIDs,
    equal,
    isDev
}