namespace ResticRestoreEverything.Models.Restic;

public class SnapshotInfo
{
    public DateTime DateTime { get; set; } = default!;
    public string Id { get; set; } = default!;
    public List<string> Files { get; set; } = new();
}