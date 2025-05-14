using Nasty.Common.Registry;
using System.Text;

namespace Nasty.Common.Middleware
{
    // 自定义中间件
    public class RequestMiddleware : IAutofacMiddleware
    {

        public RequestMiddleware()
        {

        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //由于解析Boby损耗性能，由各自过滤器自己解析 降低性能损耗
            context.Request.EnableBuffering();

            //using (var reader = new StreamReader(
            //    context.Request.Body,
            //    Encoding.UTF8,
            //    detectEncodingFromByteOrderMarks: true,
            //    bufferSize: 1024,
            //    leaveOpen: true))
            //{
            //    var body = await reader.ReadToEndAsync();
            //    context.Items["RequestBody"] = body;
            //    context.Request.Body.Position = 0;
            //}

            return next(context);
        }
    }

}
