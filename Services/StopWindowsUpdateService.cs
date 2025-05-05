using System;
using System.ServiceProcess;

namespace Quick_Disk_Cleanup_Helper.Services
{
    public static class WindowsServiceHelper
    {
        public static void StopWindowsUpdateService()
        {
            try
            {
                using (ServiceController service = new ServiceController("wuauserv"))
                {
                    if (service.Status != ServiceControllerStatus.Stopped &&
                        service.Status != ServiceControllerStatus.StopPending)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Ошибка при остановке службы Windows Update", ex);
            }
        }
    }
}
