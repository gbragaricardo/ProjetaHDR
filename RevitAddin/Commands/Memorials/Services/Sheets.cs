using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR
{
    internal class Sheets
    {
        public string Consorcio { get; private set; }

        public string GetTitleBlockName(Document doc)
        {
            // Filtrar todas as folhas (ViewSheet) no documento
            FilteredElementCollector sheetCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet));

            // Obter a primeira folha
            ViewSheet firstSheet = sheetCollector.Cast<ViewSheet>().FirstOrDefault();

            if (firstSheet == null)
            {
                return null;
            }

            // Filtrar os elementos do tipo FamilyInstance associados à folha
            FilteredElementCollector titleBlockCollector = new FilteredElementCollector(doc, firstSheet.Id)
                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance));

            // Obter o primeiro bloco de margem e carimbo
            FamilyInstance titleBlock = titleBlockCollector.Cast<FamilyInstance>().FirstOrDefault();

            if (titleBlock == null)
                return null;

            // Retornar o nome da família do bloco de margem e carimbo
            return titleBlock.Symbol.FamilyName.Replace("á","a").Replace("Á","a");
        }


        public string ValidateTitleBlock(string titleBlockName)
        {
            // Define as validações no dicionário
            var titleBlockMappings = new Dictionary<string, string>
            {
                { "Diamante", "Consórcio Diamante Engenharia" },
                { "Metaverso", "Metaverso Consórcios" },
                { "Minas", "Consórcio Minas Projetos" },
                { "Objetiva", "Objetiva Projetos e Serviços" },
                { "Pitagoras", "Consórcio Pitágoras" },
                { "Projeta", "Projeta Consultoria e Serviços" },
                { "Vitoria", "Vitória Consórcio"}
            };

            // Itera sobre as chaves do dicionário para verificar se estão contidas
            foreach (var mapping in titleBlockMappings)
            {
                if (titleBlockName.ToLower().Contains(mapping.Key.ToLower()))
                {
                    Consorcio = mapping.Key.ToLower();
                    return mapping.Value; // Retorna o valor correspondente à chave encontrada
                }
            }

            // Retorno padrão caso nenhuma correspondência seja encontrada
            return null;
        }

    }
}
