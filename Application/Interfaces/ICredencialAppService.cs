using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entidades;
using JJ.NET.Core.DTO;

namespace Application.Interfaces
{
    public interface ICredencialAppService
    {
        IEnumerable<GSCredencial> Pesquisar(GSCredencialPesquisaRequest requisicao);
        GSCredencial PesquisarPorID(int PK_GSCredencial);
        IEnumerable<Item> ObterTipoDePesquisa();
        IEnumerable<GSCategoria> ObterCategorias();
        bool SalvarCredencial(GSCredencial gSCredencial);
    }
}
