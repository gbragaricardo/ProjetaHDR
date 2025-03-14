using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
using ProjetaHDR.UI.Services;
using System.Windows.Input;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using ProjetaHDR.Startup;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace ProjetaHDR.UI.ViewModels
{
    internal class DrenViewModel : ObservableObject
    {
        public Dictionary<Element, string> AllFixturesAndComments { get; set; }

        public ObservableCollection<FixtureFamilyItem> AddedFixtureFamilies { get; set; }
        public RevitContext Context { get; set; }
        public RelayCommand AddComboBoxCommand { get;}
        public RelayCommand RefreshAllFixturesCommand { get; }
        public RelayCommand RemoveComboBoxCommand { get;}
        public RelayCommand TestCommand { get; }


        public DrenViewModel(RevitContext context)
        {
            Context = context;
            AddedFixtureFamilies = new ObservableCollection<FixtureFamilyItem>();
            LoadFixtureList();
            AddComboBoxCommand = new RelayCommand(param => AddFixtureComboBox());
            RefreshAllFixturesCommand = new RelayCommand(param => LoadFixtureList());
            TestCommand = new RelayCommand(param => Test());
            RemoveComboBoxCommand = new RelayCommand(param => RemoveFixtureComboBox());


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

        private void LoadFixtureList()
        {
            AllFixturesAndComments?.Clear();

            AllFixturesAndComments = GetPlumbingFixtures();
        }

        private void Test()
        {
            TaskDialog.Show("Teste", $"Nome: {AddedFixtureFamilies[1].InstanceElement.Name}\n{AddedFixtureFamilies[1].DisplayName}");
        }

        public Dictionary<Element, string> GetPlumbingFixtures()
        {
            Dictionary<Element, string> fixtures = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(f => !String.IsNullOrEmpty(f.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()))
                .ToDictionary(element => element,
                              element => element.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString()
                              );

            return fixtures;
        }
    }
}