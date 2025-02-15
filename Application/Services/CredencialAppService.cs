using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entidades;
using Domain.Enumeradores;
using Domain.Interfaces;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData.DTO;

namespace Application.Services
{
    internal class CredencialAppService : ICredencialAppService
    {
        #region Interface
        private readonly IGSCredencialRepository gSCredencialRepository;
        private readonly IGSCategoriaRepository gSCategoriaRepository;
        #endregion

        #region Construtor
        public CredencialAppService()
        {
            gSCredencialRepository = Bootstrap.Container.GetInstance<IGSCredencialRepository>();
            gSCategoriaRepository = Bootstrap.Container.GetInstance<IGSCategoriaRepository>();
        }
        #endregion

        #region Eventos
        #endregion

        #region Metodos
        public IEnumerable<GSCredencial> Pesquisar(GSCredencialPesquisaRequest requisicao)
        {
            if (requisicao == null)
                return new List<GSCredencial>();

            requisicao.ValidarResultado = new ValidarResultado();

            string condicao = "";
            string valor = requisicao.Valor.LimparEntradaSQL();

            switch (requisicao.TipoDePesquisa)
            {
                case TipoDePesquisa.Todos:
                    condicao = $" GSCategoria.Categoria COLLATE NOCASE LIKE '%{valor}%'   OR \n" +
                               $" GSCredencial.Credencial COLLATE NOCASE LIKE '%{valor}%' \n";
                    break;
                case TipoDePesquisa.Categoria:
                    condicao = $" GSCategoria.Categoria COLLATE NOCASE LIKE '%{valor}%' ";
                    break;
                case TipoDePesquisa.Credencial:
                    condicao = $" GSCredencial.Credencial COLLATE NOCASE LIKE '%{valor}%' ";
                    break;
                default:
                    break;
            }

            return gSCredencialRepository.ObterLista(condicao);
        }

        public IEnumerable<Item> ObterTipoDePesquisa()
        {
            return Enum.GetValues(typeof(TipoDePesquisa)).Cast<TipoDePesquisa>().Select(tp => new Item { ID = ((int)tp).ToString(), Nome = tp.ToString() });
        }

        public IEnumerable<GSCategoria> ObterCategorias()
        {
            return gSCategoriaRepository.ObterLista();
        }
        #endregion
    }
}
