using AlinaLib;
using AlinaLib.Domain.Entity;
using Microsoft.Win32;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;

namespace AlinaWpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string FolderPath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            FolderPath = AppDomain.CurrentDomain.BaseDirectory.ToString();
            //Class1 dffdf;
            //DirectoryWatcher dfdsfa = new DirectoryWatcher("c:"); dfdsfa.Start();


            // FolderDialog fileDialog = new OpenFolderDialog();
            // fileDialog.ShowDialog();
            //var text = fileDialog.FileName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                SelectedPath = FolderPath
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.FolderPath = dialog.SelectedPath;
               
               // System.Windows.MessageBox.Show(folderName);
            }
        }
    }
}