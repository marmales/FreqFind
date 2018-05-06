using FreqFind.Lib.ViewModels;
using FreqFind.Views;
using System.Windows;

namespace FreqFind
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += App_Startup;
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new WizardView();
            mainWindow.DataContext = new MainViewModel();
            mainWindow.Show();
        }
    }
}
