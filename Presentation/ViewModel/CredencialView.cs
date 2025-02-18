using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;
using Domain.Entidades;
using JJ.NET.Core.Extensoes;
using Application.Services;
using Application.Interfaces;
using Presentation;

public class CredencialView : INotifyPropertyChanged
{
    private IConfiguracaoAppService _configuracaoAppService;

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T field, T value, string propertyName)
    {
        if (!Equals(field, value))
        {
            field = value;
            OnPropertyChanged(propertyName);
        }
    }

    private int _pkGSCredencial;
    public int PK_GSCredencial
    {
        get => _pkGSCredencial;
        set => SetProperty(ref _pkGSCredencial, value, nameof(PK_GSCredencial));
    }

    private string _dataModificacao;
    public string DataModificacao
    {
        get => _dataModificacao;
        set => SetProperty(ref _dataModificacao, value, nameof(DataModificacao));
    }

    private string _categoria;
    public string Categoria
    {
        get => _categoria;
        set => SetProperty(ref _categoria, value, nameof(Categoria));
    }
    private string _credencial;
    public string Credencial
    {
        get => _credencial;
        set => SetProperty(ref _credencial, value, nameof(Credencial));
    }

    private string _senhaVisivel;
    public string SenhaVisivel
    {
        get => _senhaVisivel;
        set => SetProperty(ref _senhaVisivel, value, nameof(SenhaVisivel));
    }
    public string SenhaCriptografada { get; set; }
    public string SenhaIV { get; set; }

    private string _iconeCopiarCredencial = "\uE8C8"; 
    public string IconeCopiarCredencial
    {
        get => _iconeCopiarCredencial;
        set => SetProperty(ref _iconeCopiarCredencial, value, nameof(IconeCopiarCredencial));
    }

    private bool _podeCopiarCredencial = true;
    public bool PodeCopiarCredencial
    {
        get => _podeCopiarCredencial;
        set => SetProperty(ref _podeCopiarCredencial, value, nameof(PodeCopiarCredencial));
    }
    private bool _podeCopiarSenha = true;
    public bool PodeCopiarSenha
    {
        get => _podeCopiarSenha;
        set => SetProperty(ref _podeCopiarSenha, value, nameof(PodeCopiarSenha));
    }

    private string _iconeCopiarSenha = "\uE8C8"; 
    public string IconeCopiarSenha
    {
        get => _iconeCopiarSenha;
        set => SetProperty(ref _iconeCopiarSenha, value, nameof(IconeCopiarSenha));
    }

    private bool _exibirSenha;
    public bool ExibirSenha
    {
        get => _exibirSenha;
        set => SetProperty(ref _exibirSenha, value, nameof(ExibirSenha));
    }

    private Brush _corBotaoExibirSenha;
    public Brush CorBotaoExibirSenha
    {
        get => _corBotaoExibirSenha;
        set => SetProperty(ref _corBotaoExibirSenha, value, nameof(CorBotaoExibirSenha));
    }

    public ICommand ExcluirCommand { get; }
    public ICommand AlterarCommand { get; }
    public ICommand ExibirSenhaCommand { get; }
    public ICommand CopiarCredencialCommand { get; }
    public ICommand CopiarSenhaCommand { get; }

    public Action<CredencialView> OnExcluir { get; set; }
    public Action<CredencialView> OnAlterar { get; set; }
    
    public CredencialView(IConfiguracaoAppService configuracaoAppService)
    {
        ExcluirCommand = new CommandHandler(() => Excluir(), true);
        AlterarCommand = new CommandHandler(() => Alterar(), true);
        ExibirSenhaCommand = new CommandHandler(() => AlternarExibicaoSenha(), true);
        CopiarCredencialCommand = new CommandHandler(() => CopiarCredencial(), true);
        CopiarSenhaCommand = new CommandHandler(() => CopiarSenha(), true);

        _configuracaoAppService = configuracaoAppService;

        CorBotaoExibirSenha = ObterCorDoEstilo("Nenhuma");
    }

    private void Excluir() => OnExcluir?.Invoke(this);
    private void Alterar() => OnAlterar?.Invoke(this);

    private void AlternarExibicaoSenha()
    {
        try
        {
            ExibirSenha = !ExibirSenha;

            if (ExibirSenha)
            {
                var criptografiaRequest = new CriptografiaRequest
                {
                    Valor = SenhaCriptografada,
                    IV = SenhaIV,
                };

                string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

                if (!criptografiaRequest.ValidarResultado.EhValido)
                    throw new Exception(criptografiaRequest.ValidarResultado.Erros.First());

                SenhaVisivel = senhaDescriptografada;

                CorBotaoExibirSenha = ObterCorDoEstilo("Cinza11");
            }
            else
            {
                SenhaVisivel = SenhaVisivel.Ocultar();

                CorBotaoExibirSenha = ObterCorDoEstilo("Nenhuma");
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private async void CopiarCredencial()
    {
        Clipboard.SetText(Credencial);
        IconeCopiarCredencial = "\uE001"; // Ícone de check
        PodeCopiarCredencial = false;   

        await Task.Delay(2000); 

        IconeCopiarCredencial = "\uE8C8"; // Volta ao ícone original
        PodeCopiarCredencial = true; 
    }
    private async void CopiarSenha()
    {
        try
        {
            var criptografiaRequest = new CriptografiaRequest
            {
                Valor = SenhaCriptografada,
                IV = SenhaIV,
            };

            string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

            if (!criptografiaRequest.ValidarResultado.EhValido)
                throw new Exception(criptografiaRequest.ValidarResultado.Erros.First());

            Clipboard.SetText(senhaDescriptografada);

            IconeCopiarSenha = "\uE001";
            PodeCopiarSenha = false;

            await Task.Delay(2000);

            IconeCopiarSenha = "\uE8C8";
            PodeCopiarSenha = true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    private Brush ObterCorDoEstilo(string chave)
    {
        if (App.Current.Resources.Contains(chave))
            return (Brush)App.Current.Resources[chave];

        return Brushes.Transparent; 
    }
}
