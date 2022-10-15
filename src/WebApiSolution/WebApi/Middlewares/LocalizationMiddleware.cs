using WebApi.Models;

namespace WebApi.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public LocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var headers = httpContext.Request.Headers;
        var apiLanguage = headers["Api-Language"];
        if (!apiLanguage.Any())
        {
            await _next.Invoke(httpContext);
            return;
        }

        var lang = apiLanguage.FirstOrDefault();
        var dicLang = Languages.GetLanguage(lang);
        if (dicLang == null || !dicLang.Any())
        {
            await _next.Invoke(httpContext);
            return;
        }

        var originBody = httpContext.Response.Body;
        try
        {
            var memStream = new MemoryStream();
            httpContext.Response.Body = memStream;

            await _next(httpContext).ConfigureAwait(false);

            memStream.Position = 0;
            var responseBody = new StreamReader(memStream).ReadToEnd();

            foreach (var item in dicLang)
            {
                responseBody = responseBody.Replace("\"" + item.Key + "\":", "\"" + item.Value + "\":");
            }

            var memoryStreamModified = new MemoryStream();
            var sw = new StreamWriter(memoryStreamModified);
            sw.Write(responseBody);
            sw.Flush();
            memoryStreamModified.Position = 0;

            await memoryStreamModified.CopyToAsync(originBody).ConfigureAwait(false);
        }
        finally
        {
            httpContext.Response.Body = originBody;
        }
    }
}