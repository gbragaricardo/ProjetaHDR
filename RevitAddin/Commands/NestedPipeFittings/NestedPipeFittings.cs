using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.RevitContext;
using ProjetaHDR.Utils;

namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class NestedPipeFittings : RevitCommandBase, IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            using (Transaction transacao = new Transaction(Context.Doc, "Nested Families"))
            {
                transacao.Start();

                // Obtém todas as conexões do projeto
                IList<Element> conexoes = PipeFittingUtils.SelecionarConexoes(Context.Doc);

                // Atualiza os parâmetros das conexões aninhadas
                int instanciasAlteradas = FamilyParameterUtils.InserirSistemaFamiliaAninhada(conexoes, "Abreviatura do sistema", "PRJ HDR: Sistema");

                
                TaskDialog.Show("Resultado", $"{instanciasAlteradas} famílias aninhadas modificadas.");

                transacao.Commit();

                return Result.Succeeded;
            }
        }
    }
}
