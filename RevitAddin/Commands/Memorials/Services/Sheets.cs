using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR
{
    internal static class Sheets
    {
        public static string GetTitleBlockName(Document doc)
        {
            // Filtrar todas as folhas (ViewSheet) no documento
            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet));

            // Obter a primeira folha
            ViewSheet firstSheet = sheetCollector.Cast<ViewSheet>().FirstOrDefault();

            if (firstSheet == null)
            {
                return "Nenhuma folha encontrada.";
            }

            // Filtrar os elementos do tipo FamilyInstance associados à folha
            FilteredElementCollector titleBlockCollector = new FilteredElementCollector(doc, firstSheet.Id)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance));

            // Obter o primeiro bloco de margem e carimbo
            FamilyInstance titleBlock = titleBlockCollector.Cast<FamilyInstance>().FirstOrDefault();

            if (titleBlock == null)
            {
                return "Nenhum bloco de margem e carimbo encontrado.";
            }

            // Retornar o nome da família do bloco de margem e carimbo
            return titleBlock.Symbol.FamilyName;
        }


        public static string ValidateTitleBlock(string titleBlockName)
        {
            // Define as validações no dicionário
            var titleBlockMappings = new Dictionary<string, string>
            {
                { "Diamante", "Consórcio Diamante Engenharia" },
                { "Objetiva", "Objetiva Projetos e Serviços" }
                //{ "Objetiva", "Objetiva Projetos e Serviços" }
                //{ "Objetiva", "Objetiva Projetos e Serviços" }
                //{ "Objetiva", "Objetiva Projetos e Serviços" }
            };

            // Itera sobre as chaves do dicionário para verificar se estão contidas
            foreach (var mapping in titleBlockMappings)
            {
                if (titleBlockName.ToLower().Contains(mapping.Key.ToLower()))
                {
                    return mapping.Value; // Retorna o valor correspondente à chave encontrada
                }
            }

            // Retorno padrão caso nenhuma correspondência seja encontrada
            return "Formato desconhecido";
        }

    }
}
