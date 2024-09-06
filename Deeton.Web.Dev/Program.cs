using Deeton.Web;
using System.Diagnostics;

using var app = new WebApplication("localhost:8080");
app.Subsystem.LoadBuiltinEndpoint();

// Any testing goes here, please do not commit this file once done with changes. Thanks.
// <------ TESTING CODE GOES HERE! ------->

var api = app.CreatePathHandler("/api");

api.WithSubPath("/v1/fart")
   .WithContentTypeGetter((_) => Text.Html)
   .WithContentGetter((_) => Content.FromText("<html>fart</html>"));


// <------ TESTING CODE ENDS HERE! ------->

Process.Start("cmd.exe", "/c start localhost:8080/MY_AUTOSTART_PATH");
app.Listen();