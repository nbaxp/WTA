namespace WTA.Application.Abstractions.Domain;

public interface IConcurrencyStamp
{
  public string? ConcurrencyStamp { get; set; }
}
