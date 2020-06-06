using System;
using UnityEngine;

public class LogHandler : Singleton<LogHandler>
{
    public System.Action<string, string> HandleExceptionCallback;

    protected override void Initialize()
    {
        Application.logMessageReceived += LogCallback;
    }

    protected override void Uninitialize()
    {
        Application.logMessageReceived -= LogCallback;
    }

    public void AddException(Exception ex)
    {
        LogCallback(ex.Message, ex.StackTrace, LogType.Exception);
#if UNITY_EDITOR || !DISTRIBUTION_VERSION
        UnityEngine.Debug.LogError(ex);
#endif
    }

    public void AddException(string condition)
    {
        LogCallback(condition, new System.Diagnostics.StackTrace().ToString(), LogType.Exception);
    }

    public void AddException(string condition, string stackTrace)
    {
        LogCallback(condition, stackTrace, LogType.Exception);
    }

    private void LogCallback(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Assert)
        {
#if UNITY_EDITOR
            if (type == LogType.Assert)
            {
                if (condition.StartsWith("Screen position out of view frustum")
                    || condition.StartsWith("Invalid AABB")
                     || condition.StartsWith("Assertion failed on expression: 'IsFinite(outDistanceForSort)'")
                      || condition.StartsWith("Assertion failed on expression: 'IsFinite(outDistanceAlongView)'")
                      || condition.StartsWith("Converting invalid MinMaxAABB"))
                {
                    return;
                }
            }
#endif
            if (HandleExceptionCallback != null)
            {
                HandleExceptionCallback(condition, stackTrace);
            }
        }
    }


}