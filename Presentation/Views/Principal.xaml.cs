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
using Application;
using Application.Interfaces;
using Domain.Entidades;
using Domain.Enumeradores;
using JJ.NET.Core.Extensoes;

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
    }
}
