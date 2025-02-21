using System.Linq;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using ProjetaHDR.RevitAddin.RevitContext;
using System.IO;
using System.Windows.Input;
using System;

namespace ProjetaHDR.UI.ViewModels
{

    internal class FamiliesViewModel : ObservableObject
    {
        private readonly string familiesDirectory = @"P:\QUALIDADE\11 - ARQUIVOS BASE DAS DISCIPLINAS\SETOR DE BIM\ARQ\002. Bibliotecas\004. Guarda-corpo\001. BALAÚSTRE";  // Caminho das famílias
        private readonly string thumbsDirectory; // Caminho das thumbnails


        private string _search;
        public string Search
        {
            get => _search;
            set
            {
                if (_search != value)
                {
                    _search = value;
                    OnPropertyChanged();
                    LoadFamilies(_search);
                }
            }
        }

        public ObservableCollection<FamilyItem> Families { get; set; }

        public ICommand RefreshCommand { get; }

        public FamiliesViewModel()
        {
            thumbsDirectory = Path.Combine(familiesDirectory, "Thumbs");
            Families = new ObservableCollection<FamilyItem>();
            RefreshCommand = new RelayCommand(_ => LoadFamilies());
            LoadFamilies();
        }

        private void LoadFamilies()
        {
            Families.Clear();

            if (!Directory.Exists(familiesDirectory))
                return;

            var files = Directory.GetFiles(familiesDirectory, "*.rfa");

            foreach (var file in files)
            {
                Families.Add(new FamilyItem(file, thumbsDirectory));
            }
        }
        private void LoadFamilies(string search)
        {
            Families.Clear();

            if (!Directory.Exists(familiesDirectory))
                return;

            var files = Directory.GetFiles(familiesDirectory, "*.rfa");

            // Filtra só os arquivos cujo caminho (ou nome) contenha o 'search'
            var filteredFiles = files.Where(file => file.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);

            foreach (var file in filteredFiles)
            {
                Families.Add(new FamilyItem(file, thumbsDirectory));
            }
        }

    }








    //internal class FamiliesViewModel : ObservableObject
    //{
    //    private string _familiesFolder = @"C:\Families"; // Defina o caminho das famílias

    //    public ObservableCollection<string> Families { get; set; }

    //    public ICommand RefreshCommand { get; }

    //    public FamiliesViewModel()
    //    {
    //        Families = new ObservableCollection<string>();
    //        RefreshCommand = new RelayCommand(LoadFamilies);
    //        LoadFamilies();
    //    }

    //    private void LoadFamilies(object parameter = null)
    //    {
    //        Families.Clear();
    //        if (Directory.Exists(_familiesFolder))
    //        {
    //            foreach (var file in Directory.GetFiles(_familiesFolder, "*.rfa"))
    //            {
    //                Families.Add(Path.GetFileName(file));
    //            }
    //        }
    //    }
    //}
}
