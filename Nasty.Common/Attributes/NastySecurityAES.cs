using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nasty.Common.Config;
using Nasty.Common.LoadParams;
using Nasty.Common.Registry;
using Nasty.Common.Security;
using Nasty.Common.Session;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Nasty.Common.Attributes
{

    public class NastySecurityAES : ActionFilterAttribute
    {


        // 可以通过构造函数注入服务
        public NastySecurityAES()
        {

        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                context.HttpContext.Request.Body.Position = 0;
                using (var reader = new StreamReader(context.HttpContext.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: 1024,
                    leaveOpen: true))
                {
                    //得到原始boby
                    var body = reader.ReadToEndAsync().Result;

                    //得到原始参数
                    var argParams = JsonConvert.DeserializeObject<ArgParams>(body);
                    var arg = AESHelper.Decrypt(argParams?.Arg ?? string.Empty, SuperConfig.Get("SecurityAESKey"));

                    //得到控制器方法定义的参数类型
                    var @params = context.ActionArguments.FirstOrDefault();
                    var key = @params.Key;
                    var value = @params.Value;
                    var type = value.GetType();

                    // 将body反序列化为参数类型
                    var deserializedValue = JsonConvert.DeserializeObject(arg, type);

                    // 更新Action参数
                    context.ActionArguments[key] = deserializedValue;
                }

                base.OnActionExecuting(context);
            }
            catch (Exception ex)
            {

                context.Result = new ObjectResult(new ResultData<string>()
                {
                    IsSuccess = false,
                    Data = string.Empty,
                    Message = ex.Message,
                });
                return;
            }

    
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            dynamic result = (context.Result as ObjectResult)?.Value;

            var data = result?.Data;
            if (result != null && result.IsSuccess && data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                data = AESHelper.Encrypt(json, SuperConfig.Get("SecurityAESKey"));
            }

            context.Result = new ObjectResult(new ResultData<string>()
            {
                IsSuccess = result?.IsSuccess ?? false,
                Data = data,
                Message = result?.Message ?? string.Empty,
            });
            base.OnActionExecuted(context);
        }

    }
}
