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
    internal class InputFixtureItem : ObservableObject
    {
        public string Id { get; set; }

        private FixtureFamilyItem _correspondentFixture;
        public FixtureFamilyItem CorrespondentFixture
        {
            get => _correspondentFixture;
            set
            {
                if (_correspondentFixture != value)
                {
                    _correspondentFixture = value;
                    Id = _correspondentFixture.Id;
                    OnPropertyChanged();

                    if(Dev.ViewModel != null)
                    {
                        if (!Dev.ViewModel.AddedFixtureFamilies.Any(added => added.Id == _correspondentFixture.Id))
                            _correspondentFixture = null;
                        
                        Dev.ViewModel.AutoCalcFlowRate();
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

                if (_isSelected && Dev.ViewModel != null)
                {
                    Dev.ViewModel.SelectedInputFixture = this;
                }
            }
        }
    }
}
