using System.ComponentModel.DataAnnotations;

namespace XRFID.Demo.Server.Helpers;

internal static class EnumHelper
{
    internal static string GetDisplayText(Enum value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        Type type = value.GetType();
        if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
        {
            var sb = new System.Text.StringBuilder();

            foreach (Enum field in Enum.GetValues(type))
            {
                if (Convert.ToInt64(field) == 0 && Convert.ToInt32(value) > 0)
                    continue;

                if (value.HasFlag(field))
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    var f = type.GetField(field.ToString());
                    var da = (DisplayAttribute)Attribute.GetCustomAttribute(f, typeof(DisplayAttribute));
                    sb.Append(da?.ShortName ?? da?.Name ?? field.ToString());
                }
            }

            return sb.ToString();
        }
        else
        {
            var f = type.GetField(value.ToString());
            if (f != null)
            {
                var da = (DisplayAttribute)Attribute.GetCustomAttribute(f, typeof(DisplayAttribute));
                if (da != null)
                    return da.ShortName ?? da.Name;
            }
        }

        return value.ToString();
    }
}
