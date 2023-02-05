using WTA.Application.Abstractions.Components;

namespace WTA.Application.Authentication;

[Configuration]
public class IdentityOptions
{
    public bool SupportsUserLockout { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public TimeSpan DefaultLockoutTimeSpan { get; set; }
}
