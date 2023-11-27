using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MathCore.IO;

[Copyright("Iain Ballard", url = "https://stackoverflow.com/a/3504251/2353975")]
public static class Win32Processes
{
    /// <summary>
    /// Find out what process(es) have a lock on the specified file.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <returns>Processes locking the file</returns>
    /// <remarks>See also:
    /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
    /// </remarks>
    public static IReadOnlyCollection<Process> GetLockingProcesses(string path)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Поддерживается только на платформе Windows на WinAPI");

        if (!Path.IsPathRooted(path))
            path = Path.GetFullPath(path);

        var key = Guid.NewGuid().ToString();
        var res = RmStartSession(out var handle, 0, key);

        if (res != 0)
            throw new InvalidOperationException("Could not begin restart session.  Unable to determine file locker.");

        try
        {
            const int more_data = 234;
            uint proc_info = 0;
            uint reboot_reason_none = __RebootReasonNone;

            string[] resources = [path]; // Just checking on one resource.

            res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

            if (res != 0) 
                throw new InvalidOperationException("Could not register resource.");

            //Note: there's a race condition here -- the first call to RmGetList() returns
            //      the total number of process. However, when we call RmGetList() again to get
            //      the actual processes this number may have increased.
            res = RmGetList(handle, out var proc_info_needed, ref proc_info, null, ref reboot_reason_none);

            if (res == more_data)
                return GetProcesses(proc_info_needed, handle, reboot_reason_none);
            else if (res != 0)
                throw new InvalidOperationException("Could not list processes locking resource. Failed to get size of result.");
        }
        finally
        {
            RmEndSession(handle);
        }

        return Array.Empty<Process>();
    }


    [StructLayout(LayoutKind.Sequential)]
    private struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    const int __RebootReasonNone = 0;
    const int CCH_RM_MAX_APP_NAME = 255;
    const int CCH_RM_MAX_SVC_NAME = 63;

    private enum RM_APP_TYPE
    {
        RmUnknownApp = 0,
        RmMainWindow = 1,
        RmOtherWindow = 2,
        RmService = 3,
        RmExplorer = 4,
        RmConsole = 5,
        RmCritical = 1000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_APP_NAME + 1)] public string strAppName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCH_RM_MAX_SVC_NAME + 1)] public string strServiceShortName;

        public RM_APP_TYPE ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;
        [MarshalAs(UnmanagedType.Bool)] public bool bRestartable;
    }

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    private static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[] rgsFilenames,
        uint nApplications, [In] RM_UNIQUE_PROCESS[] rgApplications, uint nServices,
        string[] rgsServiceNames);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
    private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

    [DllImport("rstrtmgr.dll")]
    private static extern int RmEndSession(uint pSessionHandle);

    [DllImport("rstrtmgr.dll")]
    private static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded,
        ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
        ref uint lpdwRebootReasons);

    private static Process[] GetProcesses(uint ProcInfoNeeded, uint handle, uint RebootReasons)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new PlatformNotSupportedException("Поддерживается только на платформе Windows на WinAPI");

        var processes = new List<Process>(10);
        // Create an array to store the process results
        var process_info = new RM_PROCESS_INFO[ProcInfoNeeded];
        var proc = ProcInfoNeeded;

        // Get the list
        var res = RmGetList(handle, out _, ref proc, process_info, ref RebootReasons);

        if (res != 0) throw new InvalidOperationException("Could not list processes locking resource.");
        for (var i = 0; i < proc; i++)
            try
            {
                processes.Add(Process.GetProcessById(process_info[i].Process.dwProcessId));
            }
            catch (ArgumentException)
            {
                // catch the error -- in case the process is no longer running
            }

        return [.. processes];
    }
}