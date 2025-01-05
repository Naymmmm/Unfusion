using System;
using System.Diagnostics;

namespace LabFusion.Utilities;

public static class ProcessUtils
{
    private static bool ProcessRunningInternal(string processName)
    {
        // Check if the process name is valid
        if (string.IsNullOrEmpty(processName))
        {
            throw new ArgumentException("Process name cannot be null or empty.", nameof(processName));
        }

        // Get all processes with the specified name
        Process[] processes = Process.GetProcessesByName(processName);

        // Return true if any processes are found
        return processes.Length > 0;
    }

    public static bool ProcessRunning(string args)
    {

        return ProcessRunningInternal(args);
    }
}
