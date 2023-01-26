using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;

namespace WTA.Application.Domain.System;

[SystemModule]
[Permission]
[Display(Name = "事件")]
public class EntityEvent : BaseEntity
{
    public DateTimeOffset Date { get; set; }
    public string Entity { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string? Original { get; set; }
    public string? Current { get; set; } = null!;
}
