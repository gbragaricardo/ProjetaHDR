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
                    return;
                }

                if (_instanceElementId != value)
                {
                    _instanceElementId = value;
                    OnPropertyChanged();
                    UpdateAreaFlow();

                    if (RainNetwork.ViewModel != null)
                        RainNetwork.ViewModel.AutoCalcFlowRate();
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

        public void UpdateAreaFlow()
        {
            if (RainNetwork.HelperContext != null && _instanceElementId != null)
            {

                Element areaElement = RainNetwork.HelperContext.Doc.GetElement(InstanceElementId);
                if (areaElement == null)
                {

                    Description = null;
                    FlowRate = 0;
                    return;
                }
                    

                Description = areaElement
                    .get_Parameter(BuiltInParameter.ROOM_NAME)
                    .AsValueString();

                var areaValue = areaElement.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble();
                areaValue = UnitUtils.ConvertFromInternalUnits(areaValue, UnitTypeId.SquareMeters);

                double a = areaElement.get_Parameter(new Guid("c9688612-5497-43b6-b36e-bf7559036bec")).AsDouble();
                double b = areaElement.get_Parameter(new Guid("80506c0c-c8cd-43bb-9895-70a89ea83433")).AsDouble();
                double c = areaElement.get_Parameter(new Guid("5cf801e8-6c6d-411b-a076-69d66f1b93fd")).AsDouble();
                double k = areaElement.get_Parameter(new Guid("c8022a50-c2ba-4d9c-982e-97cbfc536fb6")).AsDouble();
                double runoffCoef = areaElement.get_Parameter(new Guid("bee6c53d-9ca0-4a45-8fab-a710e1884587")).AsDouble();
                int tr = areaElement.get_Parameter(new Guid("fd39346e-f1b6-42bc-8996-ab8a606aa983")).AsInteger();

                double rainIntensity = (k * (Math.Pow(tr, a))) / (Math.Pow((5 + b), c));
                double areaFlowRate = ((rainIntensity * areaValue) / 60);

                if (runoffCoef > 0)
                    areaFlowRate *= runoffCoef;

                FlowRate = Math.Round(areaFlowRate, 2); return;
            }

            Description = null;
            FlowRate = 0;
            
        }
    }
}
