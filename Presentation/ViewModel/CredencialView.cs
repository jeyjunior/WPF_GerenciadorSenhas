using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using Domain.Entidades;
using JJ.NET.Core.Extensoes;
using Application.Services;
using Application.Interfaces;

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

    public GSCredencial GSCredencial { get; set; }

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

    private bool _exibirSenha;
    public bool ExibirSenha
    {
        get => _exibirSenha;
        set => SetProperty(ref _exibirSenha, value, nameof(ExibirSenha));
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
                    throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);

                SenhaVisivel = senhaDescriptografada;
            }
            else
            {
                SenhaVisivel = SenhaVisivel.Ocultar();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private void CopiarCredencial()
    {
        Clipboard.SetText(Credencial);
    }

    private void CopiarSenha()
    {
        var criptografiaRequest = new CriptografiaRequest
        {
            Valor = SenhaCriptografada,
            IV = SenhaIV,
        };

        string senhaDescriptografada = _configuracaoAppService.Descriptografar(criptografiaRequest);

        if (!criptografiaRequest.ValidarResultado.EhValido)
            throw new Exception(criptografiaRequest.ValidarResultado.Erros.ToList()[0]);

        Clipboard.SetText(senhaDescriptografada);
    }
}
