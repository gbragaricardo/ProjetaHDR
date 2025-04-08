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
    internal class ExecuteDrenEvent : RevitCommandBase, IExternalEventHandler
    {
        Document _doc;
        ObservableCollection<FixtureFamilyItem> _mainFixtures;

        public ExecuteDrenEvent(Document doc, ObservableCollection<FixtureFamilyItem> mainFixtures)
        {
            _doc = doc;
            _mainFixtures = mainFixtures;
        }
        public void Execute(UIApplication app)
        {
            InitializeContextEvent(app);

            Guid flowRateParamGuid = new Guid("ac19ab22-052c-47b3-8e14-76ecd81f5353");

            using (Transaction transaction = new Transaction(Context.Doc, "Executar Rede Pluvial"))
            {
                transaction.Start();
                foreach (var addedFix in _mainFixtures)
                {
                    if (addedFix.InstanceElementId == ElementId.InvalidElementId ||
                        addedFix.InstanceElementId == null)
                        continue; 

                    Element addedFixElement = Context.Doc.GetElement(addedFix.InstanceElementId);
                    addedFixElement.get_Parameter(flowRateParamGuid).Set(addedFix.FlowRate);

                    foreach (var pipe in addedFix.OutputPipes)
                        pipe.get_Parameter(flowRateParamGuid).Set(addedFix.FlowRate);
                }

                transaction.Commit();
            }
        }

        public string GetName() => "WindowDrenagem";
    }
}
