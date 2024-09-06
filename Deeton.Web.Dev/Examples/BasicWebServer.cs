using Deeton.Web;
using Deeton.Web.Endpoints;
using Newtonsoft.Json;
using System.Diagnostics;

using var app = new WebApplication("localhost:8080");
app.Subsystem.LoadBuiltinEndpoint();

var dashboard = new HttpPathHandler("/dashboard", app);

dashboard.WithSubPath("/home")
    .WithContentTypeGetter((data) => Text.Html)
    .WithContentGetter((data) =>
    {
        var userName = data.Headers.TryGet("name") ?? "user";
        return Content.FromText(@$"
<html lang=""en"">
    <title>Deeton.Web example</title>
    <link href=""/dashboard/assets/dash-styles.css"" rel=""stylesheet"" />
    <script defer src=""/dashboard/assets/bundle.js""></script>

    <h1>Welcome, {userName}</h1>
    <p id=""loading"">This is the user dashboard, please wait while we load.</p>

    <body id=""main"">
        <!-- Javascript bundle will automatically insert the page. -->
    </body>
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

* {
    text-align: center;
}
"));

dashboard.WithSubPath("/assets/bundle.js")
    .WithContentTypeGetter((_) => Text.Javascript)
    .WithContentGetter((_) =>
    {
        // NOTE: In this Examples/ directory there is this file, please copy it to the build location.
        return Content.FromFile("bundle.js");
    });

dashboard.WithSubPath("/motd")
    .WithContentTypeGetter((_) => Application.Json)
    .WithContentGetter((_) =>
    {
        return Content.FromText(JsonConvert.SerializeObject(
            new MessageOfTheDay
            {
                Message = "Welcome to the Deeton.Web example!"
            }
        ));
    });

Process.Start("cmd.exe", "/c start http://localhost:8080/dashboard/home");

// This is currently needed so the application doesn't just close.
Console.ReadKey();

struct MessageOfTheDay
{
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }
}

