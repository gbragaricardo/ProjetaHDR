using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Word;
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
        public void DeleteTags(string nomeTag)
        {
            foreach (WordInterop.Range delrange in _wordDoc.StoryRanges)
            {
                WordInterop.Find findObject = delrange.Find;
                findObject.MatchCase = false;

                findObject.Text = $"{nomeTag}Inicio";

                findObject.Replacement.Text = ""; // Substitui por vazio

                object replaceAll = WordInterop.WdReplace.wdReplaceAll;
                findObject.Execute(Replace: ref replaceAll);

            }

            foreach (WordInterop.Range delrange in _wordDoc.StoryRanges)
            {
                WordInterop.Find findObject = delrange.Find;
                findObject.MatchCase = false;

                findObject.Text = $"{nomeTag}Final";

                findObject.Replacement.Text = ""; // Substitui por vazio

                object replaceAll = WordInterop.WdReplace.wdReplaceAll;
                findObject.Execute(Replace: ref replaceAll);

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
                string foundText = range.Text;

                string adjustedNewText = AdjustNewTextCase(foundText, newText);

                range.Text = adjustedNewText;

                range.HighlightColorIndex = WordInterop.WdColorIndex.wdBrightGreen;

                range.Collapse(WordInterop.WdCollapseDirection.wdCollapseEnd);
            }
        }
        public void DeleteSpecificParagraph(string nomeTag)
        {
            foreach (WordInterop.Range delrange in _wordDoc.StoryRanges)
            {
                WordInterop.Find findObject = delrange.Find;
                findObject.ClearFormatting();
                findObject.MatchCase = false; // Ignora maiúsculas/minúsculas
                findObject.MatchWholeWord = false;
                findObject.Wrap = WordInterop.WdFindWrap.wdFindStop;
                findObject.MatchWildcards = true; // Ativa wildcards

                findObject.Text = $"{nomeTag}Inicio*{nomeTag}Final";

                findObject.Replacement.Text = ""; // Substitui por vazio


                object replaceAll = WordInterop.WdReplace.wdReplaceAll;
                findObject.Execute(Replace: ref replaceAll);

            }
        }

        private string AdjustNewTextCase(string foundText, string newText)
        {
            if (string.IsNullOrEmpty(foundText)) return newText;

            if (foundText == foundText.ToUpper())
            {
                return newText.ToUpper(); // Se o texto antigo era UPPER, novo texto também será
            }
            else if (foundText == foundText.ToLower())
            {
                return newText.ToLower(); // Se o texto antigo era lower, novo texto também será
            }
            else
            {
                return MainUtils.Captalize(newText); // Mantém o padrão (ou adicione outras regras)
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