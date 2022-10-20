using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace WebApi.Middlewares;
public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IStringLocalizer<LocalizationMiddleware> _localizer;
    private readonly IList<CultureInfo>? _localizationLanguages;
    public LocalizationMiddleware(RequestDelegate next, IStringLocalizer<LocalizationMiddleware> localizer, IOptions<RequestLocalizationOptions> localizationOptions)
    {
        _next = next;
        _localizer = localizer;
        _localizationLanguages = localizationOptions.Value.SupportedCultures;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var headers = httpContext.Request.Headers;
        var apiLanguage = headers["Accept-Language"];
        if (!apiLanguage.Any())
        {
            await _next.Invoke(httpContext);
            return;
        }

        var lang = apiLanguage.FirstOrDefault();
        
        var isLanguageExists = IsLanguageExists(lang);
        
        if (!isLanguageExists)
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

            var allStrings = _localizer.GetAllStrings();

            foreach (var item in allStrings)
                responseBody = responseBody.Replace("\"" + item.Name + "\":", "\"" + item.Value + "\":");

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

    private bool IsLanguageExists(string? languageKey)
    {
        if (string.IsNullOrEmpty(languageKey))
            return false;

        if (_localizationLanguages == null)
            return false;

        return _localizationLanguages.Any(x => x.Name.Equals(languageKey));
    }

}

public static class LocalizationOptions
{
    public static void AddLocalizationOptions(this IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        }).Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new("en-US");

            CultureInfo[] cultures = new CultureInfo[]
            {
                new("tr-TR"),
                new("en-US"),
                new("de-DE")
            };

            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
        });
    }
}