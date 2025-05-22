using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using STC.WPF.ViewModels;
using STC.WPF.Views;

namespace STC.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //public MainWindow()
        //{
        //    InitializeComponent();
        //    MainFrame.Content = new InputView();
        //}

        public MainWindow()
        {
            InitializeComponent();
            InputViewInit();
        }

        public void InputViewInit()
        {
            var inputView = new InputView();
            inputView.DataContext = new InputPageViewModel();
            MainFrame.Content = inputView;
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void InputUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            InputViewInit();
        }

        private void ReferenceUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ReferenceView();
        }

        private void ReportUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ReportView();
        }

        private void AboutUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new AboutView();
        }
    }
}