using Deeton.Web;
using Deeton.Web.Endpoints;
using System.Diagnostics;

using var app = new WebApplication("localhost:8080");

var dashboard = new HttpPathHandler("/dashboard", app);

dashboard.WithSubPath("/home")
    .WithContentTypeGetter((data) => Text.Html)
    .WithContentGetter((data) =>
    {
        var userName = data.Headers.TryGet("name") ?? "user";
        return Content.FromText(@$"
<html>
    <link href=""/dashboard/assets/dash-styles.css"" rel=""stylesheet"" />

    <h1>Welcome, {userName}</h1>
    <p>This is the user dashboard, please wait while we load.</p>
</html>
");
    });

dashboard.WithSubPath("/assets/dash-styles.css")
    .WithContentTypeGetter((_) => Text.Css)
    .WithContentGetter((_) => Content.FromText(@"
h1 {
    text-align: center;
    font-family: 'monospace';
}

p {
    text-align: center;
}
"));

Process.Start("cmd.exe", "/c start http://localhost:8080/dashboard/home");

// This is currently needed so the application doesn't just close.
Console.ReadKey();
