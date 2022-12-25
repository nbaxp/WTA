using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.System;

public class SystemModuleAttribute : ResourceAttribute
{
  public SystemModuleAttribute() : base("SystemManagement")
  {
  }
}
