using System.Windows;
using System.Windows.Input;
using STC.WPF.ViewModels;
using STC.WPF.Views;

namespace STC.WPF
{
    public partial class MainWindow : Window
    {
        private readonly InputPageViewModel _inputVM = new();
        private InputView _inputView;

        private readonly ReferenceViewModel _referenceVM = new();
        private ReferenceView _referenceView;

        private readonly ReportViewModel _reportVM = new();
        private ReportView _reportView;

        private AboutView _aboutView;

        public MainWindow()
        {
            InitializeComponent();
            InputViewInit();
        }

        public ReportViewModel ReportVM { get; private set; } = new ReportViewModel();
        public void InputViewInit()
        {
            var inputView = new InputView();
            var inputVM = new InputPageViewModel(_reportVM);
            inputVM.ReportVM = ReportVM;

            inputView.DataContext = inputVM;
            MainFrame.Content = inputView;
        }

        private void ReferenceUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            if (_referenceView == null)
            {
                _referenceView = new ReferenceView();
                _referenceView.DataContext = _referenceVM;
            }

            MainFrame.Content = _referenceView;
        }

        private void ReportUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            if (_reportView == null)
            { 
                _reportView = new ReportView(); 
                _reportView.DataContext = _reportVM;
            }

            MainFrame.Content = _reportView;
        }

        private void AboutUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            if (_aboutView == null)
            { 
                _aboutView = new AboutView();
            }

            MainFrame.Content = _aboutView;
        }

        private void InputUserFormButton_Click(object sender, RoutedEventArgs e)
        {
            var inputVM = new InputPageViewModel(_reportVM);
            var inputView = new InputView()
            {
                DataContext = inputVM
            };
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
    }
}
