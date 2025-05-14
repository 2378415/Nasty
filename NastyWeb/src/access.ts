/**
 * @see https://umijs.org/zh-CN/plugins/plugin-access
 * */
export default function access(initialState: { currentUser?: API.CurrentUser } | undefined, v: any) {
  const currentUser = initialState?.currentUser ?? {} as API.CurrentUser;
  return {
    verify: (route: any) => {
      let roles: string[] = route.roles || [];
      let permissions: string[] = route.permissions || [];
      if (roles.length == 0 && permissions.length == 0) return true;

      let isUnion = route.isUnion || false;

      let userPermissions: string[] = currentUser.Permissions || [];
      let userRoles: string[] = currentUser.Roles || [];

      let isRoleAny = false;
      let isPermissionAny = false;
      if (isUnion) {
        //并集权限处理，标签Permissions或者Roles必须完全包含路由定义的权限。
        isPermissionAny = (permissions.length == 0) ? true : permissions.every(perm => userPermissions.includes(perm));
        isRoleAny = (roles.length == 0) ? true : roles.every(role => userRoles.includes(role));
      } else {
        //交集权限处理，标签Permissions或者Roles与路由定义的权限有交集即可。
        isPermissionAny = permissions.length == 0 ? true : userPermissions.some(perm => permissions.includes(perm));
        isRoleAny = roles.length == 0 ? true : userRoles.some(role => roles.includes(role));
      }

      return isPermissionAny && isRoleAny;
    },
  };
}
