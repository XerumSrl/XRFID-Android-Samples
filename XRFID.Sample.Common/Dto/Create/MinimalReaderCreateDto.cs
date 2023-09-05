namespace XRFID.Sample.Common.Dto.Create;

public class MinimalReaderCreateDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
