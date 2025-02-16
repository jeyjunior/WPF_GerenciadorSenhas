using System;
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

    public int PK_GSCredencial { get; set; }
    public string DataModificacao { get; set; }
    public string Categoria { get; set; }
    public string Credencial { get; set; }

    private string _senhaVisivel;
    public string SenhaVisivel
    {
        get => _senhaVisivel;
        set
        {
            if (_senhaVisivel != value)
            {
                _senhaVisivel = value;
                OnPropertyChanged(nameof(SenhaVisivel)); 
            }
        }
    }

    public string SenhaCriptografada { get; set; }
    public string SenhaIV { get; set; }
    private bool exibirSenha { get; set; }
    public ICommand ExcluirCommand { get; }
    public ICommand AlterarCommand { get; }
    public ICommand ExibirSenhaCommand { get; }
    public Action<int> OnExcluir { get; set; }
    public Action<int> OnAlterar { get; set; }

    public CredencialView(IConfiguracaoAppService _configuracaoAppService)
    {
        ExcluirCommand = new CommandHandler(() => Excluir(), true);
        AlterarCommand = new CommandHandler(() => Alterar(), true);
        ExibirSenhaCommand = new CommandHandler(() => ExibirSenha(), true);

        this._configuracaoAppService = _configuracaoAppService;
    }

    private void Excluir()
    {
        OnExcluir?.Invoke(PK_GSCredencial);
    }

    private void Alterar()
    {
        OnAlterar?.Invoke(PK_GSCredencial);
    }

    private void ExibirSenha()
    {
        try
        {
            exibirSenha = !exibirSenha;

            if (exibirSenha)
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
}
