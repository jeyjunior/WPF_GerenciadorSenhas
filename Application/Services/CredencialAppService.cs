using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entidades;
using Domain.Interfaces;

namespace Application.Services
{
    internal class CredencialAppService : ICredencialAppService
    {
        private readonly IGSCredencialRepository gSCredencialRepository;

        public CredencialAppService()
        {
             gSCredencialRepository = Bootstrap.Container.GetInstance<IGSCredencialRepository>();   
        }

        public IEnumerable<GSCredencial> Pesquisar()
        {
            return gSCredencialRepository.ObterLista();
        }
    }
}
