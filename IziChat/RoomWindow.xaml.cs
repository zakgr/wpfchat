using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace IziChat
{
    /// <summary>
    /// Interaction logic for RoomWindow.xaml
    /// </summary>
    public partial class RoomWindow
    {
        public RoomWindow(MainWindow paWindow)
        {

            InitializeComponent();
            MainWindow = paWindow;
        }

        public static readonly DependencyProperty MainWindowProperty = DependencyProperty.Register(
            "MainWindow", typeof(MainWindow), typeof(RoomWindow), new PropertyMetadata(default(MainWindow)));

        public MainWindow MainWindow
        {
            get { return (MainWindow) GetValue(MainWindowProperty); }
            set { SetValue(MainWindowProperty, value); }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try { this.Close(); } catch { }
        }
    }
}
