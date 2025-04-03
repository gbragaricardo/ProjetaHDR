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
        Guid _flowRateParamGuid = new Guid("ac19ab22-052c-47b3-8e14-76ecd81f5353");


        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();

                    CorrespondentFixture = Dev.ViewModel.AddedFixtureFamilies.FirstOrDefault(f => f.Id == Id);
                }
            }
        }

        private FixtureFamilyItem _correspondentFixture;
        public FixtureFamilyItem CorrespondentFixture
        {
            get => _correspondentFixture;
            set
            {
                if (_correspondentFixture != value)
                {
                    _correspondentFixture = value;
                    OnPropertyChanged();

                    FlowRate = CorrespondentFixture.FlowRate;
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


        //private void UpdateFlowRate()
        //{
        //    foreach (var area in InputAreas)
        //        FlowRate += area.FlowRate;

        //    foreach (var inputFixture in InputFixtureItems)
        //        FlowRate += inputFixture.FlowRate;

        //    FlowRate = Math.Round(FlowRate, 2);
        //}
    }
}
