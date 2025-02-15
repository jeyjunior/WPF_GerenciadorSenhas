using Application.Interfaces;
using Domain.Entidades;
using JJ.NET.Core.Extensoes;
using JJ.NET.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ConfiguracaoAppService : IConfiguracaoAppService
    {
        public string Descriptografar(CriptografiaRequest criptografiaRequest)
        {
            if (criptografiaRequest == null)
                return "";

            criptografiaRequest.ValidarResultado = new JJ.NET.Core.Validador.ValidarResultado();

            var criptografiaResult = Criptografia.Descriptografar(new DescriptografarRequest { Valor = criptografiaRequest.Valor, IV = criptografiaRequest.IV, TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES });

            if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
            {
                criptografiaRequest.ValidarResultado.Adicionar(criptografiaResult.Erro);
                return "";
            }

            return criptografiaResult.Valor;
        }
        public string Criptografar(CriptografiaRequest criptografiaRequest)
        {
            if (criptografiaRequest == null)
                return "";

            criptografiaRequest.ValidarResultado = new JJ.NET.Core.Validador.ValidarResultado();

            var criptografiaResult = Criptografia.Descriptografar(new DescriptografarRequest { Valor = criptografiaRequest.Valor, IV = criptografiaRequest.IV, TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES });

            if (criptografiaResult.Erro.ObterValorOuPadrao("").Trim() != "")
            {
                criptografiaRequest.ValidarResultado.Adicionar(criptografiaResult.Erro);
                return "";
            }

            return criptografiaResult.Valor;
        }
    }
}
