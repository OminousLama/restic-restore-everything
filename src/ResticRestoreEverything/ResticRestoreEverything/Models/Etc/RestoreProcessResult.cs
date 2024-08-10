namespace ResticRestoreEverything.Models.Etc;

public class RestoreProcessResult
{
    public int ExitCode { get; set; } = 1;
    public string Error { get; set; } = default!;
    public string Out { get; set; } = default!;
    public string Combined { get; set; } = default!;
}