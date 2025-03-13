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
        private string _username = "Hidro";
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        IList<Element> AllDocumentFixtures { get; set; }
        ObservableCollection<FixtureFamilyItem> FixtureFamilies { get; set; }
        public RevitContext Context { get; set; }
        public RelayCommand AddComboBoxCommand { get;}
        public RelayCommand RemoveComboBoxCommand { get;}
        private int _generateId = 1;

        public DrenViewModel(RevitContext context)
        {
            Context = context;
            FixtureFamilies = new ObservableCollection<FixtureFamilyItem>();
            LoadFixtureList();

        }

        private void LoadFixtureList()
        {
            AllDocumentFixtures = GetPlumbingFixtures();

            foreach (var fixture in AllDocumentFixtures)
            {
                if (FixtureFamilies.Any(f => f.Id == fixture.Id))
                    continue;

                var fixtureFamilyItem = new FixtureFamilyItem
                {
                    Name = fixture.Name,
                    Id = fixture.Id,
                    InstanceElement = fixture
                };

                FixtureFamilies.Add(fixtureFamilyItem);
            }

        }

        public IList<Element> GetPlumbingFixtures()
        {
             IList<Element> fixtures = new FilteredElementCollector(Context.Doc)
                .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
                .WhereElementIsNotElementType()
                .ToElements()
                .Where(family => family.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS).AsValueString() != null)
                .ToList();

            return fixtures;
        }


        //private int _selectedIndex;
        //public int SelectedIndex
        //{
        //    get => _selectedIndex;
        //    set
        //    {
        //        if (_selectedIndex != value)
        //        {
        //            _selectedIndex = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
        //public ObservableCollection<string> QuantidadePecas { get; set; }
        //public IList<Element> ListaPecasProjeto { get; set; }
        //public IList<string> NomesPecas { get; set; }
        //public RevitContext Context { get; set; }

        //public RelayCommand AddComboBoxCommand { get; }
        //public RelayCommand RemoveComboBoxCommand { get; }


        //public DrenViewModel(RevitContext context)
        //{
        //    Context = context;
        //    NomesPecas = GetAllFixtures();
        //    QuantidadePecas = new ObservableCollection<string>();
        //    AddComboBoxCommand = new RelayCommand( x => AddComboBox());
        //    QuantidadePecas.Add("Default");
        //    QuantidadePecas.IndexOf();

        //}


        //private void AddComboBox()
        //{
        //    QuantidadePecas.Add(""); // Adiciona um novo item, criando outro ComboBox
        //}

        //internal IList<string> GetAllFixtures()
        //{
        //    IList<Element> elements = new FilteredElementCollector(Context.Doc)
        //        .OfCategory(BuiltInCategory.OST_PlumbingFixtures)
        //        .WhereElementIsNotElementType()
        //        .ToElements();

        //    ListaPecasProjeto = elements;

        //    return elements
        //        .Select(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS)
        //        .AsValueString())
        //        .Where(value => value != null)
        //        .ToList();
        //}
    }
}