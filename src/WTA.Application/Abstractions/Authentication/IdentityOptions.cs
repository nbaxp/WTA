namespace WTA.Application.Authentication;

public class IdentityOptions
{
    public const string Position = "Identity";
    public bool SupportsUserLockout { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public TimeSpan DefaultLockoutTimeSpan { get; set; }
}
