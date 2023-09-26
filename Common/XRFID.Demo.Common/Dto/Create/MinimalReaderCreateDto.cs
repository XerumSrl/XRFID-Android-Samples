namespace XRFID.Demo.Common.Dto.Create;

public class MinimalReaderCreateDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
