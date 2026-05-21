using ProjetaHDR.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels
{
    public class CreateWaterproofTypeViewModel : ObservableObject
    {
        private string _waterproofName;
        public string WaterproofName {
            get => _waterproofName;
            set 
            {
                if (_waterproofName != value) 
                {
                    _waterproofName = value;
                    OnPropertyChanged();
                }   
            }
        }

        private double _waterproofThickness;
        public double WaterproofThickness {
            get => _waterproofThickness;
            set 
            {
                if (_waterproofThickness != value) 
                {
                    _waterproofThickness = value;
                    OnPropertyChanged();
                }   
            }
        }

        private string _isHorizontalWaterproof;
        public string IsHorizontalWaterproof
        {
            get => _isHorizontalWaterproof;
            set 
            {
                if (_isHorizontalWaterproof != value) 
                {
                    _isHorizontalWaterproof = value;
                    OnPropertyChanged();
                }   
            }
        }

        private string _isVerticalWaterproof;
        public string IsVerticalWaterproof
        {
            get => _isVerticalWaterproof;
            set 
            {
                if (_isVerticalWaterproof != value) 
                {
                    _isVerticalWaterproof = value;
                    OnPropertyChanged();
                }   
            }
        }

        public RelayCommand CreateTypeCommand { get; set; }

        public CreateWaterproofTypeViewModel()
        {
            CreateTypeCommand = new RelayCommand(CreateType);
        }

        private void CreateType(object parameter)
        {
            
        }
    }
}
