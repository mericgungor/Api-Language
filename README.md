# Api Language


Most of developers knows, how to change content language.
But, changing api fields may be difficult.
Of course, you can use dto and automapper etc.

This github repository helps to translate api fields.
For example:
from (english)
```
{
    "name":"Name"
}
```
to (türkçe)

```
{
    "ad":"Name"
}
```
## Features

- You can change api fields language 
- .Net 6
- No Dependency
- Middleware
- header 
- Public, open source, feel free to use



## Installation & Using

Write your api code.
Copy Languages.cs, modify your language,your fields.
Copy LocalizationMiddleware.cs, don't change any thing.

in program.cs
if you want to apply to all api, use this
```
    app.UseMiddleware<LocalizationMiddleware>();
```
if you want to apply to special (named) api, use this
```
app.UseWhen(context => context.Request.Path.StartsWithSegments("/WeatherForecast"), appBuilder =>
{
    appBuilder.UseMiddleware<LocalizationMiddleware>();
});
```
Set api header
```
--header 'Api-Language: de'
```
Unfortunately, you can't use with swagger easyly, use Postman

## Warnings
- less performance
- Don't support earlier .Net (<.Net 5)
- header needed