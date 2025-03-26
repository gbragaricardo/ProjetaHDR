using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ProjetaHDR.Commands;

namespace ProjetaHDR.UI.ViewModels
{
    internal class AreaFamilyItem : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }


        private ElementId _instanceElementId;
        public ElementId InstanceElementId
        {
            get => _instanceElementId;
            set
            {
                if (value == null)
                {
                    _instanceElementId = ElementId.InvalidElementId;
                    OnPropertyChanged();

                    Description = null;
                }
                else if (_instanceElementId != value)
                {
                    _instanceElementId = value;
                    OnPropertyChanged();

                    if (Dev.HelperContext != null)
                        Description = Dev.HelperContext.Doc.GetElement(_instanceElementId)?.get_Parameter(BuiltInParameter.ROOM_NAME).AsValueString();

                    else
                        Description = null;
                }
            }
        }

        private double _flowRate;
        public double FlowRate
        {
            get => _flowRate;
            set
            {
                if (_flowRate != value)
                {
                    _flowRate = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();

                if (_isSelected && Dev.ViewModel != null)
                {
                    Dev.ViewModel.SelectedAreaItem = this;
                }
            }
        }
    }
}
