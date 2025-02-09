using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using ProjetaHDR.RevitContext;
using System.Runtime.Remoting.Contexts;
using ProjetaHDR.OnStartup;
using ProjetaHDR.Utils;
using Autodesk.Revit.DB.Plumbing;


namespace ProjetaHDR.Commands
{
    [Transaction(TransactionMode.Manual)]
    internal class Dev : RevitCommandBase, IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            View vistaAtiva = doc.ActiveView;

            using (Transaction trans = new Transaction(doc, "Testar Posição Relativa das Tags"))
            {
                trans.Start();

                // Obtém todos os tubos na vista ativa
                IList<Element> tubos = ObterTubosNaVista(doc, vistaAtiva);
                if (tubos.Count == 0)
                {
                    TaskDialog.Show("Erro", "Nenhum tubo encontrado na vista ativa.");
                    trans.RollBack();
                    return Result.Failed;
                }

                // Obtém o ID do tipo de tag (pode ser ajustado conforme necessário)
                ElementId tagId = ObterIdTag(doc);
                if (tagId == null)
                {
                    TaskDialog.Show("Erro", "Nenhuma tag de tubo encontrada no projeto.");
                    trans.RollBack();
                    return Result.Failed;
                }

                // Obtém os vetores da vista ativa (Right e Up)
                var (Right, Up) = ObterVetoresDaVista(vistaAtiva);

                double deslocamento = 0.2; // Define o deslocamento da tag (ajustável)

                // Percorre os tubos e cria as tags com deslocamento relativo à vista
                foreach (Element tubo in tubos)
                {
                    Pipe pipe = tubo as Pipe;
                    if (pipe == null) continue;

                    // Obtém o ponto médio do tubo
                    XYZ pontoMedio = ObterPontoMedioTubo(pipe);

                    // Aplica deslocamento na direção "Up" da vista
                    XYZ posicaoTag = pontoMedio + (Up * deslocamento);

                    // Cria a tag no ponto calculado
                    CriarTag(doc, vistaAtiva, tagId, pipe, posicaoTag);
                }

                trans.Commit();
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Obtém todos os tubos visíveis na vista ativa.
        /// </summary>
        private IList<Element> ObterTubosNaVista(Document doc, View vista)
        {
            return new FilteredElementCollector(doc, vista.Id)
                .OfCategory(BuiltInCategory.OST_PipeCurves)
                .WhereElementIsNotElementType()
                .ToElements();
        }

        /// <summary>
        /// Obtém um ID válido para o tipo de tag de tubos disponível no projeto.
        /// </summary>
        private ElementId ObterIdTag(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeTags)
                .WhereElementIsElementType()
                .ToElements()
                .FirstOrDefault(e => e.Name == "Diametro").Id;
        }

        /// <summary>
        /// Obtém os vetores da vista ativa (Right e Up) para calcular posições relativas.
        /// </summary>
        private (XYZ Right, XYZ Up) ObterVetoresDaVista(View vista)
        {
            if (vista is View3D view3D)
            {
                XYZ viewDirection = view3D.UpDirection.CrossProduct(view3D.RightDirection);
                return (view3D.RightDirection, view3D.UpDirection);
            }

            return (vista.RightDirection, vista.UpDirection);
        }

        /// <summary>
        /// Obtém o ponto médio do tubo.
        /// </summary>
        private XYZ ObterPontoMedioTubo(Pipe pipe)
        {
            LocationCurve locationCurve = pipe.Location as LocationCurve;
            return locationCurve?.Curve.Evaluate(0.5, true) ?? XYZ.Zero;
        }

        /// <summary>
        /// Cria uma tag independente para um tubo na posição especificada.
        /// </summary>
        private void CriarTag(Document doc, View vista, ElementId tagId, Pipe pipe, XYZ posicaoTag)
        {
            IndependentTag.Create(
                doc,
                tagId,
                vista.Id,
                new Reference(pipe),
                false,
                TagOrientation.Horizontal,
                posicaoTag
            );
        }



    }
}