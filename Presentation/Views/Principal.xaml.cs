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
        int indiceSelecionado = 0;
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

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {

            //try
            //{
            //    if (dtgCredencial.Items.Count <= 0)
            //        throw new Exception("Pesquise ou cadastre alguma credencial antes de realizar essa operação.");

            //    if (dtgCredencial.SelectedItems.Count <= 0) 
            //        throw new Exception("Nenhuma credencial selecionada.");

            //    indiceSelecionado = dtgCredencial.SelectedIndex;

            //    if (!PodeAlterarCredencial(out int PK_GSCredencial))
            //        throw new Exception("Não foi possível obter as informações da credencial selecionada.");

            //    var gSCredencial = _credencialAppService.PesquisarPorID(PK_GSCredencial);

            //    if (gSCredencial == null)
            //        throw new Exception("Não foi possível obter as informações da credencial selecionada.");

            //    CadastroCredencial cadastroCredencial = new CadastroCredencial(gSCredencial);
            //    cadastroCredencial.ShowDialog();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    Pesquisar();
            //}
        }

        private void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    var linha = dtgCredencial.SelectedItem as CredencialView;

            //    if (linha == null)
            //        return;

            //    linha.ExibirSenha = !linha.ExibirSenha;

            //    int PK_GESCredencial = linha.PK_GSCredencial;

            //    var gSCredencial = _credencialAppService.PesquisarPorID(PK_GESCredencial);

            //    if (gSCredencial == null)
            //        throw new Exception("Não foi possível obter as informações de senha da credencial.");

            //    if (linha.ExibirSenha)
            //    {
            //        var criptografiaRequest = new CriptografiaRequest
            //        {
            //            Valor = gSCredencial.Senha,
            //            IV = gSCredencial.IVSenha,
            //        };

            //        string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

            //        if (!criptografiaRequest.ValidarResultado.EhValido)
            //            throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);

            //        linha.SenhaVisivel = senhaDescriptografada;
            //    }
            //    else
            //    {
            //        linha.SenhaVisivel = gSCredencial.Senha.Ocultar();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
            //finally
            //{
            //    dtgCredencial.Items.Refresh();
            //}
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

            lblTotal.Content = "Total: " + ret.Count();
        }

        private void BindPrincipal(IEnumerable<GSCredencial> gSCredencials)
        {
            if (gSCredencials != null)
            {
                var resultado = gSCredencials.Select(i => new CredencialView(_configuracaoAppService)
                {
                    PK_GSCredencial = i.PK_GSCredencial,
                    DataModificacao = i.DataModificacao?.ToShortDateString() ?? "",
                    Categoria = i.GSCategoria?.Categoria ?? "",
                    Credencial = i.Credencial,
                    SenhaVisivel = OcultarSenha(i.Senha, i.IVSenha),
                    SenhaCriptografada = i.Senha,
                    SenhaIV = i.IVSenha,
                    
                    // Define ações para os botões
                    OnExcluir = ExcluirItem,
                    OnAlterar = AlterarItem
                }).ToList();

                listaCredenciais.ItemsSource = resultado;
            }
            else
            {
                listaCredenciais.ItemsSource = new List<CredencialView>();
            }
        }

        private string OcultarSenha(string senhaCriptografada, string senhaIV)
        {
            var criptografiaRequest = new CriptografiaRequest { Valor = senhaCriptografada, IV = senhaIV };

            string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

            if (!criptografiaRequest.ValidarResultado.EhValido)
                throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);

            return senhaDescriptografada.Ocultar();
        }

        private void ExcluirItem(int id)
        {
            var confirmacao = MessageBox.Show("Deseja excluir esta credencial?", "Confirmação", MessageBoxButton.YesNo);
            if (confirmacao == MessageBoxResult.Yes)
            {
                // Lógica para excluir
                MessageBox.Show($"Credencial {id} excluída.");
                //AtualizarLista();
            }
        }

        private void AlterarItem(int id)
        {
            MessageBox.Show($"Abrindo edição da credencial {id}");
            // Lógica para abrir a tela de edição
        }

        #endregion
    }
}
