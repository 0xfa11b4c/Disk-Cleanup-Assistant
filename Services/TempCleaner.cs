﻿using System;
using System.IO;

namespace Quick_Disk_Cleanup_Helper
{
    public class TempCleaner
    {
        public static void ClearTemp()
        {
            string[] pathsToClean =
            {
                Path.GetTempPath(),
                Environment.ExpandEnvironmentVariables("%TEMP%"),
                Environment.ExpandEnvironmentVariables("%LocalAppData%\\Temp"),
                @"C:\Windows\Temp",
                @"C:\Windows\Prefetch",
                Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\Recent")
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
