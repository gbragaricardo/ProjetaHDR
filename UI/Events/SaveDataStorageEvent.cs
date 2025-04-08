using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitAddin.Commands.Services;
using ProjetaHDR.UI.ViewModels;

namespace ProjetaHDR.UI.Events
{
    internal class SaveDataStorageEvent : RevitCommandBase, IExternalEventHandler
    {
        Document _doc;
        ObservableCollection<FixtureFamilyItem> _mainFixtures;

        public SaveDataStorageEvent(Document doc, ObservableCollection<FixtureFamilyItem> mainFixtures)
        {
            _doc = doc;
            _mainFixtures = mainFixtures;
        }
        public void Execute(UIApplication app)
        {
            InitializeContextEvent(app);

            using (Transaction transaction = new Transaction(Context.Doc, "Trechos Salvos"))
            {

                transaction.Start();

                FixtureStorageManager.SaveDataToRevit(_doc, _mainFixtures);

                transaction.Commit();
            }
        }

        public string GetName() => "WindowDrenagem";
    }
}
