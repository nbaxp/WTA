using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Volo.Abp.Guids;

namespace WTA.Infrastructure.Services;

public class DefaultGuidGenerator : Application.Abstractions.IGuidGenerator
{
    private SequentialGuidGenerator _sequentialGuidGenerator { get; }

    public DefaultGuidGenerator(IConfiguration configuration)
    {
        var options = new AbpSequentialGuidGeneratorOptions();
        var databaseKey = configuration.GetConnectionString("default")?.ToLower();
        if (databaseKey is not null)
        {
            if (databaseKey.Contains("oracle"))
            {
                options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsBinary;
            }
            else if (databaseKey.Contains("sqlserver"))
            {
                options.DefaultSequentialGuidType = SequentialGuidType.SequentialAtEnd;
            }
            else if (databaseKey.Contains("mysql") || databaseKey.Contains("PostgreSql"))
            {
                options.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString;
            }
        }
        this._sequentialGuidGenerator = new SequentialGuidGenerator(new OptionsWrapper<AbpSequentialGuidGeneratorOptions>(options));
    }

    public Guid Create()
    {
        return _sequentialGuidGenerator.Create();
    }
}
