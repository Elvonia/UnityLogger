using System.IO;
using UnityEngine;

#if BEPINEX
using BepInEx;

[BepInPlugin("com.github.Elvonia.UnityLogger", "Unity Logger", PluginInfo.PLUGIN_VERSION)]
public class Logger : BaseUnityPlugin
{
    private void LogError(string message)
    {
        Logger.LogError(message);
    }

    private void LogWarning(string message)
    {
        Logger.LogWarning(message);
    }

    private void LogInfo(string message)
    {
        Logger.LogInfo(message);
    }

    public void Awake()
    {
        string path = Path.Combine(Paths.GameRootPath, "Logs");
        CommonAwake(path);
    }

    public void OnDestroy()
    {
        CommonDestroy();
    }

#elif MELONLOADER
using MelonLoader;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(Logger), "Unity Logger", PluginInfo.PLUGIN_VERSION, "Kalico")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

public class Logger : MelonMod
{
    private void LogError(string message)
    {
        MelonLogger.Error(message);
    }

    private void LogWarning(string message)
    {
        MelonLogger.Warning(message);
    }

    private void LogInfo(string message)
    {
        MelonLogger.Msg(message);
    }

    public override void OnInitializeMelon()
    {
        string path = Path.Combine(MelonEnvironment.GameRootDirectory, "Logs");
        CommonAwake(path);
    }

    public override void OnDeinitializeMelon()
    {
        CommonDestroy();
    }

#endif

    private static StreamWriter logFileWriter;

    private void CommonAwake(string path)
    {
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

        LogInfo("Logger initialized. Logs will be written to '.\\Logs'");
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"[{type}] {logString}";

        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                LogError(logEntry);
                break;
            case LogType.Warning:
                LogWarning(logEntry);
                break;
            default:
                LogInfo(logEntry);
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

    private void CommonDestroy()
    {
        Application.logMessageReceived -= HandleLog;

        if (logFileWriter != null)
        {
            logFileWriter.Close();
            logFileWriter = null;
        }

        LogInfo("Logger deinitialized. Log file closed.");
    }
}
