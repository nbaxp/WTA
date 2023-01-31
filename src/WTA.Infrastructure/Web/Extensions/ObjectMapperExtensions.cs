using System.Collections;
using System.Reflection;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Injections;

namespace WTA.Infrastructure.Web.Extensions;

public static class ObjectMapperExtensions
{
    /// <summary>
    /// 从模型更新实体
    /// </summary>
    public static T FromObject<T>(this T to, object from)
    {
        try
        {
            to.InjectFrom<DeepInjection>(from);
            return to;
        }
        catch (Exception ex)
        {
            throw new Exception($"{from.GetType().FullName}映射到${typeof(T).FullName}时失败:{ex.Message},{ex}", ex);
        }
    }

    /// <summary>
    /// 从实体创建模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from"></param>
    /// <returns></returns>
    public static T ToObject<T>(this object from)
    {
        try
        {
            if (typeof(T).IsGenericType && typeof(T).IsAssignableTo(typeof(IList)) && from is IList)
            {
                var toListType = typeof(T);
                var elementType = typeof(T).GetGenericArguments()[0];
                var toList = (IList)Activator.CreateInstance(typeof(T))!;
                var fromList = (IList)from;
                foreach (var item in fromList)
                {
                    toList.Add(Activator.CreateInstance(elementType).FromObject(item));
                }
                return (T)toList;
            }
            return (T)Activator.CreateInstance<T>().InjectFrom<DeepInjection>(from);
        }
        catch (Exception ex)
        {
            throw new Exception($"{from.GetType().FullName}映射到${typeof(T).FullName}时失败:{ex.Message},{ex}", ex);
        }
    }

    private class DeepInjection : LoopInjection
    {
        protected override bool MatchTypes(Type sourceType, Type targetType)
        {
            if (sourceType != typeof(string) &&
                targetType != typeof(string) &&
                sourceType.IsGenericType &&
                targetType.IsGenericType &&
                sourceType.IsAssignableTo(typeof(IEnumerable)) &&
                sourceType.IsAssignableTo(typeof(IEnumerable))
                )
            {
                return true;
            }
            return base.MatchTypes(sourceType, targetType);
        }

        protected override void SetValue(object source, object target, PropertyInfo sp, PropertyInfo tp)
        {
            if (tp.GetCustomAttribute<IgnoreUpdateAttribute>() != null)
            {
                return;
            }
            if (sp.PropertyType != typeof(string) &&
                sp.PropertyType != typeof(string) &&
                sp.PropertyType.IsAssignableTo(typeof(IEnumerable)) &&
                tp.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            {
                var targetGenericType = tp.PropertyType.GetGenericArguments()[0];
                var listType = typeof(List<>).MakeGenericType(targetGenericType);
                var addMethod = listType.GetMethod("Add");
                var list = Activator.CreateInstance(listType);
                var sourceList = (IEnumerable)sp.GetValue(source);
                foreach (var item in sourceList)
                {
                    addMethod.Invoke(list, new[] { Activator.CreateInstance(targetGenericType).FromObject(item) });
                }
                tp.SetValue(target, list);
                return;
            }
            base.SetValue(source, target, sp, tp);
        }
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class IgnoreUpdateAttribute : Attribute
{
}
