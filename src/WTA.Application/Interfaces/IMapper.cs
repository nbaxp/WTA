namespace WTA.Application.Interfaces;

public interface IMapper
{
  void From<T>(T to, object from);

  T To<T>(object from);
}
