using Application;
using Application.Interfaces;
using Presentation.ViewModels;
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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Interfaces
        private readonly ICredencialAppService credencialAppService;
        #endregion

        #region Propriedades
        #endregion

        public MainWindowViewModel ViewModel => (MainWindowViewModel)this.DataContext;
        public MainWindow()
        {
            InitializeComponent();
            credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();

            this.DataContext = new MainWindowViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.TipoDeOrdenacao = credencialAppService.ObterTipoDeOrdenacao();
            ViewModel.TipoDePesquisa = credencialAppService.ObterTipoDePesquisa();

            ViewModel.SelecionarTipoDeOrdenacao(0);
            ViewModel.SelecionarTipoDePesquisa(0);
        }
    }
}