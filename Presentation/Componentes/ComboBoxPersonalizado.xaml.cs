using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using JJ.NET.Core.DTO;
using Presentation.ViewModel;

namespace Presentation.Componentes
{
    public partial class ComboBoxPersonalizado : UserControl
    {
        public ComboBoxPersonalizado()
        {
            InitializeComponent();

            ViewModel = new ComboBoxPersonalizadoViewModel();
            this.DataContext = ViewModel;
        }

        public ComboBoxPersonalizadoViewModel ViewModel { get; set; }

        public static readonly DependencyProperty ItensSourceProperty =
        DependencyProperty.Register(nameof(ItensSource), typeof(ObservableCollection<Item>), typeof(ComboBoxPersonalizado), new PropertyMetadata(null, OnItensSourceChanged));

        public ObservableCollection<Item> ItensSource
        {
            get => (ObservableCollection<Item>)GetValue(ItensSourceProperty);
            set => SetValue(ItensSourceProperty, value);
        }

        private static void OnItensSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBoxPersonalizado control && e.NewValue is ObservableCollection<Item> newCollection)
            {
                control.ViewModel.Itens = newCollection;
            }
        }

        private void btnComboBox_Click(object sender, RoutedEventArgs e)
        {
            popupItens.IsOpen = !popupItens.IsOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            popupItens.IsOpen = false;
        }
    }
}
