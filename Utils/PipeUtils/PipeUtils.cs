using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Utils
{
    internal static class PipeUtils
    {
        
        public static void setPipeSlope(IList<Element> pipesList)
        {
            foreach (Element tubo in pipesList)
            {
                // Obtém os parâmetros necessários
                Parameter parametroInclinacao = tubo.LookupParameter("PRJ HDR: Inclinacao Tag");
                Parameter parametroAbreviaturaSistema = tubo.get_Parameter(BuiltInParameter.RBS_DUCT_PIPE_SYSTEM_ABBREVIATION_PARAM);
                Parameter parametroClassificacaoSistema = tubo.get_Parameter(BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM);
                Parameter parametroDiametro = tubo.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

                string abreviaturaSistema = parametroAbreviaturaSistema.AsString();
                string classificacaoSistema = parametroClassificacaoSistema.AsString();
                double diametroEmMilimetros = parametroDiametro.AsDouble() * 304.8;

                if (abreviaturaSistema == "ESG")
                {
                    if (classificacaoSistema == "Sanitário")
                    {

                        if (diametroEmMilimetros <= 75)
                        {
                            parametroInclinacao.Set("2%");

                        }
                        else
                        {
                            parametroInclinacao.Set("1%");
                        }
                    }

                    else
                    {
                        parametroInclinacao.Set("1%");
                    }
                }
            }
        }





    }
}
