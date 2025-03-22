using Application;
using System.Configuration;
using System.Data;
using System.Windows;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Bootstrap.Inicializar();
        }
    }
}
