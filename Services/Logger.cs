using System;
using System.IO;

public static class Logger
{
    private static readonly string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
    private static bool isInitialized = false;

    public static void LogError(string message, Exception ex)
    {
        try
        {
            EnsureInitialized();
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}\n{ex}\n\n");
        }
        catch
        {
            // Не логируем ошибку логгера
        }
    }

    private static void EnsureInitialized()
    {
        if (!isInitialized)
        {
            try
            {
                File.WriteAllText(logPath, ""); // Очищаем лог
            }
            catch
            {
                // Игнорируем, если не удаётся очистить
            }

            isInitialized = true;
        }
    }
}
