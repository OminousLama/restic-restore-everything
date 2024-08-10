using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ResticRestoreEverything.Models;
using ResticRestoreEverything.Models.Etc;
using ResticRestoreEverything.Models.Restic;

namespace ResticRestoreEverything.Services;

public class RestoreService(LoggerService _logger, string repo, string repoSecret, string target, bool dryRun)
{
    private Dictionary<SnapshotInfo, List<string>> FilesBySnapshot { get; set; } = new();
    private List<SnapshotInfo> _allSnaps = new();
    
    #region Indexing
    public void IndexFindings(string findingsFile)
    {
        string pattern = @"snapshot (\w+) from (\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})";
        string[] lines = File.ReadAllLines(findingsFile);

        _logger.LogInfo("Indexing snapshots and files...");
        SnapshotInfo? currentSnapshot = null;
        foreach (string line in lines)
        {
            Match match = Regex.Match(line, pattern);
            
            #region Begin new snapshot
            if (match.Success)
            {
                _logger.LogDebug($"Found new snapshot begin line: {line}");

                if (currentSnapshot != null)
                {
                    _allSnaps.Add(currentSnapshot);
                }
                
                string snapshotId = match.Groups[1].Value;
                DateTime date = DateTime.Parse(match.Groups[2].Value);
                currentSnapshot = new SnapshotInfo()
                {
                    DateTime = date,
                    Id = snapshotId
                };
                continue;
            }
            #endregion
            #region Add to existing snapshot
            if (currentSnapshot == null)
            {
                _logger.LogFatal($"Got file path even though there is not current snapshot to index it to. Findings file might be corrupt.");
                return;
            }

            if (line.Length > 0)
            {
                currentSnapshot.Files.Add(line);
            }
            else
            {
                _logger.LogDebug("Skipping empty line.");
            }
            
            #endregion
        }
        _logger.LogInfo("Snapshots and files indexed.");
        #region Transform
        _logger.LogInfo("Transforming index...");

        _allSnaps = _allSnaps.OrderByDescending(x => x.DateTime).ToList();
        foreach (var snap in _allSnaps)
        {
            List<string> snapFileList = new();
            FilesBySnapshot.Add(snap, snapFileList);
            foreach (var file in snap.Files)
            {
                if (IsFileAlreadyIndexInFinal(file)) continue;
                
                snapFileList.Add(file);
            }
        }
        
        _logger.LogInfo("Transformation done:");
        _logger.LogInfo(JsonSerializer.Serialize(FilesBySnapshot.ToDictionary(pair => pair.Key.Id, pair => pair.Value)));
        #endregion
    }

    public void RestoreFiles()
    {
        foreach (var snap in FilesBySnapshot.Keys)
        {
            RestoreSnapshot(snap);
        }
    }

    private void RestoreSnapshot(SnapshotInfo snap)
    {
        FilesBySnapshot.TryGetValue(snap, out List<string>? values);
        if (values == null || values.Count == 0)
        {
            _logger.LogInfo($"No files to restore from snapshot '{snap.Id}'.");
            return;
        }
        
        foreach (var file in values)
        {
            RestoreSnapshot(snap.Id, file);
        }
    }
    
    private void RestoreSnapshot(string snapid, string file)
    {
        _logger.LogInfo($"Restoring '{file}' from '{snapid}'...");

        string args = $"-r \"{repo}\" --password-command=\"echo '{repoSecret}'\" restore \"{snapid}\" --path \"{file}\" --target \"{target}\"";

        var res = DispatchResticProcess(args);
        if (res.ExitCode != 0)
        {
            _logger.LogError($"Got a non-zero exit code while restoring file '{file}' from snapshot '{snapid}'. Restic output: \n{res.Combined}");
        }
        else
        {
            _logger.LogInfo($"Restored file '{file}' from snapshot '{snapid}'. Restic output: \n{res.Combined}");
        }
    }

    private RestoreProcessResult DispatchResticProcess(string arguments)
    {
        string command = "restic";

        // Create a new process
        using Process process = new Process();
        // Configure the process using the StartInfo properties
        process.StartInfo.FileName = command;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        StringBuilder outputBuilder = new StringBuilder();
        StringBuilder errorBuilder = new StringBuilder();
        StringBuilder combinedBuilder = new StringBuilder();

        // Capture the output/results
        process.OutputDataReceived += (sender, args) =>
        {
            outputBuilder.AppendLine(args.Data);
            combinedBuilder.AppendLine(args.Data);
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            errorBuilder.AppendLine(args.Data);
            combinedBuilder.AppendLine(args.Data);
        };

        // Start the process
        process.Start();

        // Begin reading the output
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        // Wait for the process to exit
        process.WaitForExit();

        // Get the output
        string output = outputBuilder.ToString();
        string error = errorBuilder.ToString();

        return new()
        {
            ExitCode = process.ExitCode,
            Error = error,
            Out = output,
            Combined = combinedBuilder.ToString(),
        };
    }
    
    private bool IsFileAlreadyIndexInFinal(string file)
    {
        foreach (var fileList in FilesBySnapshot.Values)
        {
            if (fileList.Contains(file)) return true;
        }

        return false;
    }
    #endregion
}