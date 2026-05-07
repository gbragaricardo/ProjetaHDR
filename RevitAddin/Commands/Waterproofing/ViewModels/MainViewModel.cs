using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models;
using ProjetaHDR.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public ObservableCollection<WaterproofingType> AvailableTypes { get; set; } = new ObservableCollection<WaterproofingType>();

        private WaterproofingType _waterproofingType;
        public WaterproofingType WaterproofingType
        {
            get => _waterproofingType;
            set
            {
                if (_waterproofingType != value)
                {
                    _waterproofingType = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _baseboardHeigth = 0.0;
        public double BaseboardHeigth
        {
            get => _baseboardHeigth;
            set
            {
                if (_baseboardHeigth != value)
                {
                    _baseboardHeigth = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _offset = 0.0;
        public double Offset
        {
            get => _offset;
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            CancelCommand = new RelayCommand(Cancel);
            SelectRegionsCommand = new RelayCommand(SelectRegions);
        }

        public RelayCommand CancelCommand { get; }
        public RelayCommand SelectRegionsCommand { get; }

        private void SelectRegions(object parameter)
        {

        }
        private void Cancel(object parameter)
        {
            return;
        }

    }
}