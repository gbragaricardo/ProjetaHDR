using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.Utils;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Seduc : RevitCommandBase, IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            using (Transaction transacao = new Transaction(Context.Doc, "Nested Families"))
            {
                transacao.Start();

                int modifiedFamilies = ModifiedFamilies();

                if (modifiedFamilies > 0)
                    TaskDialog.Show("Resultado", $"Parametro \"Etapa Seduc\" ajustado em {modifiedFamilies} familias aninhadas.");

                else
                    TaskDialog.Show("Resultado", "Nenhuma familia aninhada a ser parametrizada");

                transacao.Commit();

                return Result.Succeeded;
            }
        }

        public int ModifiedFamilies()
        {
            int contador = 0;

            IList<Element> pipeFittings = PipeUtils.GetAllOfCategory(Context.Doc, BuiltInCategory.OST_PipeFitting);
            IList<Element> pipeCurves = PipeUtils.GetAllOfCategory(Context.Doc, BuiltInCategory.OST_PipeCurves);
            IList<Element> pipeAcessories = PipeUtils.GetAllOfCategory(Context.Doc, BuiltInCategory.OST_PipeAccessory);
            IList<Element> plumbingFixtures= PipeUtils.GetAllOfCategory(Context.Doc, BuiltInCategory.OST_PlumbingFixtures);

            contador = MatchNestedParams(pipeFittings, contador);
            contador = MatchNestedParams(pipeCurves, contador);
            contador = MatchNestedParams(pipeAcessories, contador);
            contador = MatchNestedParams(plumbingFixtures, contador);

            return contador;
        }

        public int MatchNestedParams(IList<Element> category, int counter) 
        {

            foreach (Element element in category)
            {
                if (element is FamilyInstance familyInstance)
                {
                    FamilyInstance familiaHospedeira = familyInstance.SuperComponent as FamilyInstance;
                    if (familiaHospedeira == null) continue;

                    Parameter parametroHospedeiro = familiaHospedeira.LookupParameter("Etapa Seduc");
                    if (parametroHospedeiro == null) continue;

                    string valorParametroHospedeiro = parametroHospedeiro.AsString();
                    if (string.IsNullOrEmpty(valorParametroHospedeiro)) continue;

                    Parameter parametroAninhado = familyInstance.LookupParameter("Etapa Seduc");
                    if (parametroAninhado != null && parametroAninhado.AsString() != valorParametroHospedeiro)
                    {
                        parametroAninhado.Set(valorParametroHospedeiro);
                        counter++;
                    }
                }
            }

            return counter;
        }
    }
}
