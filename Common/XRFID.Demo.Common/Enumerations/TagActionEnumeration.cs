using Xerum.XFramework.Domain.Entities;

namespace XRFID.Demo.Common.Enumerations;

public class TagActionEnumeration : Enumeration<string>
{
    public TagActionEnumeration() : base()
    {

    }

    public TagActionEnumeration(string key, string displayValue) : base(key, displayValue)
    {

    }

    public static readonly TagActionEnumeration Read = new TagActionEnumeration("READ", "Tag read");

    public static readonly TagActionEnumeration NotRead = new TagActionEnumeration("NOTREAD", "Tag not found");

    public static readonly TagActionEnumeration Update = new TagActionEnumeration("UPDATE", "Update tag info");
}
