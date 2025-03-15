using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ProjetaHDR.Commands.Services;
using ProjetaHDR.RevitAddin.Commands.Services;

namespace ProjetaHDR.UI.ViewModels
{
    internal class DrenViewModel : ObservableObject
    {

        public Dictionary<ElementId, string> AllFixturesAndComments { get; set; }

        public ObservableCollection<FixtureFamilyItem> AddedFixtureFamilies { get; set; }
        public RevitContext Context { get; set; }
        public RelayCommand AddComboBoxCommand { get;}
        public RelayCommand RefreshAllFixturesCommand { get; }
        public RelayCommand RemoveComboBoxCommand { get;}
        public RelayCommand TestCommand { get; }


        public DrenViewModel(RevitContext context)
        {
            Context = context;
            AddComboBoxCommand = new RelayCommand(param => AddFixtureComboBox());
            RefreshAllFixturesCommand = new RelayCommand(param => LoadFixtureList());
            RemoveComboBoxCommand = new RelayCommand(param => RemoveFixtureComboBox());
            TestCommand = new RelayCommand(param => Test());

            AddedFixtureFamilies = new ObservableCollection<FixtureFamilyItem>();
            LoadFixturesFromRevit();
            LoadFixtureList();

        }

        private void AddFixtureComboBox()
        {
            var fixtureFamily = new FixtureFamilyItem();
            AddedFixtureFamilies.Add(fixtureFamily);
        }

        private void RemoveFixtureComboBox()
        {
            AddedFixtureFamilies.Remove(AddedFixtureFamilies[AddedFixtureFamilies.Count()-1]);
        }

        internal void LoadFixtureList()
        {
            AllFixturesAndComments?.Clear();

            AllFixturesAndComments = GetPlumbingFixtures();
        }

        public void ValidateFixtureItems()
        {
            foreach (var item in AddedFixtureFamilies)
            {
                if (item.InstanceElementId != null && !AllFixturesAndComments.ContainsKey(item.InstanceElementId))
                {
                    item.InstanceElementId = ElementId.InvalidElementId;
                }
            }
        }

        private void Test()
        {
            TaskDialog.Show("Teste", $"Nome: {AddedFixtureFamilies[0].InstanceElementId}\n{AddedFixtureFamilies[0].Comment}");
        }

        public void SaveDataStorage()
        {
            FixtureStorageManager.SaveDataToRevit(Context.Doc, AddedFixtureFamilies);
        }

        public void LoadFixturesFromRevit()
        {
            AddedFixtureFamilies.Clear();
            var loadedFixtures = FixtureStorageManager.LoadDataFromRevit(Context.Doc);

            foreach (var fixture in loadedFixtures)
                AddedFixtureFamilies.Add(fixture);
        }

        public Dictionary<ElementId, string> GetPlumbingFixtures()
        {
            Dictionary<ElementId, string> fixtures = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(f => !String.IsNullOrEmpty(f.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()))
                .ToDictionary(element => element.Id,
                              element => element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()
                              );

            return fixtures;
        }
    }
}