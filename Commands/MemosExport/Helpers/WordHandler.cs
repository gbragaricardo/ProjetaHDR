using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjetaHDR.Utils;

namespace ProjetaHDR.Commands.Helpers
{
    internal class WordHandler
    {
        public ProjectInfo InfoParameters { get; set; }
        public string ExportPath { get; set; }

        public WordHandler(ProjectInfo projectInfo, string exportPath)
        {
            InfoParameters = projectInfo;
            ExportPath = exportPath;
        }


        public void ReplaceText(string textoAntigo, string parametroReferencia)
        {
            using (var wordDocument = WordprocessingDocument.Open(ExportPath, true))
            {
                var body = wordDocument.MainDocumentPart.Document.Body;

                var parametro = InfoParameters.LookupParameter(parametroReferencia);
                var textoNovo = parametro?.AsString();

                if (string.IsNullOrEmpty(textoNovo))
                {
                    textoNovo = parametroReferencia;
                }

                // Substituir no corpo do documento
                ReplaceInElements(body.Descendants<Run>(), textoAntigo, textoNovo);

                // Substituir nos cabeçalhos
                foreach (var headerPart in wordDocument.MainDocumentPart.HeaderParts)
                {
                    ReplaceInElements(headerPart.RootElement.Descendants<Run>(), textoAntigo, textoNovo);
                }

                // Substituir nos rodapés
                foreach (var footerPart in wordDocument.MainDocumentPart.FooterParts)
                {
                    ReplaceInElements(footerPart.RootElement.Descendants<Run>(), textoAntigo, textoNovo);
                }

                // Salvar o documento após as alterações
                wordDocument.MainDocumentPart.Document.Save();
            }
        }



        internal void ReplaceInElements(IEnumerable<Run> runElements, string textoAntigo, string textoNovo)
        {
            foreach (var runElement in runElements)
            {
                var runTextElements = runElement.Elements<Text>().ToList();

                for (int i = 0; i < runTextElements.Count; i++)
                {
                    var textElement = runTextElements[i];
                    string searchText = textElement.Text;
                    string textLower = textElement.Text.ToLower();

                    // Verificar se o texto contém o valor a ser substituído
                    if (textLower.Contains(textoAntigo.ToLower()))
                    {
                        // Ajustar o formato do texto novo para corresponder ao formato do texto antigo
                        if (searchText == searchText.ToUpper())
                        {
                            textoNovo = textoNovo.ToUpper();
                        }
                        else if (searchText == searchText.ToLower())
                        {
                            textoNovo = textoNovo.ToLower();
                        }
                        else
                        {
                            textoNovo = MainUtils.Captalize(textoNovo);
                        }

                        // Substituir o texto no nó atual
                        textElement.Text = Regex.Replace(textElement.Text, Regex.Escape(textoAntigo),
                            textoNovo,
                            RegexOptions.IgnoreCase);

                        // Aplicar o destaque verde ao texto substituído
                        var runProperties = runElement.GetFirstChild<RunProperties>();
                        if (runProperties == null)
                        {
                            runProperties = new RunProperties();
                            runElement.PrependChild(runProperties);
                        }

                        runProperties.Highlight = new Highlight { Val = HighlightColorValues.Green };
                    }
                }
            }
        }


    }
}
