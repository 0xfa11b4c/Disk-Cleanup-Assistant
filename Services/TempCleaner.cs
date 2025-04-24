using System;
using System.IO;

public class TempCleaner
{
    public static void ClearTemp()
    {
        string tempPath = Path.GetTempPath();
        string systemTemp = @"C:\Windows\Temp";
        string localTemp = Environment.ExpandEnvironmentVariables("%TEMP%");

        ClearDirectory(localTemp);
        ClearDirectory(tempPath);
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
                catch { /* ingore */ }
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
