using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models;
using ProjetaHDR.RevitAddin.Commands.Waterproofing.Services;
using ProjetaHDR.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly Document _doc;
        private readonly UIDocument _uiDoc;
        public ObservableCollection<WaterproofingType> AvailableTypes { get; set; } = new ObservableCollection<WaterproofingType>
        {
            new WaterproofingType{Name= "Manta Asfaltica",},
            new WaterproofingType{Name= "Outro Tipo", },
            new WaterproofingType{Name= "Tipo 3 Asfaltica"}
        };

        public bool IsConfirmed { get; set; }

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

    }
}