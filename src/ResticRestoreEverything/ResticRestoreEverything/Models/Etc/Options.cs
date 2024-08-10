using CommandLine;

namespace ResticRestoreEverything.Models.Etc;

public class Options
{
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option('f', "file", Required = true, HelpText = "Restic findings file to use.")]
    public string FindingsFile { get; set; } = default!;
    
    [Option('r', "repo", Required = true, HelpText = " Restic repository the files should be restored from.")]
    public string Repo { get; set; } = default!;
    
    [Option('s', "secret", Required = true, HelpText = "Restic repository secret.")]
    public string Secret { get; set; } = default!;
    
    [Option('t', "target", Required = true, HelpText = "Target directory files will be restored to.")]
    public string Target { get; set; } = default!;
}