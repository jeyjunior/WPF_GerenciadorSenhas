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
        #endregion

        #region Propriedades
        #endregion

        #region Construtor
        public Principal()
        {
            InitializeComponent();

            _credencialAppService = Bootstrap.Container.GetInstance<ICredencialAppService>();
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
        }

        private void BindPrincipal(IEnumerable<GSCredencial> gSCredencials)
        {
            if (gSCredencials != null)
            {
                var resultado = gSCredencials.Select(i => new
                {
                    PK_GSCredencial = i.PK_GSCredencial,
                    DataModificacao = i.DataModificacao != null ? i.DataModificacao.Value.ToShortDateString() : "",
                    Categoria = i.GSCategoria != null ? i.GSCategoria.Categoria : "",
                    Credencial = i.Credencial,
                    Senha = i.Senha
                }).ToList();

                dtgCredencial.ItemsSource = resultado;
            }
            else
            {
                var resultado = new List<dynamic>()
                {
                    new 
                    {
                        PK_GSCredencial = 0,
                        DataModificacao = "",
                        Categoria = "",
                        Credencial = "",
                        Senha = ""
                    }
                };

                dtgCredencial.ItemsSource = resultado;
            }
        }
        #endregion

        private void btnAdicionar_Click(object sender, RoutedEventArgs e)
        {
            CadastroCredencial cadastroCredencial = new CadastroCredencial();
            cadastroCredencial.ShowDialog();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAlterar_Click(object sender, RoutedEventArgs e)
        {
            if (dtgCredencial.Items.Count <= 0)
                return;

            var gSCredencialSelecionada = dtgCredencial.SelectedItem;

            if (gSCredencialSelecionada == null)
                return;

            var pK_GSCredencial = gSCredencialSelecionada.GetType().GetProperty("PK_GSCredencial")?.GetValue(gSCredencialSelecionada); ;

            if (pK_GSCredencial == null)
                return;

            var gSCredencial = _credencialAppService.PesquisarPorID(pK_GSCredencial.ObterValorOuPadrao(0));

            if (gSCredencial != null)
            {
                CadastroCredencial cadastroCredencial = new CadastroCredencial(gSCredencial);
                cadastroCredencial.ShowDialog();
            }
        }
    }
}
