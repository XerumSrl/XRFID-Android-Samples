using Xerum.XFramework.Common.Enums;

namespace XRFID.Demo.Common.Dto;

public class TagDto
{
    public TagDto()
    {
        Tid = string.Empty;
        PC = string.Empty;
        Rssi = 0;
        Timestamp = DateTime.Now;
        TagSeenCount = 1;
        ItemStatus = ItemStatus.None;
    }

    public Guid Id { get; set; }

    public string Epc { get; set; }

    public string Tid { get; set; }

    public string PC { get; set; }

    public short Rssi { get; set; }

    public DateTime Timestamp { get; set; }

    public int TagSeenCount { get; set; }

    public ItemStatus ItemStatus { get; set; }

    public byte[] ByteEpc { get; set; }

    public byte[] ByteTid { get; set; }

    public string Antenna { get; set; }

    public int InventoryId { get; set; }

    public DateTime TagFirstSeen { get; set; }

    public DateTime TagLastSeen { get; set; }

    public bool IsValid { get; set; }

    public bool IsActive { get; set; }

    public bool IsDuplicate { get; set; }

    public bool IsRegExValid { get; set; }

    public bool ToBeProcessed { get; set; }

    public DateTime? TimeTagExpire { get; set; }

    public Guid SessionId { get; set; }

    public Guid ReaderId { get; set; }
}
