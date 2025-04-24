using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void CleanUp_Click(object sender, RoutedEventArgs e)
        {
            bool clearTemp = CheckTemp.IsChecked == true;
            bool clearRecycle = CheckRecycle.IsChecked == true;
            bool clearCache = CheckCache.IsChecked == true;
            bool clearLog = CheckLog.IsChecked == true;

            Button cleanupButton = sender as Button;
            cleanupButton.IsEnabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            worker.DoWork += (s, args) =>
            {
                int totalSteps = 0;
                if (clearTemp) totalSteps++;
                if (clearRecycle) totalSteps++;
                if (clearCache) totalSteps++;
                if (clearLog) totalSteps++;

                int currentStep = 0;

                if (clearTemp)
                {
                    TempCleaner.ClearTemp();
                    currentStep++;
                    (s as BackgroundWorker).ReportProgress(currentStep * 100 / totalSteps);
                }

                if (clearRecycle)
                {
                    RecycleCleaner.ClearRecycleBin();
                    currentStep++;
                    (s as BackgroundWorker).ReportProgress(currentStep * 100 / totalSteps);
                }

                Thread.Sleep(300);
            };

            worker.ProgressChanged += (s, args) =>
            {
                pbStatus.Value = args.ProgressPercentage;
            };

            worker.RunWorkerCompleted += (s, args) =>
            {
                cleanupButton.IsEnabled = true;
                MessageBox.Show("Очистка завершена!");
                LoadDiskInfo();
            };

            worker.RunWorkerAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}