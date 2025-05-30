﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nasty.Common.Model;
using Nasty.Common.Registry;
using Nasty.Common.Session;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nasty.Common.ActionFilter
{
    /// <summary>
    /// 截获数据库连接与当前用户，存储在 AsyncLocal<T> 中，注意数据库必须是Scoped生命周期，否则会出现事务问题
    /// </summary>
    public class AppSessionActionFilter : IAutofacActionFilter
    {
        public AppSessionActionFilter(SqlSugarClient client)
        {
            // 将当前数据库连接存储在 AsyncLocal<T> 中
            AppSession.CurrentDb.Value = client;
        }


        public void OnActionExecuting(ActionExecutingContext context)
        {
            // 在控制器方法执行之前执行一些操作...
            // 例如，你可以在这里检查模型的有效性，或者设置一些共享数据。

            //获取当前HttpContext
            AppSession.CurrentHttp.Value = context.HttpContext;


            // 获取当前用户
            var userName = context.HttpContext.User.Identity?.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                // 将当前用户存储在 AsyncLocal<T> 中
                // 同一个请求中，任何地方都可以通过 AppSession.CurrentUser.Value 获取当前用户
                var info = SessionManager.GetUserInfo(userName) ?? new UserInfoModel();
                if (string.IsNullOrEmpty(info.Id))
                {
                    // 如果用户信息为空，终止请求并返回 401 Unauthorized
                    context.Result = new UnauthorizedResult();
                    return;
                }
                else
                {
                    AppSession.CurrentUser.Value = info;
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // 在控制器方法执行之后执行一些操作...
        }

    }
}
