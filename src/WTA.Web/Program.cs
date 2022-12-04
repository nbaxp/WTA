using WTA.Infrastructure.Data;
using WTA.Infrastructure.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Config();
var app = builder.Build();
app.Configure();
app.Run();
