using ProjetaHDR.RevitAddin.Commands.Waterproofing.Models;
using ProjetaHDR.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.RevitAddin.Commands.Waterproofing.ViewModels.Wrappers
{
    public class WaterproofingLayerItemViewModel : ObservableObject
    {

        private string _headerName;
        public string HeaderName
        {
            get => _headerName;
            set
            {
                if (_headerName != value)
                {
                    _headerName = value.ToUpper();
                    OnPropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    HeaderName = value.ToUpper();
                    OnPropertyChanged();
                }
            }
        }

        private double _thickness;
        public double Thickness
        {
            get => _thickness;
            set
            {
                if (_thickness != value)
                {
                    _thickness = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasHorizontalArea;
        public bool HasHorizontalArea
        {
            get => _hasHorizontalArea;
            set
            {
                if (_hasHorizontalArea != value)
                {
                    _hasHorizontalArea = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _hasVerticalArea;
        public bool HasVerticalArea
        {
            get => _hasVerticalArea;
            set
            {
                if (_hasVerticalArea != value)
                {
                    _hasVerticalArea = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
