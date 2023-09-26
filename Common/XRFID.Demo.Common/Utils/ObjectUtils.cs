using System.Reflection;
using System.Text.Json;

namespace XRFID.Demo.Common.Utils;

public static class ObjectUtils
{
    //public static T DeepCopy<T>(T other)
    //{
    //    if (other is null)
    //    {
    //        return other;
    //    }
    //    Type otherType = other.GetType();
    //    object? oValue = Activator.CreateInstance(otherType) ?? throw new Exception("HOW?!");
    //    PropertyInfo[] properties = otherType.GetProperties();
    //    foreach (PropertyInfo property in properties)
    //    {
    //        dynamic? propertyValue = property.GetValue(other);
    //        if (propertyValue is null)
    //        {
    //            property.SetValue(oValue, null);
    //            continue;
    //        }
    //        if (!propertyValue.GetType().IsPrimitive)
    //        {
    //            property.SetValue(oValue, DeepCopy(propertyValue));
    //            continue;
    //        }
    //        property.SetValue(oValue, propertyValue);
    //    }
    //    return (T)oValue;
    //}

    public static T DeepCopy<T>(T other)
    {
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(other)) ?? throw new Exception();
    }

    public static bool GenericEquals<T>(T a, T b)
    {
        if (ReferenceEquals(a, b))// both null and same reference fall in this guard
        {
            return true;
        }

        if (a is null ^ b is null)//exclusive or 
        {
            return false;
        }

        PropertyInfo[] properties = a.GetType().GetProperties();//fake warning neither a nor b are null here

        foreach (PropertyInfo property in properties)
        {
            bool equal = false;
            dynamic? valueA = property.GetValue(a);
            dynamic? valueB = property.GetValue(b);
            if (valueA is null && valueB is null)
            {
                continue;
            }
            if (a is null ^ b is null)//exclusive or 
            {
                return false;
            }

            Type type = valueA.GetType();
            if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime))
            {
                equal = valueA == valueB;
            }
            else
            {
                equal = GenericEquals(valueA, valueB);
            }

            if (!equal)
            {
                return false;
            }
        }
        return true;
    }
}
