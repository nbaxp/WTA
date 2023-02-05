namespace WTA.Application.Abstractions;

public interface IMapper
{
    void From<T>(T to, object from);

    T To<T>(object from);
}
