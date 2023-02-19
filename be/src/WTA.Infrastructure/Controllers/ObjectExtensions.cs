namespace WTA.Infrastructure.Controllers;

public static class ObjectExtensions
{
    public static int GetPageSize(this object? obj, int @default = 10)
    {
        return (int)(obj?.GetType().GetProperties().Where(o => o.Name.ToLower() == "pagesize").FirstOrDefault()?.GetValue(obj) ?? @default);
    }

    public static int GetPageIndex(this object? obj, int @default = 1)
    {
        return (int)(obj?.GetType().GetProperties().Where(o => o.Name.ToLower() == "pageindex").FirstOrDefault()?.GetValue(obj) ?? @default);
    }

    public static string GetOrderBy(this object? obj, string @default = "Id")
    {
        var names = (obj?.GetType().GetProperties().Where(o => o.Name.ToLower() == "orderby").FirstOrDefault()?.GetValue(obj) ?? @default).ToString();
        return $"new ({names})";
    }
}
