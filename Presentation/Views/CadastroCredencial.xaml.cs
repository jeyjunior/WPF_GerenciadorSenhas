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
using Application.Services;
using Domain.Entidades;
using Domain.Enumeradores;
using JJ.NET.Core.Extensoes;

namespace Presentation.Views
{
    public partial class CadastroCredencial : Window, IDisposable
    {
        #region Interface
        private readonly ICredencialAppService _credencialAppService;
        private readonly IConfiguracaoAppService _configuracaoAppService;
        #endregion

        #region Propriedades Publicas
        public GSCredencial GSCredencialAtualizada { get; private set; }
        #endregion

        #region Construtor
        public CadastroCredencial(GSCredencial gSCredencial)
        {
            InitializeComponent();

            _credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
            _configuracaoAppService = Bootstrap.Container.GetInstance<IConfiguracaoAppService>();

            GSCredencialAtualizada = gSCredencial;
        }
        #endregion

        #region Eventos
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindComboBoxCategoria();
            AtualizarComponentes();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var gSCredencial = new GSCredencial
                {
                    PK_GSCredencial = GSCredencialAtualizada.PK_GSCredencial,
                    Credencial = txtCredencial.Text,
                    Senha = txtSenha.Password,
                    IVSenha = GSCredencialAtualizada.IVSenha.ObterValorOuPadrao(""),
                    DataCriacao = (GSCredencialAtualizada.PK_GSCredencial > 0) ? GSCredencialAtualizada.DataCriacao : DateTime.Now,
                    DataModificacao = null,
                    FK_GSCategoria = (int)cboCategoria.SelectedValue,
                };

                var PK_GSCredencial = _credencialAppService.SalvarCredencial(gSCredencial);

                if (!gSCredencial.ValidarResultado.EhValido)
                    throw new Exception(gSCredencial.ValidarResultado.Erros.ToList()[0]);

                if (PK_GSCredencial <= 0)
                    throw new Exception("Falha ao tentar salvar credencial, certifique-se que todas as informações estão corretas.");

                MessageBox.Show("Credencial salva com sucesso.");

                GSCredencialAtualizada = _credencialAppService.PesquisarPorID(PK_GSCredencial);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                txtCredencial.Focus();
            }
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

        private void txtSenha_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (txtSenha.IsVisible)
                txtSenhaVisivel.Text = txtSenha.Password;
        }

        private void txtSenhaVisivel_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtSenhaVisivel.IsVisible)
                txtSenha.Password = txtSenhaVisivel.Text;
        }
        #endregion

        #region Metodos
        private void AtualizarComponentes()
        {
            if (GSCredencialAtualizada.PK_GSCredencial == 0)
            {
                lblTitulo.Content = "Cadastrar Credencial";

                txtCredencial.Text = "";
                txtSenha.Password = "";
                cboCategoria.SelectedIndex = 0;
            }
            else
            {
                lblTitulo.Content = "Atualizar Credencial";

                var criptografiaRequest = new CriptografiaRequest { Valor = GSCredencialAtualizada.Senha, IV = GSCredencialAtualizada.IVSenha };
                string senha = _configuracaoAppService.Descriptografar(criptografiaRequest);

                if (!criptografiaRequest.ValidarResultado.EhValido)
                {
                    // AVISAR ERRO COM MENSAGEM PERSONALIZADA
                    throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);
                    this.Close();
                }

                txtSenha.Password = senha;
                txtCredencial.Text = GSCredencialAtualizada.Credencial;

                if (GSCredencialAtualizada.FK_GSCategoria != null)
                    cboCategoria.SelectedValue = GSCredencialAtualizada.FK_GSCategoria.ObterValorOuPadrao(0);
            }

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
