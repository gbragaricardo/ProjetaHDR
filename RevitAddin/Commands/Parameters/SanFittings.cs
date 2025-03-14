﻿using System;
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
    internal class SanFittings : RevitCommandBase, IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            InitializeContext(commandData);

            using (Transaction transacao = new Transaction(Context.Doc, "Nested Families"))
            {
                transacao.Start();

                // Obtém todas as conexões do projeto
                IList<Element> conexoes = PipeUtils.GetAllPipeFittings(Context.Doc);

                // Atualiza os parâmetros das conexões aninhadas
                int instanciasAlteradas = NestedFittings.InserirSistemaFamiliaAninhada(conexoes, "Abreviatura do sistema", "PRJ HDR: Sistema");

                
                TaskDialog.Show("Resultado", $"{instanciasAlteradas} famílias aninhadas modificadas.");

                transacao.Commit();

                return Result.Succeeded;
            }
        }
    }
}
