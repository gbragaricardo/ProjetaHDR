using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ProjetaHDR.RevitAddin.Commands.Services;
using ProjetaHDR.UI.Services;
using Autodesk.Revit.DB.Plumbing;

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

        private FixtureFamilyItem _selectedInputFixture;
        public FixtureFamilyItem SelectedInputFixture
        {
            get => _selectedInputFixture;
            set
            {
                if (_selectedInputFixture != value)
                {
                    _selectedInputFixture = value;
                    OnPropertyChanged();
                }
            }
        }

        public IList<string> ClassifiedOutputPipes
        {
            get
            {
                if (SelectedFixtureFamily?.OutputPipes == null)
                    return new List<string>();

                return SelectedFixtureFamily.OutputPipes
                    .GroupBy(group => group.Name)
                    .Select(group =>
                    {
                        string name = group.Key.Replace("PVC", "").Trim();
                        string diameter = $"{group.First().Diameter * 304.8}";
                        return $"{group.Count()}x {name} - {diameter}mm";
                    })
                    .ToList();
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
        public RelayCommand AddInputFixtureCommand { get; }
        public RelayCommand RemoveInputFixtureCommand { get; }
        public RelayCommand SelectPipesCommand { get; }




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
            AddInputFixtureCommand = new RelayCommand(param => AddInputFixture(param));
            RemoveInputFixtureCommand = new RelayCommand(param => RemoveInputFixture(param));
            SelectPipesCommand = new RelayCommand(param => SelectOutputPipes(param));


            AddedFixtureFamilies = new ObservableCollection<FixtureFamilyItem>();

            LoadFixturesFromRevit();
            LoadFixtureList();

        }

        private void SelectOutputPipes(object param)
        {
            Selection selection = Context.UiDoc.Selection;
            ISelectionFilter selectFilter = new SelectionOptions(BuiltInCategory.OST_PipeCurves);
            IList<Reference> pipesReferences = new List<Reference>();

            try
            {
                pipesReferences = selection.PickObjects(ObjectType.Element, selectFilter);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            { }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (pipesReferences.Count == 0)
                return;

            SelectedFixtureFamily.OutputPipes?.Clear();

            foreach (var pipeRef  in pipesReferences)
            {
                Element pipeElement = Context.Doc.GetElement(pipeRef);
                Pipe pipe = pipeElement as Pipe;
                SelectedFixtureFamily.OutputPipes.Add(pipe);
            }
        }

        private void RemoveInputFixture(object param)
        {
            var selectedInputxFixture = SelectedFixtureFamily.InputFixtureItems.FirstOrDefault(fixture => fixture.InputFixtureSelected);
            var selectedInputFixtureIndex = SelectedFixtureFamily.InputFixtureItems.IndexOf(selectedInputxFixture);

            if (selectedInputxFixture == null)
                return;
            else
                SelectedFixtureFamily.InputFixtureItems.RemoveAt(selectedInputFixtureIndex);


            if (SelectedFixtureFamily.InputFixtureItems.Count == 0) return;


            if (selectedInputFixtureIndex == 0)
                SelectedFixtureFamily.InputFixtureItems.ElementAtOrDefault(0).InputFixtureSelected = true;

            else
                SelectedFixtureFamily.InputFixtureItems.ElementAtOrDefault(selectedInputFixtureIndex - 1).InputFixtureSelected = true;
        }

        private void AddInputFixture(object param)
        {
            if (SelectedFixtureFamily == null)
                return;

            if (param is ElementId areaId)
            {
                var selectedFixture = SelectedFixtureFamily.InputFixtureItems.FirstOrDefault(fixture => fixture.IsSelected);
                var selectedFixtureIndex = SelectedFixtureFamily.InputFixtureItems.IndexOf(selectedFixture);

                SelectedFixtureFamily.InputFixtureItems.Insert(selectedFixtureIndex + 1, new FixtureFamilyItem());
            }
            else
            {
                SelectedFixtureFamily.InputFixtureItems.Add(new FixtureFamilyItem());
            }
        }

        private void RemoveInputArea(object param)
        {
            var selectedArea = SelectedFixtureFamily.InputAreas.FirstOrDefault(area => area.IsSelected);
            var selectedAreaIndex = SelectedFixtureFamily.InputAreas.IndexOf(selectedArea);

            if (selectedArea == null)
                return;
            else
                SelectedFixtureFamily.InputAreas.RemoveAt(selectedAreaIndex);


            if (SelectedFixtureFamily.InputAreas.Count == 0) return;


            if (selectedAreaIndex == 0)
                SelectedFixtureFamily.InputAreas.ElementAtOrDefault(0).IsSelected = true;

            else
                SelectedFixtureFamily.InputAreas.ElementAtOrDefault(selectedAreaIndex - 1).IsSelected = true;

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
                    item.InstanceElementId = ElementId.InvalidElementId;

                foreach (var inputItem in item.InputFixtureItems)
                {
                    if (inputItem.InstanceElementId != null && !AddedFixtureFamilies.Any(addedFix => addedFix.InstanceElementId == inputItem.InstanceElementId))
                        inputItem.InstanceElementId = ElementId.InvalidElementId;
                }

                foreach (var inputArea in item.InputAreas)
                {
                    if (inputArea.InstanceElementId != null && !AllDocumentAreas.ContainsKey(inputArea.InstanceElementId))
                        inputArea.InstanceElementId = ElementId.InvalidElementId;
                }

                var toRemove = item.OutputPipes.Where(p => p == null || !p.IsValidObject).ToList();
                foreach (var pipe in toRemove)
                {
                    item.OutputPipes.Remove(pipe);
                }
            }
        }

        internal void LoadAndValidate()
        {
            LoadFixtureList();
            OnPropertyChanged(nameof(AllFixturesAndComments));
            OnPropertyChanged(nameof(AllDocumentAreas));


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
            var orderedElementsList = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(f => !String.IsNullOrEmpty(f.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()))
                .OrderBy(f => f.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString());

            Dictionary<ElementId, string> fixturesDictionary = orderedElementsList
                .ToDictionary(element => element.Id,
                              element => element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()
                              );

            return fixturesDictionary;
        }

        public Dictionary<ElementId, string> GetAllAreas()
        {
            var orderedAreasList = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(a => !String.IsNullOrEmpty(a.get_Parameter(BuiltInParameter.ROOM_NAME).AsValueString()))
                .OrderBy(a => a.get_Parameter(BuiltInParameter.ROOM_NAME).AsValueString());

            Dictionary<ElementId, string> areas = orderedAreasList
                .ToDictionary(element => element.Id,
                              element => element.get_Parameter(BuiltInParameter.ROOM_NAME).AsValueString()
                              );

            return areas;
        }
    }
}