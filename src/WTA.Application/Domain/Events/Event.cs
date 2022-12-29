using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Domain;
using WTA.Application.Domain.System;

namespace WTA.Application.Domain;

[SystemModule]
[Permission]
[Display(Name = "事件")]
public class Event : BaseEntity
{
    public DateTimeOffset Date { get; set; }
    public string Entity { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string? Original { get; set; }
    public string? Current { get; set; } = null!;
}
