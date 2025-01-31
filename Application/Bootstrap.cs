using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using Microsoft.Data.SqlClient;
using JJ.NET.CrossData;
using JJ.NET.CrossData.Enumerador;
using JJ.NET.Data.Interfaces;
using JJ.NET.Data;
using System.Data.Common;
using Domain.Interfaces;
using InfraData.Repository;
using JJ.NET.CrossData.Extensao;
using Domain.Entidades;
using static System.Net.Mime.MediaTypeNames;

namespace Application
{
    public class Bootstrap
    {
        public static Container Container { get; private set; }

        public static void Inicializar()
        {
            try
            {
                ConfiguracaoBancoDados.IniciarConfiguracao(Conexao.SQLite, "Gerenciador de Senhas", @"C:\GerenciadorDeSenhas");

                Container = new Container();
                Container.Options.DefaultLifestyle = Lifestyle.Scoped;

                Container.Register<IUnitOfWork>(() => new UnitOfWork(ConfiguracaoBancoDados.ObterConexao()), Lifestyle.Singleton);

                // REPOSITORIOS
                 Container.Register<IGSCategoriaRepository, GSCategoriaRepository>(Lifestyle.Singleton);
                 Container.Register<IGSCredencialRepository, GSCredencialRepository>(Lifestyle.Singleton);

                // APP SERVICE
                // Container.Register<IConfiguracaoAppService, ConfiguracaoAppService>(Lifestyle.Singleton);
                // Container.Register<IGSCredencialAppService, GSCredencialAppService>(Lifestyle.Singleton);

                // VIEW MODELS
                // Bootstrap.Container.Register<IConfiguracaoViewModel, ConfiguracaoViewModel>(Lifestyle.Singleton);
                // Bootstrap.Container.Register<IGerenciadorSenhasViewModel, GerenciadorSenhasViewModel>(Lifestyle.Singleton);
                // Bootstrap.Container.Register<ICadastrarSenhaViewModel, CadastrarSenhaViewModel>(Lifestyle.Singleton);

                Container = Bootstrap.Container;
                Container.Verify();

                //IniciarBaseDeDados();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao se conectar ao banco de dados.\n", ex);
            }
            catch (IOException ex)
            {
                throw new Exception("Erro ao acessar arquivos de configuração.\n", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro inesperado.\n", ex);
            }
        }

        private static void IniciarBaseDeDados()
        {
            var uow = Container.GetInstance<IUnitOfWork>();

            CriarTabelas(uow);
            InserirInformacoesIniciais(uow);
        }
    
        private static void CriarTabelas(IUnitOfWork uow)
        {
            bool gSCategoria = false;
            bool gSCredencial = false;

            try
            {
                gSCategoria = uow.Connection.VerificarTabelaExistente<GSCategoria>();
                gSCredencial = uow.Connection.VerificarTabelaExistente<GSCredencial>();
            }
            catch (SqlException ex)
            {
                throw new Exception("Erro ao verificar a existência das tabelas", ex);
            }

            if (gSCategoria && gSCredencial)
                return;

            try
            {
                uow.Begin();

                if (!gSCategoria)
                    uow.Connection.CriarTabela<GSCategoria>(uow.Transaction);

                if (!gSCredencial)
                    uow.Connection.CriarTabela<GSCredencial>(uow.Transaction);

                uow.Commit();
            }
            catch (SqlException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao criar as tabelas no banco de dados", ex);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw new Exception("Erro inesperado ao criar as tabelas", ex);
            }

        }

        private static void InserirInformacoesIniciais(IUnitOfWork uow)
        {
            var gSCategoriaRepository = Container.GetInstance<IGSCategoriaRepository>();

            try
            {
                if (gSCategoriaRepository.ObterLista().ToList().Count() > 0)
                    return;

                var categorias = new string[]
                {
                    "Redes Sociais",
                    "Bancos e Finanças",
                    "E-commerce",
                    "Email",
                    "Trabalho",
                    "Streaming",
                    "Jogos",
                    "Lojas de Aplicativos",
                    "Fóruns",
                    "Plataformas de Ensino e Cursos",
                    "Celulares e Dispositivos Móveis",
                    "Computadores e Sistemas Operacionais",
                    "VPNs e Proxy",
                    "Carteiras Digitais",
                    "Serviços de Backup",
                };

                uow.Begin();

                for (int i = 0; i < categorias.Length; i++)
                    gSCategoriaRepository.Adicionar(new GSCategoria { Categoria = categorias[i] });

                uow.Commit();
            }
            catch (SqlException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao inserir informações iniciais", ex);
            }
            catch (IOException ex)
            {
                uow.Rollback();
                throw new Exception("Erro ao acessar arquivos durante a inserção de dados", ex);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw new Exception("Erro inesperado ao inserir informações iniciais", ex);
            }
        }
    }
}
