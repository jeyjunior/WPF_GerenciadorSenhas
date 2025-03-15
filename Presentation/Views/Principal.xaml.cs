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
using System.ComponentModel;
using JJ.NET.Core.DTO;
using System.DirectoryServices;

namespace Presentation.Views
{
    public partial class Principal : Window, INotifyPropertyChanged
    {
        #region Interfaces
        private readonly ICredencialAppService _credencialAppService;
        private readonly IConfiguracaoAppService _configuracaoAppService;
        #endregion

        #region Propriedades
        int indiceSelecionado = 0;
        private ObservableCollection<CredencialView> _credenciais;
        private ObservableCollection<Item> _listaDeItens;
        public ObservableCollection<Item> ListaDeItens
        {
            get { return _listaDeItens; }
            set
            {
                _listaDeItens = value;
                OnPropertyChanged(nameof(ListaDeItens));
            }
        }
        private IEnumerable<GSCredencial> gSCredencials;
        private TipoDeOrdenacao ultimaOrdenacao;
        private SortDirection direcaoOrdenacao;

        private string _itemSelecionado;
        public string ItemSelecionado
        {
            get { return _itemSelecionado; }
            set
            {
                _itemSelecionado = value;
                OnPropertyChanged(nameof(ItemSelecionado));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Propriedades Publicas
        public event PropertyChangedEventHandler PropertyChanged;
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
            CarregarComboBoxTipoDeOrdenacao();
            CarregarComboBoxTipoDePesquisa();
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

                if (cadastroCredencial.CredencialSalva)
                    AtualizarCredencialView(cadastroCredencial.GSCredencialAtualizada);
                
                AtualizarStatus();
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
        private void btnOrdenacao_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Item tipoDeOrdenacao = cboTipoOrdenacao.ViewModel.ItemSelecionado;

                switch ((TipoDeOrdenacao)tipoDeOrdenacao.ID.ConverterParaInt32(0))
                {
                    case TipoDeOrdenacao.Cadastro:
                        if (ultimaOrdenacao == TipoDeOrdenacao.Cadastro && direcaoOrdenacao == SortDirection.Ascending)
                        {
                            gSCredencials = gSCredencials.OrderByDescending(i => i.DataCriacao).ToList();
                            direcaoOrdenacao = SortDirection.Descending;
                        }
                        else
                        {
                            gSCredencials = gSCredencials.OrderBy(i => i.DataCriacao).ToList();
                            direcaoOrdenacao = SortDirection.Ascending;
                        }

                        break;
                    case TipoDeOrdenacao.Modificação:
                        if (ultimaOrdenacao == TipoDeOrdenacao.Modificação && direcaoOrdenacao == SortDirection.Ascending)
                        {
                            gSCredencials = gSCredencials.OrderByDescending(i => i.DataModificacao).ToList();
                            direcaoOrdenacao = SortDirection.Descending;
                        }
                        else
                        {
                            gSCredencials = gSCredencials.OrderBy(i => i.DataModificacao).ToList();
                            direcaoOrdenacao = SortDirection.Ascending;
                        }

                        break;
                    case TipoDeOrdenacao.Categoria:
                        if (ultimaOrdenacao == TipoDeOrdenacao.Categoria && direcaoOrdenacao == SortDirection.Ascending)
                        {
                            gSCredencials = gSCredencials.OrderByDescending(i => i.GSCategoria?.Categoria.ObterValorOuPadrao("").Trim()).ToList();
                            direcaoOrdenacao = SortDirection.Descending;
                        }
                        else
                        {
                            gSCredencials = gSCredencials.OrderBy(i => i.GSCategoria?.Categoria.ObterValorOuPadrao("").Trim()).ToList();
                            direcaoOrdenacao = SortDirection.Ascending;
                        }

                        break;
                    case TipoDeOrdenacao.Credencial:

                        if (ultimaOrdenacao == TipoDeOrdenacao.Credencial && direcaoOrdenacao == SortDirection.Ascending)
                        {
                            gSCredencials = gSCredencials.OrderByDescending(i => i.Credencial).ToList();
                            direcaoOrdenacao = SortDirection.Descending;
                        }
                        else
                        {
                            gSCredencials = gSCredencials.OrderBy(i => i.Credencial).ToList();
                            direcaoOrdenacao = SortDirection.Ascending;
                        }

                        break;
                    default:
                        break;
                }

                BindPrincipal();
                AtualizarUltimaOrdenacao();
                AtualizarStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Metodos
        private void CarregarComboBoxTipoDePesquisa()
        {
            cboTipoDePesquisa.ViewModel.Itens = _credencialAppService.ObterTipoDePesquisa();
            cboTipoDePesquisa.ViewModel.ItemSelecionado = cboTipoDePesquisa.ViewModel.Itens[0];
        }

        private void CarregarComboBoxTipoDeOrdenacao()
        {
            cboTipoOrdenacao.ViewModel.Itens = _credencialAppService.ObterTipoDeOrdenacao();
            cboTipoOrdenacao.ViewModel.ItemSelecionado = cboTipoOrdenacao.ViewModel.Itens[0];
        }
        private void Pesquisar()
        {
            try
            {
                Item tipoDeOrdenacao = cboTipoOrdenacao.ViewModel.ItemSelecionado;
                Item tipoDePesquisa = cboTipoDePesquisa.ViewModel.ItemSelecionado;

                var requisicao = new GSCredencialPesquisaRequest
                {
                    Valor = txtPesquisar.Text,
                    TipoDePesquisa = (TipoDePesquisa)tipoDePesquisa.ID.ConverterParaInt32(0),
                    TipoDeOrdenacao = (TipoDeOrdenacao)tipoDeOrdenacao.ID.ConverterParaInt32(0)
                };

                gSCredencials = _credencialAppService.Pesquisar(requisicao);
                BindPrincipal();
                AtualizarUltimaOrdenacao();
                AtualizarStatus();

                direcaoOrdenacao = SortDirection.Ascending;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void BindPrincipal()
        {
            if (gSCredencials != null && gSCredencials.Count() > 0)
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

                AtualizarStatus();
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

            if (cadastroCredencial.CredencialSalva)
                AtualizarCredencialView(cadastroCredencial.GSCredencialAtualizada);

            AtualizarStatus();
        }
        private void AtualizarCredencialView(GSCredencial gSCredencial)
        {
            if (gSCredencial == null)
                return;

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
        private void AtualizarStatus()
        {
            lblTotal.Content = "";

            if (_credenciais.Count > 0)
                lblTotal.Content = "Total: " + _credenciais.Count.ToString("N0");
        }
        
        private void AtualizarUltimaOrdenacao()
        {
            ultimaOrdenacao = (TipoDeOrdenacao)((Item)cboTipoOrdenacao.ViewModel.ItemSelecionado).ID.ConverterParaInt32();
        }
        #endregion
    }
}
