import axios, { AxiosRequestConfig, AxiosRequestHeaders, AxiosResponseHeaders, AxiosResponse } from 'axios';
import { Principal } from './Principal';
import { toLogin } from './KeyRoute';
import { message } from 'antd';
import { util } from './Util';
export const A = {};

const devUrl = 'http://localhost:5163';
const proUrl = 'http://localhost:5163';
export const baseUrl = util.isDev ? devUrl : proUrl;

axios.defaults.baseURL = baseUrl;
axios.defaults.withCredentials = !util.isDev;

export type AxiosConfigType = {
    isSilence: boolean;
};

function axiosTransformRequest(data: any, headers: AxiosRequestHeaders | undefined) {
    return data;
}

function axiosTransformResponse(data: any, headers: AxiosResponseHeaders | undefined) {
    try {
        return JSON.parse(data);
    } catch {
        return data;
    }
}

function customTransformResponse(response: AxiosResponse<any, any>) {
    let status = response?.status;
    if (status === 401) {
        Principal.logout();
        toLogin();
    }
}

function createAxios() {
    function getOpt() {
        let token = Principal.getToken();
        const opt: AxiosRequestConfig = {
            transformRequest: [axiosTransformRequest],
            transformResponse: [axiosTransformResponse],
            headers: {
                'Content-Type': 'application/json',
                "Authorization": `Bearer ${token}`
            },
            timeout: 30000,

        };

        return opt;
    }



    function post<T = any>(url: string, data: any, config: AxiosConfigType = { isSilence: false }): Promise<T> {
        let opt = getOpt();

        opt = Object.assign(opt, {
            method: 'post',
            url: url,
            data: JSON.stringify(data),
        });

        let isSilence = config.isSilence;

        //loading
        if (isSilence) message.loading({ content: '加载中...', key: 'loading-post', duration: 0 });
        return new Promise((resolve, reject) => {
            axios(opt).then((res) => {
                if (isSilence) message.destroy('loading-post');
                customTransformResponse(res);
                resolve(res.data);
            }).catch((err) => {
                if (isSilence) message.destroy('loading-post');
                customTransformResponse(err.response);
                reject(err);
            });
        });
    }

    return { post };
}

export const HttpClient = createAxios();
