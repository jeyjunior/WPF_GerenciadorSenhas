using Application;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Bootstrap.Inicializar();
        }
    }
}
