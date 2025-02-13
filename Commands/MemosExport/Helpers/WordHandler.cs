using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ProjetaHDR.Utils;
using WordInterop = Microsoft.Office.Interop.Word;

namespace ProjetaHDR.Commands.Helpers
{
    internal class WordHandler : IDisposable
    {
        private readonly ProjectInfo _infoParameters;
        private readonly string _exportPath;
        private WordInterop.Application _wordApp;
        private WordInterop.Document _wordDoc;

        public WordHandler(ProjectInfo projectInfo, string exportPath)
        {
            _infoParameters = projectInfo;
            _exportPath = exportPath;
        }

        public void OpenWordDocument()
        {
            try
            {
                _wordApp = new WordInterop.Application();
                _wordApp.Visible = false;

                if (!File.Exists(_exportPath))
                {
                    throw new FileNotFoundException("Arquivo Word não encontrado: " + _exportPath);
                }

                _wordDoc = _wordApp.Documents.Open(_exportPath);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Erro", "Falha ao abrir o documento Word: " + ex.Message);
            }
        }

        public void ReplaceText(string textoAntigo, string parametroReferencia)
        {
            if (_wordDoc == null)
            {
                throw new InvalidOperationException("O documento Word não foi aberto corretamente.");
            }

            var parametro = _infoParameters.LookupParameter(parametroReferencia);
            var textoNovo = parametro?.AsString() ?? parametroReferencia;

            foreach (WordInterop.Range range in _wordDoc.StoryRanges)
            {
                FindAndReplace(range, textoAntigo, textoNovo);
            }
        }

        private void FindAndReplace(WordInterop.Range range, string oldText, string newText)
        {
            WordInterop.Find findObject = range.Find;
            findObject.ClearFormatting();
            findObject.Text = oldText;
            findObject.MatchCase = false; // Ignora case durante a busca
            findObject.MatchWholeWord = false;
            findObject.Wrap = WordInterop.WdFindWrap.wdFindStop;

            // Loop para encontrar todas as ocorrências
            while (findObject.Execute())
            {
                // Captura o texto encontrado (com seu case original)
                string foundText = range.Text;

                // Ajusta o "newText" com base no case do "foundText"
                string adjustedNewText = AdjustNewTextCase(foundText, newText);

                // Substitui manualmente o texto antigo pelo novo (com case ajustado)
                range.Text = adjustedNewText;

                // Aplica o destaque
                range.HighlightColorIndex = WordInterop.WdColorIndex.wdBrightGreen;

                // Move o range para continuar a busca
                range.Collapse(WordInterop.WdCollapseDirection.wdCollapseEnd);
            }
        }

        // Método para ajustar o case do novo texto com base no texto encontrado
        private string AdjustNewTextCase(string foundText, string newText)
        {
            if (string.IsNullOrEmpty(foundText)) return newText;

            // Exemplo de lógica para ajustar o case:
            if (foundText.All(char.IsUpper))
            {
                return newText.ToUpper(); // Se o texto antigo era UPPER, novo texto também será
            }
            else if (foundText.All(char.IsLower))
            {
                return newText.ToLower(); // Se o texto antigo era lower, novo texto também será
            }
            else
            {
                return MainUtils.Captalize(newText); // Mantém o padrão (ou adicione outras regras)
            }
        }

        public void DeleteSpecificParagraph()
        {
            foreach (WordInterop.Range delrange in _wordDoc.StoryRanges)
            {
                WordInterop.Find findObject = delrange.Find;
                findObject.ClearFormatting();
                findObject.MatchCase = false; // Ignora maiúsculas/minúsculas
                findObject.MatchWholeWord = false;
                findObject.Wrap = WordInterop.WdFindWrap.wdFindStop;
                findObject.MatchWildcards = true; // Ativa wildcards

                // Padrão ajustado para incluir possíveis espaços, quebras de linha ou traços
                findObject.Text = "CSharpApagarReformaInicio*CSharpApagarReformaFinal"; // ^13 representa quebras de parágrafo
                findObject.Replacement.Text = " "; // Substitui por vazio

                object replaceAll = WordInterop.WdReplace.wdReplaceOne;
                findObject.Execute(Replace: ref replaceAll);

            }
        }


        public void SaveAndClose()
        {
            if (_wordDoc != null)
            {
                _wordDoc.Save();
                _wordDoc.Close();
                _wordApp.Quit();

                Marshal.ReleaseComObject(_wordDoc);
                Marshal.ReleaseComObject(_wordApp);
            }
        }

        public void Dispose()
        {

        }
    }
}