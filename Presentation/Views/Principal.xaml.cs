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
using Domain.Interfaces;
using Application;
using Application.Interfaces;

namespace Presentation.Views
{
    /// <summary>
    /// Lógica interna para Principal.xaml
    /// </summary>
    public partial class Principal : Window
    {
        #region Interfaces
        private readonly ICredencialAppService _credencialAppService;
        #endregion
        public Principal()
        {
            InitializeComponent();

            _credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ret = _credencialAppService.Pesquisar();
        }
    }
}
