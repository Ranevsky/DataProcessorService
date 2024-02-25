namespace DataProcessorService.Entities;

public class ModuleStatusEntity
{
    public int Id { get; set; }
    public string CategoryId { get; set; } = null!;
    public string State { get; set; } = null!;
}