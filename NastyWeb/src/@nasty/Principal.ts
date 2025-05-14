export type UserInfo = {
    Name: string,
    Account: string,
    Token: string,
}


function createPrincipal() {

    const _flag = "user-info";

    // 登录
    function login(info: UserInfo) {
        localStorage.setItem(_flag, JSON.stringify(info));
    }

    // 退出登录
    function logout() {
        localStorage.removeItem(_flag);
    }

    // 获取用户信息
    function getUserInfo(): UserInfo | null {
        const info = localStorage.getItem(_flag);
        if (!info) return null;
        return JSON.parse(info);
    }

    // 获取Token
    function getToken() {
        const info = getUserInfo();
        if (!info) return null;
        return info.Token;
    }

    function isLogin() {
        let info = getUserInfo();
        return info?.Token ? true : false;
    }

    return {
        login,
        logout,
        getUserInfo,
        getToken,
        isLogin
    };
}

export const Principal = createPrincipal();

