using Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IConfiguracaoAppService
    {
        string Descriptografar(CriptografiaRequest criptografiaRequest);
        string Criptografar(CriptografiaRequest criptografiaRequest);
    }
}
