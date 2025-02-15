using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Domain.Interfaces;
using JJ.NET.Core.Extensoes;
using Domain.Enumeradores;
using Domain.Entidades;
using Application.Interfaces;
using Application;

namespace Presentation.Views
{
    public partial class Principal : Window
    {
        #region Interfaces
        private readonly ICredencialAppService _credencialAppService;
        private readonly IConfiguracaoAppService _configuracaoAppService;
        #endregion

        #region Propriedades
        #endregion

        #region Construtor
        public Principal()
        {
            InitializeComponent();

            _credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
            _configuracaoAppService = Bootstrap.Container.GetInstance<IConfiguracaoAppService>();
        }
        #endregion

        #region Eventos
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CarregarComboBoxTipoPesquisa();
            Pesquisar();
        }

        private void btnPesquisar_Click(object sender, RoutedEventArgs e)
        {
            Pesquisar();
        }

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CadastroCredencial cadastroCredencial = new CadastroCredencial();
                cadastroCredencial.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Pesquisar();
            }
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {

        }
        int indiceSelecionado = 0;

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
           

            try
            {
                if (dtgCredencial.Items.Count <= 0)
                    throw new Exception("Pesquise ou cadastre alguma credencial antes de realizar essa operação.");

                if (dtgCredencial.SelectedItems.Count <= 0) 
                    throw new Exception("Nenhuma credencial selecionada.");

                indiceSelecionado = dtgCredencial.SelectedIndex;

                if (!PodeAlterarCredencial(out int PK_GSCredencial))
                    throw new Exception("Não foi possível obter as informações da credencial selecionada.");

                var gSCredencial = _credencialAppService.PesquisarPorID(PK_GSCredencial);

                if (gSCredencial == null)
                    throw new Exception("Não foi possível obter as informações da credencial selecionada.");

                CadastroCredencial cadastroCredencial = new CadastroCredencial(gSCredencial);
                cadastroCredencial.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Pesquisar();
            }
        }

        private void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var linha = dtgCredencial.SelectedItem as CredencialView;

                if (linha == null)
                    return;

                linha.ExibirSenha = !linha.ExibirSenha;

                int PK_GESCredencial = linha.PK_GSCredencial;

                var gSCredencial = _credencialAppService.PesquisarPorID(PK_GESCredencial);

                if (gSCredencial == null)
                    throw new Exception("Não foi possível obter as informações de senha da credencial.");

                if (linha.ExibirSenha)
                {
                    var criptografiaRequest = new CriptografiaRequest
                    {
                        Valor = gSCredencial.Senha,
                        IV = gSCredencial.IVSenha,
                    };

                    string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

                    if (!criptografiaRequest.ValidarResultado.EhValido)
                        throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);

                    linha.SenhaVisivel = senhaDescriptografada;
                }
                else
                {
                    linha.SenhaVisivel = gSCredencial.Senha.Ocultar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dtgCredencial.Items.Refresh();
            }
        }
        #endregion

        #region Metodos
        private void CarregarComboBoxTipoPesquisa()
        {
            cboTipoDePesquisa.ItemsSource = _credencialAppService.ObterTipoDePesquisa();
            cboTipoDePesquisa.DisplayMemberPath = "Nome";
            cboTipoDePesquisa.SelectedValuePath = "ID";
            cboTipoDePesquisa.SelectedValue = "0";
        }

        private void Pesquisar()
        {
            var tipoDePesquisa = cboTipoDePesquisa.SelectedValue.ToString();

            var requisicao = new GSCredencialPesquisaRequest
            {
                Valor = txtPesquisar.Text,
                TipoDePesquisa = (TipoDePesquisa)tipoDePesquisa.ConverterParaInt32(),
            };

            var ret = _credencialAppService.Pesquisar(requisicao);
            BindPrincipal(ret);

            if (indiceSelecionado < 0)
                indiceSelecionado = 0;

            if (dtgCredencial.Items.Count > 0)
                dtgCredencial.SelectedIndex = indiceSelecionado;
        }

        //private void BindPrincipal(IEnumerable<GSCredencial> gSCredencials)
        //{
        //    if (gSCredencials != null)
        //    {
        //        var resultado = gSCredencials.Select(i => new
        //        {
        //            PK_GSCredencial = i.PK_GSCredencial,
        //            DataModificacao = i.DataModificacao != null ? i.DataModificacao.Value.ToShortDateString() : "",
        //            Categoria = i.GSCategoria != null ? i.GSCategoria.Categoria : "",
        //            Credencial = i.Credencial,
        //            SenhaVisivel = i.Senha.Ocultar(),
        //        }).ToList();

        //        dtgCredencial.ItemsSource = resultado;
        //    }
        //    else
        //    {
        //        var resultado = new List<dynamic>()
        //        {
        //            new 
        //            {
        //                PK_GSCredencial = 0,
        //                DataModificacao = "",
        //                Categoria = "",
        //                Credencial = "",
        //                Senha = ""
        //            }
        //        };

        //        dtgCredencial.ItemsSource = resultado;
        //    }
        //}

        private void BindPrincipal(IEnumerable<GSCredencial> gSCredencials)
        {
            if (gSCredencials != null)
            {
                var resultado = gSCredencials.Select(i => new CredencialView
                {
                    PK_GSCredencial = i.PK_GSCredencial,
                    DataModificacao = i.DataModificacao != null ? i.DataModificacao.Value.ToShortDateString() : "",
                    Categoria = i.GSCategoria != null ? i.GSCategoria.Categoria : "",
                    Credencial = i.Credencial,
                    SenhaVisivel = i.Senha.Ocultar(), // Inicialmente oculta a senha
                }).ToList();

                dtgCredencial.ItemsSource = resultado;
            }
            else
            {
                var resultado = new List<CredencialView>
                {
                    new CredencialView
                    {
                        PK_GSCredencial = 0,
                        DataModificacao = "",
                        Categoria = "",
                        Credencial = "",
                        SenhaVisivel = ""
                    }
                };

                dtgCredencial.ItemsSource = resultado;
            }
        }

        private bool PodeAlterarCredencial(out int PK_GSCredencial)
        {
            PK_GSCredencial = 0;

            if (dtgCredencial.Items.Count == 0 || dtgCredencial.SelectedItem == null)
                return false;

            PK_GSCredencial = dtgCredencial.SelectedItem.ObterPropriedade("PK_GSCredencial", 0);

            if (PK_GSCredencial == 0)
                return false;

            return true;
        }
        #endregion

        //private void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        //{
        //    var btnExibir = sender as Button;
        //    var linha = dtgCredencial.SelectedItem;

        //    if (btnExibir == null)
        //        return;

        //    if (linha == null)
        //        return;

        //    bool exibirSenha = btnExibir.Tag.Converter<bool>();
        //    int PK_GESCredencial = linha.ObterPropriedade("PK_GSCredencial", 0);

        //    var gSCredencial = _credencialAppService.PesquisarPorID(PK_GESCredencial);

        //    if (gSCredencial == null)
        //        throw new Exception("Não foi possível obter as informações de senha da credencial.");

        //    if (exibirSenha)
        //    {
        //        var criptografiaRequest = new CriptografiaRequest 
        //        { 
        //            Valor = gSCredencial.Senha,
        //            IV = gSCredencial.IVSenha,
        //        };

        //        string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

        //        if (!criptografiaRequest.ValidarResultado.EhValido)
        //            throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);

        //        linha.DefinirValorParaPropriedade("SenhaVisivel", senhaDescriptografada);
        //    }
        //    else
        //    {
        //        linha.DefinirValorParaPropriedade("SenhaVisivel", gSCredencial.Senha.Ocultar());
        //    }

        //    btnExibir.Tag = !exibirSenha;
        //}
    }
}
