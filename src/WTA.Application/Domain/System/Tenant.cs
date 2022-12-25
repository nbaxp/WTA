using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.System;

[SystemModule]
[Permission]
[Display(Name = "租户")]
public class Tanent : BaseEntity
{
}
