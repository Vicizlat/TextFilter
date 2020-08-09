using System.Windows;
using TextFilter.ViewModels;

namespace TextFilter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainViewModel.WaitWindow.Owner = this;
        }
    }
}