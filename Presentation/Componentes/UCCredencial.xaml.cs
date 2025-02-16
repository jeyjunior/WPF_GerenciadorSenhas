using System.Windows;
using System.Windows.Controls;

namespace Presentation.Componentes
{
    public partial class UCCredencial : UserControl
    {
        public UCCredencial()
        {
            InitializeComponent();
        }

        private void btnExibirSenha_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                MessageBox.Show("Exibir/Ocultar Senha para a credencial: " + button.Tag);
            }
        }

        private void btnCopiarSenha_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Copiar Senha");
        }
    }
}