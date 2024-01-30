using System.Windows;

namespace AlinaWpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); DirectoryWatcher dfdsfa = new DirectoryWatcher(""); dfdsfa.Start(); 
            
        }
    }
}