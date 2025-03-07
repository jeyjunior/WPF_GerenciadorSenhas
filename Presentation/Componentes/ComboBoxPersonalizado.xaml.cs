using JJ.NET.Core.DTO;
using Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation.Componentes
{
    public partial class ComboBoxPersonalizado : UserControl
    {
        public ComboBoxPersonalizadoViewModel ViewModel { get; set; }
        public ComboBoxPersonalizado()
        {
            InitializeComponent();

            this.DataContext = ViewModel;
        }

        private void btnComboBox_Click(object sender, RoutedEventArgs e)
        {
            popupItens.IsOpen = !popupItens.IsOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var item = listBox.SelectedItem as Item;

            int numeroDeItens = ViewModel.Itens.Count;
            double alturaDaLinha = 24;
            double alturaMaxima = alturaDaLinha * 5;
            double alturaMinima = alturaDaLinha;

            popupItens.Height = ((numeroDeItens < 5) ? alturaDaLinha * numeroDeItens : alturaMaxima);

            if (popupItens.Height < alturaMinima)
                popupItens.Height = alturaMinima;
        }
    }
}
