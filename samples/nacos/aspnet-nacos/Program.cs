var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration.GetValue("UseNacos", false))
{
    builder.Host.UseNacosConfig("NacosConfig");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();