namespace WebApiTest.Application.Common.Delegate
{
    public class CustomDelegatingHandler: DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestId = Guid.NewGuid().ToString();
            request.Headers.Add("x-ms-client-request-id", requestId);
            Console.WriteLine("自定义管道请求前处理，增加请求id为："+requestId);

            var res = await base.SendAsync(request, cancellationToken);

            Console.WriteLine($"自定义管道请求处理响应：{res.StatusCode}");

            return res;
        }
    }
}
