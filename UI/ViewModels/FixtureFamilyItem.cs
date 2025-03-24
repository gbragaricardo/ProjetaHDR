using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using ProjetaHDR.Commands;
using ProjetaHDR.UI;

namespace ProjetaHDR.UI.ViewModels
{
    internal class FixtureFamilyItem : ObservableObject
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
                        Description = Dev.HelperContext.Doc.GetElement(_instanceElementId)?.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString();

                    else
                        Description = null;
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
                    Dev.ViewModel.SelectedFixtureFamily = this;
                }
            }
        }

        private bool _InputFixtureSelected;

        public bool InputFixtureSelected
        {
            get => _InputFixtureSelected;
            set
            {
                _InputFixtureSelected = value;
                OnPropertyChanged();

                if (_InputFixtureSelected && Dev.ViewModel != null)
                {
                    Dev.ViewModel.SelectedInputFixture = this;
                }
            }
        }

        public ObservableCollection<AreaFamilyItem> InputAreas { get; set; } = new ObservableCollection<AreaFamilyItem>();
        public ObservableCollection<FixtureFamilyItem> InputFixtureItems{ get; set; } = new ObservableCollection<FixtureFamilyItem>();
        public ObservableCollection<Pipe> OutputPipes { get; set; } = new ObservableCollection<Pipe>();

    }
}
