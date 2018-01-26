using HttpSimulateForWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HttpSimulateForWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const string URL_PATH = "url.conf";
        const string HEADER_PATH = "header.conf";
        const string DATA_PATH = "data.conf";



        MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _viewModel.Model.Stop();
            WriteFile();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = new MainWindowViewModel();
            this.DataContext = _viewModel;

            ReadFile();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btnSwitch = sender as Button;
            btnSwitch.IsEnabled = false;

            if (btnSwitch.Content.ToString() == "开始监听")
            {
                if (_viewModel.Model.Start(out string msg))
                {
                    btnSwitch.Content = "停止监听";
                }
                else
                {
                    MessageBox.Show($"启动失败！错误信息:{msg}");
                }
            }
            else
            {
                _viewModel.Model.Stop();
                btnSwitch.Content = "开始监听";
            }

            btnSwitch.IsEnabled = true;
        }

        private void Button_Filter_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Model.SetCanResponse();
        }

        void ReadFile()
        {
            using (FileStream fs = new FileStream(URL_PATH, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var buffer = new byte[1024 * 1024 * 1];
                var flag = fs.Read(buffer, 0, buffer.Length);
                var data = Encoding.UTF8.GetString(buffer, 0, flag);
                if (string.IsNullOrWhiteSpace(data))
                {
                    data = "http://127.0.0.1:10086/api/test/";
                }

                _viewModel.Model.Host = data;
            }

            using (FileStream fs = new FileStream(HEADER_PATH, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var buffer = new byte[1024 * 1024 * 5];
                var flag = fs.Read(buffer, 0, buffer.Length);
                var data = Encoding.UTF8.GetString(buffer, 0, flag);
                if (string.IsNullOrWhiteSpace(data))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Content-Type:text/html");
                    sb.AppendLine("Status Code:200");
                    data = sb.ToString();
                }

                _viewModel.Model.ResponseHeader = data;
            }

            using (FileStream fs = new FileStream(DATA_PATH, FileMode.OpenOrCreate, FileAccess.Read))
            {
                var buffer = new byte[1024 * 1024 * 5];
                var flag = fs.Read(buffer, 0, buffer.Length);
                _viewModel.Model.ResponseBody = Encoding.UTF8.GetString(buffer, 0, flag);
            }
        }


        void WriteFile()
        {
            using (FileStream fs = new FileStream(URL_PATH, FileMode.Create, FileAccess.Write))
            {
                var buffer = Encoding.UTF8.GetBytes(_viewModel.Model.Host);
                fs.Write(buffer, 0, buffer.Length);
            }

            using (FileStream fs = new FileStream(HEADER_PATH, FileMode.Create, FileAccess.Write))
            {
                var buffer = Encoding.UTF8.GetBytes(_viewModel.Model.ResponseHeader);
                fs.Write(buffer, 0, buffer.Length);
            }

            using (FileStream fs = new FileStream(DATA_PATH, FileMode.Create, FileAccess.Write))
            {
                var buffer = Encoding.UTF8.GetBytes(_viewModel.Model.ResponseBody);
                fs.Write(buffer, 0, buffer.Length);
            }
        }

    }
}
