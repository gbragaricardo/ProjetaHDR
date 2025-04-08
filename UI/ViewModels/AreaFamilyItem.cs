using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ProjetaHDR.Commands;

namespace ProjetaHDR.UI.ViewModels
{
    internal class AreaFamilyItem : ObservableObject
    {
        Guid _intensityParamGuid = new Guid("1669925a-e4f7-4013-9f2f-9c16192fb53a");

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
                    FlowRate = 0;
                }
                else if (_instanceElementId != value)
                {
                    _instanceElementId = value;
                    OnPropertyChanged();

                    if (RainNetwork.HelperContext != null)
                    {
                        Description = RainNetwork.HelperContext.Doc
                            .GetElement(_instanceElementId)?
                            .get_Parameter(BuiltInParameter.ROOM_NAME)
                            .AsValueString();

                        Element areaElement = RainNetwork.HelperContext.Doc.GetElement(InstanceElementId);
                        if (areaElement == null)
                            return;

                        var areaValue = areaElement.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
                        areaValue = UnitUtils.ConvertFromInternalUnits(areaValue, UnitTypeId.SquareMeters);

                        var rainIntensity = areaElement.get_Parameter(_intensityParamGuid).AsDouble();

                        double areaFlowRate = ((rainIntensity * areaValue) / 60);

                        FlowRate = Math.Round(areaFlowRate, 2);

                        if (RainNetwork.ViewModel != null)
                        {
                            RainNetwork.ViewModel.AutoCalcFlowRate();
                        }

                    }
                    else
                    {
                        Description = null;
                        FlowRate = 0;
                    }
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

                if (_isSelected && RainNetwork.ViewModel != null)
                {
                    RainNetwork.ViewModel.SelectedAreaItem = this;
                }
            }
        }
    }
}
