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
            double alturaDaLinha = 24; 
            double alturaMaxima = alturaDaLinha * 10; 

            AlturaListBox = (_itens.Count < 10) ? alturaDaLinha * _itens.Count : alturaMaxima;
        }

        public bool SelecionarItemPorID(string id)
        {
            bool selecionouItem = false;

            if (_itens != null && _itens.Any())
            {
                var item = _itens.FirstOrDefault(i => i.ID == id); 
                ItemSelecionado = item ?? null;

                selecionouItem = (ItemSelecionado != null);
            }
            else
            {
                ItemSelecionado = null; 
            }

            return selecionouItem;
        }

        public bool SelecionarItemPorID(int id)
        {
            return SelecionarItemPorID(id.ToString());
        }

        public bool SelecionarItemPorIndice(int indice)
        {
            if (_itens == null || !_itens.Any())
            {
                ItemSelecionado = null;
                return false;
            }

            if (indice < 0)
                indice = 0;
            else if (indice >= _itens.Count)
                indice = _itens.Count - 1;

            ItemSelecionado = _itens[indice]; 
            return true;
        }

    }
}
