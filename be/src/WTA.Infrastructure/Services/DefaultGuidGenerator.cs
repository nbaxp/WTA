using Microsoft.Extensions.Configuration;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Components;
using WTA.Infrastructure.SequentialGuid;

namespace WTA.Infrastructure.Services;

[Service<IGuidGenerator>]
public class DefaultGuidGenerator : IGuidGenerator
{
    private readonly SequentialGuidType _sequentialGuidType;

    public DefaultGuidGenerator(IConfiguration cfg)
    {
        this._sequentialGuidType = cfg.GetValue("SequentialGuidType", SequentialGuidType.SequentialAsString);
    }

    public Guid Create()
    {
        return SequentialGuidGenerator.NewSequentialGuid(_sequentialGuidType);
    }
}
