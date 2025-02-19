using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using System.Runtime.Remoting.Contexts;
using ProjetaHDR.OnStartup;
using ProjetaHDR.Utils;
using Autodesk.Revit.DB.Plumbing;
using System.Xml.Linq;


namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Dev : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiApp = commandData.Application;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using (Transaction transacao = new Transaction(doc, "DEV"))
            {
                    transacao.Start();
            /*
                    // 🔹 Buscar a tabela de planejamento pelo nome
                    IList<ViewSchedule> Listschedule = new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSchedule))
                    .Cast<ViewSchedule>()
                    .ToList();               

                    foreach (ViewSchedule schedule in Listschedule)
                    {

                        List<Element> elementosTabela = ObterElementosDaTabela(doc, schedule);
                        if (elementosTabela == null || elementosTabela.Count == 0)
                        {
                        continue;
                        }
                        // 🔹 Seleciona os elementos na interface do Revit
                        View activeView = uidoc.ActiveView;

                        activeView.HideElements(elementosTabela.Select(x => x.Id).ToList());
                     }
                    // 🔹 Obtém todos os elementos listados na tabela
              */      
                    
                     transacao.Commit();

            return Result.Succeeded;
            }
        }

        private List<Element> ObterElementosDaTabela(Document doc, ViewSchedule schedule)
        {
            List<Element> elementos = new List<Element>();

            // 🔹 Obtém a categoria da tabela
            ElementId categoriaId = schedule.Definition.CategoryId;

            // 🔹 Coleta todos os elementos dessa categoria no modelo
            IList<Element> collector = new FilteredElementCollector(doc)
                .OfCategoryId(categoriaId)
                .WhereElementIsNotElementType()
                .ToElements();

            // 🔹 Obtém os dados da tabela
            TableData tableData = schedule.GetTableData();
            TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);

            for (int row = 0; row < sectionData.NumberOfRows; row++)
            {

                for (int col = 0; col < sectionData.NumberOfColumns; col++)
                {

                
                    // 🔹 Obtém um possível valor de ID ou outro parâmetro que relacione os elementos
                    string valorTabela = sectionData.GetCellText(row, col); // Coluna 0 pode ser o nome ou ID do elemento

                // 🔹 Verifica os elementos do modelo e adiciona à lista se corresponderem à tabela
                    foreach (Element elem in collector)
                    {
                        var parametroElemento = elem.LookupParameter("DarivaBIM: Descrição");
                        var parametroElemento2 = elem.LookupParameter("Descrição");
                        var parametroElemento3 = elem.LookupParameter("PRJ HDR: Descrição");
                        if (parametroElemento2 == null && parametroElemento == null && parametroElemento3 == null)
                        {
                            return null;
                        }
                        
                        else
                        {


                            var valorParametro = parametroElemento.AsString();
                            var valorParametro2 = parametroElemento.AsString();
                            var valorParametro3 = parametroElemento.AsString();

                            if ((valorParametro == valorTabela || valorParametro2 == valorTabela || valorParametro3 == valorTabela) || elem.UniqueId == valorTabela)
                            {
                                elementos.Add(elem);
                            }
                        }
                    }
                }
            }

            return elementos;
        }

    }
}