using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ProjetaHDR.RevitAddin.Commands.Services;

namespace ProjetaHDR.UI.ViewModels
{
    internal class DrenViewModel : ObservableObject
    {

        private FixtureFamilyItem _selectedFixtureFamily;
        public FixtureFamilyItem SelectedFixtureFamily
        {
            get => _selectedFixtureFamily;
            set
            {
                if (_selectedFixtureFamily != value)
                {
                    _selectedFixtureFamily = value;
                    OnPropertyChanged();
                }
            }
        }

        private AreaFamilyItem _selectedAreaItem;
        public AreaFamilyItem SelectedAreaItem
        {
            get => _selectedAreaItem;
            set
            {
                if (_selectedAreaItem != value)
                {
                    _selectedAreaItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public Dictionary<ElementId, string> AllFixturesAndComments { get; set; }
        public Dictionary<ElementId, string> AllDocumentAreas { get; set; }

        public ObservableCollection<FixtureFamilyItem> AddedFixtureFamilies { get; set; }
        public RevitContext Context { get; set; }
        public RelayCommand AddComboBoxCommand { get; }
        public RelayCommand MoveUpCommand { get; }
        public RelayCommand MoveDownCommand { get; }
        public RelayCommand RefreshAllFixturesCommand { get; }
        public RelayCommand RemoveComboBoxCommand { get;}
        public RelayCommand AddAreaCommand { get; }
        public RelayCommand RemoveAreaCommand { get; }

        public DrenViewModel(RevitContext context)
        {
            Context = context;
            AddComboBoxCommand = new RelayCommand(param => AddFixtureComboBox());
            RefreshAllFixturesCommand = new RelayCommand(param => LoadFixtureList());
            RemoveComboBoxCommand = new RelayCommand(param => RemoveFixtureComboBox());
            MoveUpCommand = new RelayCommand(param => MoveSelectedItem(- 1));
            MoveDownCommand = new RelayCommand(param => MoveSelectedItem(+ 1));
            AddAreaCommand = new RelayCommand(param => AddInputArea(param));
            RemoveAreaCommand = new RelayCommand(param => RemoveInputArea(param));

            AddedFixtureFamilies = new ObservableCollection<FixtureFamilyItem>();

            LoadFixturesFromRevit();
            LoadFixtureList();

        }

        private void RemoveInputArea(object param)
        {
            var selectedArea = SelectedFixtureFamily.InputAreas.FirstOrDefault(area => area.IsSelected);
            var selectedAreaIndex = SelectedFixtureFamily.InputAreas.IndexOf(selectedArea);

            if (selectedArea == null)
                return;
            else
                SelectedFixtureFamily.InputAreas.RemoveAt(selectedAreaIndex);

        }

        private void AddInputArea(object param)
        {
            if (SelectedFixtureFamily == null)
                return;

            if ( param is ElementId areaId)
            {
                var selectedArea = SelectedFixtureFamily.InputAreas.FirstOrDefault(area => area.IsSelected);
                var selectedAreaIndex = SelectedFixtureFamily.InputAreas.IndexOf(selectedArea);

                SelectedFixtureFamily.InputAreas.Insert(selectedAreaIndex + 1, new AreaFamilyItem());
            }
            else
            {
                SelectedFixtureFamily.InputAreas.Add(new AreaFamilyItem());
            }
        }

        private void MoveSelectedItem(int direction)
        {
            var selectedItem = AddedFixtureFamilies.FirstOrDefault(item => item.IsSelected);

            if (selectedItem == null)
                return;

            int index = AddedFixtureFamilies.IndexOf(selectedItem);
            int newIndex = index + direction;

            if (newIndex < 0 || newIndex >= AddedFixtureFamilies.Count)
                return;

            AddedFixtureFamilies.Move(index, newIndex);
        }

        private void AddFixtureComboBox()
        {
            var fixtureFamily = new FixtureFamilyItem();
            var selectedItem = AddedFixtureFamilies.FirstOrDefault(item => item.IsSelected);
            var selectedIndex = AddedFixtureFamilies.IndexOf(selectedItem);

            if (selectedItem == null)
                AddedFixtureFamilies.Add(fixtureFamily);
            else
                AddedFixtureFamilies.Insert(selectedIndex + 1, fixtureFamily);
        }

        private void RemoveFixtureComboBox()
        {
            var selectedItem = AddedFixtureFamilies.FirstOrDefault(item => item.IsSelected);
            var selectedIndex = AddedFixtureFamilies.IndexOf(selectedItem);

            if (selectedItem == null)
                return;
            else
                AddedFixtureFamilies.RemoveAt(selectedIndex);

            if (AddedFixtureFamilies.Count == 0) return;

            if (selectedIndex == 0)
                AddedFixtureFamilies.ElementAtOrDefault(0).IsSelected = true;

            else
                AddedFixtureFamilies.ElementAtOrDefault(selectedIndex - 1).IsSelected = true;


        }

        internal void LoadFixtureList()
        {
            AllFixturesAndComments?.Clear();
            AllFixturesAndComments = GetPlumbingFixtures();

            AllDocumentAreas?.Clear();
            AllDocumentAreas = GetAllAreas();
        }

        internal void ValidateFixtureItems()
        {
            foreach (var item in AddedFixtureFamilies)
            {
                if (item.InstanceElementId != null && !AllFixturesAndComments.ContainsKey(item.InstanceElementId))
                {
                    item.InstanceElementId = ElementId.InvalidElementId;
                }
            }
        }

        internal void LoadAndValidate()
        {
            LoadFixtureList();
            OnPropertyChanged(nameof(AllFixturesAndComments));

            ValidateFixtureItems();
            OnPropertyChanged(nameof(AddedFixtureFamilies));
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

        public Dictionary<ElementId, string> GetAllAreas()
        {
            Dictionary<ElementId, string> areas = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(f => !String.IsNullOrEmpty(f.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()))
                .ToDictionary(element => element.Id,
                              element => element.get_Parameter(BuiltInParameter.ROOM_NAME).AsValueString()
                              );

            return areas;
        }
    }
}