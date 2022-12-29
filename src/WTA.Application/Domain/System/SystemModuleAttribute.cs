using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.System;

public class SystemModuleAttribute : GroupAttribute
{
    public SystemModuleAttribute() : base("SystemManagement", "SystemManagement")
    {
    }
}
