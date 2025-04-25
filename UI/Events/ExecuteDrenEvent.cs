using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
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

            Guid flowRateParamGuid = new Guid("6089145a-3038-442a-9b58-506cb1f82e0f");
            Guid conductorFlowParamGuid = new Guid("187e0ae1-747d-46c9-9e32-bae0b061bf26");
            Guid sectionConductorsParamGuid = new Guid("71208c40-1aea-4688-9d0d-f1a893d2fc17");

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

                    string sectionConductors = addedFix.OutputPipes
                        .GroupBy(group => group.Name)
                        .Select(group =>
                        {
                            string name = group.Key.Replace("PVC", "").Trim();
                            string diameter = $"{group.First().Diameter * 304.8}";
                            return $"{group.Count()}x {diameter}mm";
                        })
                        .FirstOrDefault();

                    

                    foreach (var pipe in addedFix.OutputPipes)
                    {
                        pipe.get_Parameter(flowRateParamGuid).Set(addedFix.FlowRate);
                        pipe.get_Parameter(sectionConductorsParamGuid).Set(sectionConductors);

                        if (addedFix.OutputPipes.Count() > 0 && addedFix.FlowRate > 0)
                        {
                            double conductorFlow = addedFix.FlowRate / addedFix.OutputPipes.Count();
                            pipe.get_Parameter(conductorFlowParamGuid).Set(conductorFlow);
                        }
                    }

                }

                MessageBox.Show("Execução Completa!", "Retorno");

                transaction.Commit();
            }
        }

        public string GetName() => "WindowDrenagem";
    }
}
