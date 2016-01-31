using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace TestWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "TestWPFWindow";
        }

        protected override void OnActivated(EventArgs e)
        {
            var http = GetHttp().Result;

            base.OnActivated(e);
        }

        private async Task<string> GetHttp2()
        {
            var msg = await new HttpClient().GetAsync("https://www.baidu.com");

            return await msg.Content.ReadAsStringAsync();
        }

        private Task<String> GetHttp()
        {
            return Task.Run(async () => {
                // We run on a thread pool thread now which has no SynchronizationContext on it
                HttpResponseMessage msg = await new HttpClient().GetAsync("http://Wintellect.com/");
                // We DO get here because some thread pool can execute this code
                return await msg.Content.ReadAsStringAsync();
            });
        }

    }
}
