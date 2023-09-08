namespace XRFID.Sample.Server.Entities;

public abstract class AuditEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Reference { get; set; }

    public string? CreatorUserId { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public string? LastModifierUserId { get; set; }
    public DateTime LastModificationTime { get; set; } = DateTime.Now;
}
