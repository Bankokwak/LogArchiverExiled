using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace LogArchiver;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;

    public bool Debug { get; set; }
    
    [Description("Enable archive for the log directory")]
    public bool LogArchive { get; set; } = true;
    
    [Description("The patch for every log directory")]
    public List<string> filePath { get; set; } = new List<string>();
    
    [Description("The maximum number of log files that can be archived")]
    public int MaxLogNumber { get; set; } = 50;
    
    [Description("The maximum number of archive files that can be contain in each log")]
    public int MaxArchiveLogNumber { get; set; } = 50;
    
    [Description("Enable archive for the RA log file")]
    public bool LogRaArchive { get; set; } = true;
    
    [Description("The path for the RA log directory")]
    public string LogRaDirectory { get; set; } = "";
    
    [Description("The max size of the RA log in octet (1000 octets = 1 Ko)")]
    public int MaxRaLogSize { get; set; } = 500000;
    
    [Description("The maximum number of archive files that can be contain in RA log")]
    public int MaxRaLogArchive { get; set; } = 25;
}