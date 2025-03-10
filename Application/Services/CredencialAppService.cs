﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entidades;
using Domain.Enumeradores;
using Domain.Interfaces;
using InfraData.Repository;
using JJ.NET.Core.DTO;
using JJ.NET.Core.Extensoes;
using JJ.NET.Core.Validador;
using JJ.NET.CrossData;
using JJ.NET.CrossData.DTO;
using JJ.NET.Cryptography;
using JJ.NET.Data;

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
        public GSCredencial PesquisarPorID(int PK_GSCredencial)
        {
            return gSCredencialRepository.Obter(PK_GSCredencial);
        }
        public IEnumerable<Item> ObterTipoDePesquisa()
        {
            return Enum.GetValues(typeof(TipoDePesquisa)).Cast<TipoDePesquisa>().Select(tp => new Item { ID = ((int)tp).ToString(), Valor = tp.ToString() });
        }
        public IEnumerable<GSCategoria> ObterCategorias()
        {
            return gSCategoriaRepository.ObterLista();
        }
        public int SalvarCredencial(GSCredencial gSCredencial)
        {
            if (gSCredencial == null)
                return -1;

            bool atualizarRegistro = (gSCredencial.PK_GSCredencial > 0);

            gSCredencial.ValidarResultado = new ValidarResultado();

            if (gSCredencial.Credencial.ObterValorOuPadrao("").Trim() == "")
            {
                gSCredencial.ValidarResultado.Adicionar("Credencial é um campo obrigatório.");
                return -1;
            }

            if (gSCredencial.Senha.ObterValorOuPadrao("").Trim() == "")
            {
                gSCredencial.ValidarResultado.Adicionar("Senha é um campo obrigatório.");
                return -1;
            }

            var credencial = new GSCredencial
            {
                PK_GSCredencial = gSCredencial.PK_GSCredencial,
                Credencial = gSCredencial.Credencial,
                DataCriacao = gSCredencial.DataCriacao,
                DataModificacao = (atualizarRegistro) ? DateTime.Now : gSCredencial.DataCriacao,
            };

            if (gSCredencial.FK_GSCategoria != null)
                credencial.FK_GSCategoria = gSCredencial.FK_GSCategoria;

            var criptografarRequest = new CriptografarRequest
            {
                TipoCriptografia = JJ.NET.Cryptography.Enumerador.TipoCriptografia.AES,
                Valor = gSCredencial.Senha,
                IV = gSCredencial.IVSenha.ObterValorOuPadrao(""),
            };

            var criptografarResult = Criptografia.Criptografar(criptografarRequest);

            if (criptografarResult.Erro.ObterValorOuPadrao("").Trim() != "")
            {
                gSCredencial.ValidarResultado.Adicionar(criptografarResult.Erro);
                return -1;
            }

            credencial.Senha = criptografarResult.Valor;
            credencial.IVSenha = criptografarResult.IV;

            int PK_GESCredencial = -1;


            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCredencialRepository = new GSCredencialRepository(uow);
                try
                {
                    uow.Begin();

                    if (atualizarRegistro)
                    {
                        PK_GESCredencial = _gSCredencialRepository.Atualizar(credencial);
                    }
                    else
                    {
                        PK_GESCredencial = _gSCredencialRepository.Adicionar(credencial);
                    }

                    uow.Commit();
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw new Exception(ex.Message);
                }
            }

            if (atualizarRegistro)
                return PK_GESCredencial > 0 ? gSCredencial.PK_GSCredencial : -1;

            return PK_GESCredencial;
        }
        public bool DeletarCredencial(int PK_GSCredencial)
        {
            bool ret = false;

            using (var uow = new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()))
            {
                var _gSCredencialRepository = new GSCredencialRepository(uow);
                try
                {
                    uow.Begin();

                    var result = _gSCredencialRepository.Deletar(PK_GSCredencial);

                    uow.Commit();

                    if (result > 0)
                        ret = true;
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    throw new Exception(ex.Message);
                }
            }

            return ret;
        }
        #endregion
    }
}
