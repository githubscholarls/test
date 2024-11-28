using Amazon.Runtime.Internal;
using System.Security.Permissions;

namespace WebApiTest.Middleware
{
    public class GetBodyContentMiddleware
    {
        private readonly RequestDelegate next;

        public GetBodyContentMiddleware(RequestDelegate requestDelegate)
        {
            next = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            //启用读取request
            context.Request.EnableBuffering();

            //变量设置
            var request = context.Request;
            var response = context.Response;

            //请求body  using识别执行next完后才释放
            using var requestReader = new StreamReader(request.Body);
            var requestBody = await requestReader.ReadToEndAsync();
            request.Body.Position = 0;

            await Console.Out.WriteLineAsync("中间件请求体：" + requestBody.Substring(0, requestBody.Count() > 100 ? 100 : requestBody.Count()));
            // 执行其他中间件
            await next(context);

            //await Console.Out.WriteLineAsync("body content middle start");
            //context.Request.EnableBuffering();
            //using (var rea = new StreamReader(context.Request.Body))
            //{
            //    var body = await rea.ReadToEndAsync();
            //    await Console.Out.WriteLineAsync("中间件请求体：" + body);
            //    //下次读取才有内容
            //    context.Request.Body.Position = 0;
            //    await Console.Out.WriteLineAsync("next before");
            //    //上面设置position=0，这里接口中才能继续读取
            //    await next.Invoke(context);
            //    await Console.Out.WriteLineAsync("next end");
            //}

            //await Console.Out.WriteLineAsync("body content middle end");
        }

    }
}
