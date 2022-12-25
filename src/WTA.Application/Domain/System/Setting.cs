using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.System;

[SystemModule]
[Permission]
[Display(Name = "配置")]
public class Setting : BaseEntity
{
  public string Key { get; set; } = null!;
  public string Value { get; set; } = null!;
}
