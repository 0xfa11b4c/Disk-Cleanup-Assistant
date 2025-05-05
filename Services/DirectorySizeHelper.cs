using System.IO;

namespace Quick_Disk_Cleanup_Helper.Services
{
    public static class DirectorySizeHelper
    {
        public static long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            long size = 0;

            try
            {
                foreach (string file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        size += fi.Length;
                    }
                    catch { }
                }
            }
            catch { }

            return size;
        }
    }
}
