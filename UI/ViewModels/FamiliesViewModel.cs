using System.Linq;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using ProjetaHDR.RevitAddin.RevitContext;
using System.IO;
using System.Windows.Input;
using System;
using System.Reflection;
using ProjetaHDR.UI.Events;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;

namespace ProjetaHDR.UI.ViewModels
{

    internal class FamiliesViewModel : ObservableObject
    {
        //private readonly string familiesDirectory = @"P:\QUALIDADE\11 - ARQUIVOS BASE DAS DISCIPLINAS\SETOR DE BIM\ARQ\002. Bibliotecas\004. Guarda-corpo\001. BALAÚSTRE";  // Caminho das famílias
        private readonly string familiesDirectory = @"C:\Families";
        private readonly string thumbsDirectory; // Caminho das thumbnails
        private readonly DownloadFamilyEvent _downloadHandler;
        private readonly ExternalEvent _externalEvent;


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

        public RelayCommand RefreshCommand { get; }
        public RelayCommand DownloadCommand { get; }
        public BitmapImage ImagePath { get; set; }



        public FamiliesViewModel()
        {
            _downloadHandler = new DownloadFamilyEvent();
            _externalEvent = ExternalEvent.Create(_downloadHandler);

            string imagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UIResources", "download.png");
            if (File.Exists(imagePath))
            {
                ImagePath = new BitmapImage(new Uri(imagePath));
            }

            thumbsDirectory = Path.Combine(familiesDirectory, "Thumbs");
            Families = new ObservableCollection<FamilyItem>();
            RefreshCommand = new RelayCommand(_ => LoadFamilies());
            DownloadCommand = new RelayCommand(param => DownloadFamily(param));
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



            string imagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UIResources", "download.png");
            if (File.Exists(imagePath))
            {
                ImagePath = new BitmapImage(new Uri(imagePath));
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

        private void DownloadFamily(object parameter)
        {            
            FamilyItem selectedFamily = parameter as FamilyItem;
            string familyPath = selectedFamily.FilePath;

            _downloadHandler.SetFamilyPath(selectedFamily.Name ,familyPath);
            _externalEvent.Raise();
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
