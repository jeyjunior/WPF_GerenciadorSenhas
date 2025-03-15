using JJ.NET.Core.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.ViewModel
{
    public class ComboBoxPersonalizadoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private Item _itemSelecionado;
        private ObservableCollection<Item> _itens;
        private double _alturaListBox;

        public ObservableCollection<Item> Itens
        {
            get => _itens;
            set
            {
                _itens = value;
                OnPropertyChanged(nameof(Itens));
                CalcularAlturaListBox();
            }
        }
        
        public double AlturaListBox
        {
            get => _alturaListBox;
            set
            {
                _alturaListBox = value;
                OnPropertyChanged(nameof(AlturaListBox));
            }
        }

        public Item ItemSelecionado
        {
            get => _itemSelecionado;
            set
            {
                _itemSelecionado = value;
                OnPropertyChanged(nameof(ItemSelecionado));
                OnPropertyChanged(nameof(ItemSelecionadoDescricao));
            }
        }

        public string ItemSelecionadoDescricao => _itemSelecionado?.Valor ?? "";

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void CalcularAlturaListBox()
        {
            double alturaDaLinha = 30; 
            double alturaMaxima = alturaDaLinha * 5; 

            AlturaListBox = (_itens.Count < 5) ? alturaDaLinha * _itens.Count : alturaMaxima;
        }
    }
}
