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
using System.Collections.ObjectModel;
using System.Collections.Immutable;

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
        private ObservableCollection<CredencialView> _credenciais;
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
                CadastroCredencial cadastroCredencial = new CadastroCredencial(new GSCredencial());
                cadastroCredencial.ShowDialog();

                AtualizarCredencialView(cadastroCredencial.GSCredencialAtualizada);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {

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
                _credenciais = new ObservableCollection<CredencialView>(
                    gSCredencials.Select(i => new CredencialView(_configuracaoAppService)
                    {
                        PK_GSCredencial = i.PK_GSCredencial,
                        DataModificacao = i.DataModificacao?.ToShortDateString() ?? "",
                        Categoria = i.GSCategoria?.Categoria ?? "",
                        Credencial = i.Credencial,
                        SenhaVisivel = OcultarSenha(i.Senha, i.IVSenha),
                        SenhaCriptografada = i.Senha,
                        SenhaIV = i.IVSenha,

                        OnExcluir = ExcluirItem,
                        OnAlterar = AlterarItem
                    })
                );

                listaCredenciais.ItemsSource = _credenciais;
            }
            else
            {
                _credenciais = new ObservableCollection<CredencialView>();
                listaCredenciais.ItemsSource = _credenciais;
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

        private void ExcluirItem(CredencialView credencialView)
        {
            if (_credenciais != null && _credenciais.Contains(credencialView))
            {
                var ret = _credencialAppService.DeletarCredencial(credencialView.PK_GSCredencial);

                if (!ret)
                    throw new Exception("Falha inesperada ao tentar deletar item.");

                _credenciais.Remove(credencialView);
            }
        }

        private void AlterarItem(CredencialView credencialView)
        {
            if (credencialView == null)
                throw new Exception("Não foi possível encontrar o item para alterar.");

            if (!_credenciais.Contains(credencialView))
                throw new Exception("Não foi possível encontrar o item para alterar.");

            var gSCredencial = _credencialAppService.PesquisarPorID(credencialView.PK_GSCredencial);

            CadastroCredencial cadastroCredencial = new CadastroCredencial(gSCredencial);
            cadastroCredencial.ShowDialog();

            AtualizarCredencialView(cadastroCredencial.GSCredencialAtualizada);
        }

        private void AtualizarCredencialView(GSCredencial gSCredencial)
        {
            var item = _credenciais.FirstOrDefault(i => i.PK_GSCredencial == gSCredencial.PK_GSCredencial);

            if (item == null)
            {
                _credenciais.Add(new CredencialView(_configuracaoAppService)
                {
                    PK_GSCredencial = gSCredencial.PK_GSCredencial,
                    DataModificacao = gSCredencial.DataModificacao?.ToShortDateString() ?? "",
                    Categoria = gSCredencial.GSCategoria?.Categoria ?? "",
                    Credencial = gSCredencial.Credencial,
                    SenhaVisivel = OcultarSenha(gSCredencial.Senha, gSCredencial.IVSenha),
                    SenhaCriptografada = gSCredencial.Senha,
                    SenhaIV = gSCredencial.IVSenha,

                    OnExcluir = ExcluirItem,
                    OnAlterar = AlterarItem
                });
            }
            else
            {
                item.PK_GSCredencial = gSCredencial.PK_GSCredencial;
                item.DataModificacao = gSCredencial.DataModificacao?.ToShortDateString() ?? "";
                item.Categoria = gSCredencial.GSCategoria?.Categoria ?? "";
                item.Credencial = gSCredencial.Credencial;
                item.SenhaVisivel = OcultarSenha(gSCredencial.Senha, gSCredencial.IVSenha);
                item.SenhaCriptografada = gSCredencial.Senha;
                item.SenhaIV = gSCredencial.IVSenha;
            }
        }
        #endregion
    }
}
