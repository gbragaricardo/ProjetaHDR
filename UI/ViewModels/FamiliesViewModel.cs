using System.Linq;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using ProjetaHDR.RevitAddin.RevitContext;
using System.IO;
using System.Windows.Input;

namespace ProjetaHDR.UI.ViewModels
{
    internal class FamiliesViewModel : ObservableObject
    {
        private string _familiesFolder = @"C:\Users\Usuario\Desktop\Families"; // Defina o caminho das famílias

        public ObservableCollection<string> Families { get; set; }

        public ICommand RefreshCommand { get; }

        public FamiliesViewModel()
        {
            Families = new ObservableCollection<string>();
            RefreshCommand = new RelayCommand(LoadFamilies);
            LoadFamilies();
        }

        private void LoadFamilies(object parameter = null)
        {
            Families.Clear();
            if (Directory.Exists(_familiesFolder))
            {
                foreach (var file in Directory.GetFiles(_familiesFolder, "*.rfa"))
                {
                    Families.Add(Path.GetFileName(file));
                }
            }
        }
    }
}
