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
using Application;
using Application.Interfaces;

namespace Presentation.Views
{
    public partial class CadastroCredencial : Window, IDisposable
    {
        #region Interface
        private readonly ICredencialAppService _credencialAppService;
        #endregion

        #region Propriedades
        #endregion

        #region Construtor
        public CadastroCredencial()
        {
            InitializeComponent();

            _credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
        }
        #endregion

        #region Eventos
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindComboBoxCategoria();
            LimparComponentes();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnConfigurarCredencial_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGerarCredencialAleatoria_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnConfigurarSenha_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGerarSenhaAleatoria_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        {
            if (txtSenha.Visibility == Visibility.Visible)
            {
                txtSenhaVisivel.Text = txtSenha.Password;
                txtSenha.Visibility = Visibility.Collapsed;
                txtSenhaVisivel.Visibility = Visibility.Visible;
            }
            else
            {
                txtSenha.Password = txtSenhaVisivel.Text;
                txtSenha.Visibility = Visibility.Visible;
                txtSenhaVisivel.Visibility = Visibility.Collapsed;
            }
        }

        private void btnCadastrarCategoria_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Metodos
        private void LimparComponentes()
        {
            txtCredencial.Text = "";
            txtSenha.Password = "";
            cboCategoria.SelectedIndex = 0;

            txtCredencial.Focus();
        }

        private void BindComboBoxCategoria()
        {
            cboCategoria.ItemsSource = _credencialAppService.ObterCategorias().ToList();
            cboCategoria.DisplayMemberPath = "Categoria";
            cboCategoria.SelectedValuePath = "PK_GSCategoria";
            cboCategoria.SelectedValue = "0";
        }

        public void Dispose()
        {
            GC.Collect();
        }
        #endregion
    }
}
