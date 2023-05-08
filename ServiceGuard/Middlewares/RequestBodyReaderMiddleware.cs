using System.Text;

namespace ServiceGuard.Middlewares {

    public class RequestBodyReaderMiddleware {

        private readonly RequestDelegate _next;

        public RequestBodyReaderMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            var request = context.Request;
            if (request.Method != HttpMethods.Post) {
                await _next(context);
                return;
            }

            // 在请求正文中读取指定长度的字节
            using var memoryStream = new MemoryStream();
            await request.BodyReader.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            // 重新设置请求正文
            request.Body = memoryStream;
            //Console.WriteLine($"TTTT: {memoryStream.Length}");

            // 执行下一个中间件
            await _next(context);
        }

    }

}
