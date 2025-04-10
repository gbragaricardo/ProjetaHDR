using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using ProjetaHDR.Commands;
using ProjetaHDR.UI;

namespace ProjetaHDR.UI.ViewModels
{
    internal class FixtureFamilyItem : ObservableObject
    {
        Guid _flowRateParamGuid = new Guid("ac19ab22-052c-47b3-8e14-76ecd81f5353");

        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        private bool _isValid = true;
        public bool IsValid 
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged();
                }
            }
        }

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

                    FlowRate = 0;

                    if (RainNetwork.HelperContext != null)
                    {
                        Description = RainNetwork.HelperContext.Doc
                            .GetElement(_instanceElementId)?
                            .get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
                            .AsValueString();

                        UpdateFlowRate();
                    }
                    else
                    {
                        Description = null;
                        FlowRate = 0;
                    }
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
                    RainNetwork.ViewModel.SelectedFixtureFamily = this;
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
                    UpdateFlowPerConductor();
                }
            }
        }

        private double _flowPerConductor;
        public double FlowPerConductor
        {
            get => _flowPerConductor;
            set
            {
                if (_flowPerConductor != value)
                {
                    _flowPerConductor = value;
                    OnPropertyChanged();
                    UpdateFlowPerConductor();
                }
            }
        }

        public ObservableCollection<AreaFamilyItem> InputAreas { get; set; } = new ObservableCollection<AreaFamilyItem>();
        public ObservableCollection<InputFixtureItem> InputFixtureItems{ get; set; } = new ObservableCollection<InputFixtureItem>();
        public ObservableCollection<Pipe> OutputPipes { get; set; } = new ObservableCollection<Pipe>();


        internal void UpdateFlowRate()
        {
            FlowRate = 0;

            foreach (var area in InputAreas)
            {
                area.UpdateAreaFlow();
                FlowRate += area.FlowRate;
            }
                
            foreach (var inputFixture in InputFixtureItems)
            {
                var correspodentFixture = inputFixture.CorrespondentFixture ?? null;

                if (correspodentFixture != null && correspodentFixture.IsValid == false)
                    inputFixture.CorrespondentFixture.FlowRate = 0;

                if (inputFixture.CorrespondentFixture != null)
                    FlowRate += inputFixture.CorrespondentFixture.FlowRate;
            }
            
            FlowRate = Math.Round(FlowRate, 2);
        }

        internal void UpdateFlowPerConductor()
        {
            if (OutputPipes.Count != 0)
                FlowPerConductor = Math.Round((FlowRate / OutputPipes.Count), 2);

            else
                FlowPerConductor = 0;
        }
    }
}
