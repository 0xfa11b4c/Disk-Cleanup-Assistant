using System;
using System.IO;

namespace Quick_Disk_Cleanup_Helper
{
    public class DiskInfoModel
    {
        public string Name { get; set; }
        public string FileSystem { get; set; }
        public double TotalSizeGB { get; set; }
        public double FreeSpaceGB { get; set; }
        public double UsedPercentage { get; set; }
        public string InfoString => $"{Name} {FreeSpaceGB:F1}GB / {TotalSizeGB:F1}GB";

        public static DiskInfoModel FromDriveInfo(DriveInfo drive)
        {
            double totalSize = drive.TotalSize / 1073741824.0;
            double freeSpace = drive.AvailableFreeSpace / 1073741824.0;
            double used = 100 - ((freeSpace / totalSize) * 100);

            return new DiskInfoModel
            {
                Name = drive.Name.TrimEnd('\\'),
                FileSystem = drive.DriveFormat,
                TotalSizeGB = totalSize,
                FreeSpaceGB = freeSpace,
                UsedPercentage = used
            };
        }
    }
}
