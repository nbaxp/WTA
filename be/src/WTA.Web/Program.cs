using WTA.Web;

new Startup().Start(args);

//WebApplication.CreateBuilder(args).BuildAndRun();

//var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
//if (aspNetCoreEnvironment == "Development")
//{
//    builder.ConfigureAppConfiguration(delegate (HostBuilderContext _, IConfigurationBuilder cb)
//    {
//        try
//        {
//            var config = cb.Build();
//            var address = config.GetValue<string>("NacosConfig:ServerAddresses:0")?.Trim().TrimEnd('/');
//            AddJsonByUrl(cb, $"{address}/nacos/appsettings.json");
//            AddJsonByUrl(cb, $"{address}/nacos/appsettings.{aspNetCoreEnvironment}.json");
//        }
//        catch (Exception ex)
//        {
//            throw new Exception($"开发模式下加载配置文件失败:{ex.Message}", ex);
//        }
//    });
//}

//private static void AddJsonByUrl(IConfigurationBuilder configurationBuilder, string url)
//{
//    var stream = new HttpClient().GetStreamAsync(url).Result;
//    configurationBuilder.AddJsonStream(stream);
//}
