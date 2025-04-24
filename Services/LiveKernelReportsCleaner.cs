using System.IO;
using System;

public class LiveKernelReportsCleaner
{
    public static void ClearLiveKernelReports()
    {
        string systemTemp = @"C:\Windows\LiveKernelReports";
        ClearDirectory(systemTemp);
    }

    private static void ClearDirectory(string path)
    {
        try
        {
            if (!Directory.Exists(path))
                return;

            foreach (string file in Directory.GetFiles(path))
            {
                try { File.Delete(file); }
                catch { /* ignore */ }
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                try { Directory.Delete(dir, true); }
                catch { /* ignore */ }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при очистке " + path + ": " + ex.Message);
        }
    }
}