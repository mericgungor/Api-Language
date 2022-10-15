namespace WebApi.Models;

public static class Languages
{
    public static Dictionary<string, string>? GetLanguage(string? language)
    {
        Type t = typeof(Languages);
        var field = t.GetFields().Where(x => x.Name.Equals(language)).FirstOrDefault();
        return field == null ? null : field.GetValue(null) as Dictionary<string, string>;
    }

    //Türkçe
    public static Dictionary<string, string> tr = new Dictionary<string, string>()
    {
        {"date","tarih"},
        {"temperatureC","sıcaklıkC"},
        {"temperatureF","sıcaklıkF"},
        {"summary","özet"}
    };

    //Almanca
    public static Dictionary<string, string> de = new Dictionary<string, string>()
    {
        {"date","datum"},
        {"temperatureC","temperaturC"},
        {"temperatureF","temperaturF"},
        {"summary","zusammenfassung"}
    };
}