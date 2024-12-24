using MelonLoader;
using MelonLoader.Utils;
using System.IO;
using UnityEngine;

[assembly: MelonInfo(typeof(Logger), "Unity Logger", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

public class Logger : MelonMod
{
    private static StreamWriter logFileWriter;

    public override void OnInitializeMelon()
    {
        string path = Path.Combine(MelonEnvironment.GameRootDirectory + "\\Logs");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fileTimeStamp = System.DateTime.Now.ToString("MM-dd-yy_HH-mm-ss");
        string logTimeStamp = "---------- || Debug Log || " + System.DateTime.Now.ToString("MM/dd/yy || HH-mm-ss") +  " || ----------";

        string logFilePath = Path.Combine(path, $"DebugLog_{fileTimeStamp}.txt");
        logFileWriter = new StreamWriter(logFilePath, true) { AutoFlush = true };

        if (logFileWriter != null)
        {
            logFileWriter.WriteLine($"{logTimeStamp}");
        }

        Application.logMessageReceived += HandleLog;

        MelonLogger.Msg("Logger initialized. Logs will be written to '.\\Logs'");
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"[{type}] {logString}";

        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                MelonLogger.Error(logEntry);
                break;
            case LogType.Warning:
                MelonLogger.Warning(logEntry);
                break;
            default:
                MelonLogger.Msg(logEntry);
                break;
        }

        if (logFileWriter != null)
        {
            logFileWriter.WriteLine($"{System.DateTime.Now}: {logEntry}");
            if (!string.IsNullOrEmpty(stackTrace) && (type == LogType.Error || type == LogType.Exception))
            {
                logFileWriter.WriteLine(stackTrace);
            }
        }
    }

    public override void OnDeinitializeMelon()
    {
        Application.logMessageReceived -= HandleLog;

        if (logFileWriter != null)
        {
            logFileWriter.Close();
            logFileWriter = null;
        }

        MelonLogger.Msg("Logger deinitialized. Log file closed.");
    }
}
