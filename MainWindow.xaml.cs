using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quick_Disk_Cleanup_Helper.Services;

namespace Quick_Disk_Cleanup_Helper
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DiskInfoModel _diskInfo;
        public DiskInfoModel DiskInfo
        {
            get => _diskInfo;
            set
            {
                _diskInfo = value;
                OnPropertyChanged(nameof(DiskInfo));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadDiskInfo();
        }

        private void LoadDiskInfo()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == "C:\\")
                {
                    DiskInfo = DiskInfoModel.FromDriveInfo(drive);
                    break;
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void CleanUp_Click(object sender, RoutedEventArgs e)
        {
            bool clearTemp = CheckTemp.IsChecked == true;
            bool clearRecycle = CheckRecycle.IsChecked == true;
            bool clearCache = CheckCache.IsChecked == true;
            bool clearLog = CheckLog.IsChecked == true;
            bool clearWinUpdCache = CheckWindowsCache.IsChecked == true;

            Button cleanupButton = sender as Button;
            cleanupButton.IsEnabled = false;
            pbStatus.Value = 0;

            int totalSteps = (clearTemp ? 1 : 0) + (clearRecycle ? 1 : 0) + (clearCache ? 1 : 0) + (clearLog ? 1 : 0) + (clearWinUpdCache ? 1 : 0);
            int currentStep = 0;

            if (totalSteps == 0)
            {
                MessageBox.Show("Select at least one cleanup category.");
                cleanupButton.IsEnabled = true;
                return;
            }

            long totalFreedBytes = 0;

            try
            {
                await Task.Run(() =>
                {
                    void ReportProgress()
                    {
                        currentStep++;
                        Dispatcher.Invoke(() =>
                        {
                            pbStatus.Value = currentStep * 100 / totalSteps;
                        });
                    }

                    if (clearTemp)
                    {
                        try
                        {
                            string tempPath = Path.GetTempPath();
                            long before = DirectorySizeHelper.GetDirectorySize(tempPath);

                            TempCleaner.ClearTemp();

                            long after = DirectorySizeHelper.GetDirectorySize(tempPath);
                            totalFreedBytes += Math.Max(0, before - after);
                        }
                        catch (Exception ex) { Logger.LogError("TempCleaner", ex); }
                        ReportProgress();
                    }

                    if (clearRecycle)
                    {
                        try
                        {
                            RecycleCleaner.ClearRecycleBin();
                        }
                        catch (Exception ex) { Logger.LogError("RecycleCleaner", ex); }
                        ReportProgress();
                    }

                    if (clearCache)
                    {
                        try
                        {
                            string path = @"C:\ProgramData\Microsoft\Windows\WER\ReportQueue";
                            long before = DirectorySizeHelper.GetDirectorySize(path);

                            LiveKernelReportsCleaner.ClearLiveKernelReports();

                            long after = DirectorySizeHelper.GetDirectorySize(path);
                            totalFreedBytes += Math.Max(0, before - after);
                        }
                        catch (Exception ex) { Logger.LogError("LiveKernelReportsCleaner", ex); }
                        ReportProgress();
                    }

                    if (clearLog)
                    {
                        try
                        {
                            // Заглушка — если нет реализации, можешь добавить позже
                            // totalFreedBytes += ...
                        }
                        catch (Exception ex) { Logger.LogError("ClearLogs", ex); }
                        ReportProgress();
                    }

                    if (clearWinUpdCache)
                    {
                        try
                        {
                            string[] winUpdPaths =
                            {
                                Environment.ExpandEnvironmentVariables(@"C:\Windows\SoftwareDistribution\Download"),
                                @"C:\Windows\SoftwareDistribution\DataStore"
                            };

                            long before = winUpdPaths.Sum(p => DirectorySizeHelper.GetDirectorySize(p));

                            WindowsUpdateCacheCleaner.ClearWinUpdCache();

                            long after = winUpdPaths.Sum(p => DirectorySizeHelper.GetDirectorySize(p));
                            totalFreedBytes += Math.Max(0, before - after);
                        }
                        catch (Exception ex) { Logger.LogError("Windows Update Cleaner", ex); }
                        ReportProgress();
                    }

                    Thread.Sleep(300);
                });

                double freedMB = totalFreedBytes / (1024.0 * 1024);
                string resultMessage = $"Очистка завершена!\nОсвобождено: {freedMB:F2} МБ";
                MessageBox.Show(resultMessage);

                LoadDiskInfo();
            }
            catch (Exception ex)
            {
                Logger.LogError("Critical cleanup failure", ex);
                MessageBox.Show("Произошла ошибка во время очистки.");
            }
            finally
            {
                cleanupButton.IsEnabled = true;
                if (pbStatus.Value == 100)
                {
                    await Task.Delay(5000);
                    pbStatus.Value = 0;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}