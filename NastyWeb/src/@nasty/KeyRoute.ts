import { DefaultFooter } from '@ant-design/pro-components';
import { history, Link } from '@umijs/max';

export enum KeyRoute {
    Login = "/user/login",
    Register= "/user/register",
    RegisterResult = '/user/register-result',
    Default = "/mobile/index",
}

// 跳转到登录页
export function toLogin() {
    if (window.location.pathname !== KeyRoute.Login) {
        history.replace({
            pathname: KeyRoute.Login
        });
    }
}

// 跳转到默认页面
export function toDefault() {
    if (window.location.pathname !== KeyRoute.Default) {
        history.replace({
            pathname: KeyRoute.Default
        });
    }
}

