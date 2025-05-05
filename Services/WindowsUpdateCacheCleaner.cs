using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Disk_Cleanup_Helper.Services
{
    class WindowsUpdateCacheCleaner
    {
        public static void ClearWinUpdCache()
        {
            string[] pathsToClean =
            {
                Environment.ExpandEnvironmentVariables("C:\\Windows\\SoftwareDistribution\\Download"),
                @"C:\Windows\SoftwareDistribution\DataStore",
            };

            foreach (string path in pathsToClean)
            {
                ClearDirectory(path);
            }
        }



        private static void ClearDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                    return;

                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        var attributes = File.GetAttributes(file);
                        if ((attributes & FileAttributes.System) == 0)
                        {
                            File.Delete(file);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Ошибка при удалении файла {file}", ex);
                    }
                }

                foreach (string dir in Directory.GetDirectories(path))
                {
                    try
                    {
                        Directory.Delete(dir, true);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Ошибка при удалении директории {dir}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Ошибка при очистке пути {path}", ex);
            }
        }
    }
}
