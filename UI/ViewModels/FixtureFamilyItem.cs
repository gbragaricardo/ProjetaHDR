using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using ProjetaHDR.Commands;
using ProjetaHDR.UI;

namespace ProjetaHDR.UI.ViewModels
{
    internal class FixtureFamilyItem : ObservableObject
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8);

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _comment;
        public string Comment
        {
            get => _comment;
            set
            {
                if (_comment != value)
                {
                    _comment = value;
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
                if (value == null) _instanceElementId = ElementId.InvalidElementId;

                if (_instanceElementId != value)
                {
                    _instanceElementId = value;
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
                    Dev.ViewModel.SelectedFixtureFamily = this;
                    OnPropertyChanged(nameof(InputAreasIds));
                }
            }
        }

        public ObservableCollection<ElementId> InputAreasIds { get; set; } = new ObservableCollection<ElementId>();
        internal ObservableCollection<ElementId> InputFixtureElementIds { get; set; }


    }
}
