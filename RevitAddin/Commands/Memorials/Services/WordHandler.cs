using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;
using ProjetaHDR.Utils;
using WordInterop = Microsoft.Office.Interop.Word;

namespace ProjetaHDR
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
            if (textoNovo == "")
            {
                textoNovo = parametroReferencia;
            }

            foreach (WordInterop.Range range in _wordDoc.StoryRanges)
            {
                FindAndReplace(range, textoAntigo, textoNovo);
            }
        }

        public void ReplaceTextInFooter(string oldText, string newText)
        {
            var parametro = _infoParameters.LookupParameter(newText);

            
            var textoNovo = parametro?.AsString() ?? newText;
            if (textoNovo == "")
            {
                textoNovo = newText;
            }

            // Acessa o rodapé da seção principal
            WordInterop.HeaderFooter footer = _wordDoc.Sections[2].Footers[WordInterop.WdHeaderFooterIndex.wdHeaderFooterPrimary];

            WordInterop.Range footerRange = footer.Range;
            WordInterop.Find findObject = footerRange.Find;

            findObject.ClearFormatting();
            findObject.Text = oldText;
            findObject.MatchCase = false;
            findObject.MatchWholeWord = false;
            findObject.Wrap = WdFindWrap.wdFindStop;

            // Substitui a primeira ocorrência no rodapé
            if (findObject.Execute())
            {
                footerRange.Text = textoNovo;
                footerRange.HighlightColorIndex = WdColorIndex.wdBrightGreen; // Destacar em verde
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

        public void ReplaceImage(string imageAltText, string newImagePath)
        {
            foreach (WordInterop.Shape shape in _wordDoc.Shapes)
            {
                if (!string.IsNullOrEmpty(shape.AlternativeText) && shape.AlternativeText == imageAltText)
                {
                    // Obtém a posição e tamanho do shape original
                    float left = shape.Left;
                    float top = shape.Top;
                    float width = shape.Width;
                    float height = shape.Height;
                    Range anchor = shape.Anchor;
                    var lockAnchor = shape.LockAnchor;
                    var wrapFormat = shape.WrapFormat.Type;
                    
                    WdRelativeHorizontalPosition horizontalPos = shape.RelativeHorizontalPosition;
                    WdRelativeVerticalPosition verticalPos = shape.RelativeVerticalPosition;

                    // Remove o shape original
                    shape.Delete();

                    // Adiciona a nova imagem **respeitando a ancoragem**
                    WordInterop.Shape newShape = _wordDoc.Shapes.AddPicture(
                                                 newImagePath,
                                                 LinkToFile: MsoTriState.msoFalse,
                                                 SaveWithDocument: MsoTriState.msoTrue,
                                                 Anchor: anchor);

                    // Mantém a relação com o texto
                    newShape.RelativeHorizontalPosition = horizontalPos;
                    newShape.RelativeVerticalPosition = verticalPos;

                    // Mantém posição e tamanho
                    newShape.Left = left;
                    newShape.Top = top;


                    newShape.Width = width;
                    newShape.Height = height;

                    // Evita deslocamento automático do Word
                    newShape.LockAnchor = lockAnchor;
                    newShape.WrapFormat.Type = wrapFormat;

                    // Mantém a posição na ordem Z
                    newShape.ZOrder(MsoZOrderCmd.msoBringForward);
                    newShape.AlternativeText = imageAltText; // Mantém a identificação original

                    break; // Sai do loop após encontrar e substituir
                }
            }
        }

        public void ReplaceFooterImage(string imageAltText, string newImagePath)
        {
            WordInterop.HeaderFooter footer = _wordDoc.Sections[2].Footers[WordInterop.WdHeaderFooterIndex.wdHeaderFooterPrimary];

            foreach (WordInterop.Shape shape in footer.Range.ShapeRange)
            {
                if (!string.IsNullOrEmpty(shape.AlternativeText) && shape.AlternativeText == imageAltText)
                    {
                    // Obtém a posição e tamanho do shape original
                    float left = shape.Left;
                    float top = shape.Top;
                    float width = shape.Width;
                    float height = shape.Height;
                    Range anchor = shape.Anchor;
                    var lockAnchor = shape.LockAnchor;
                    var wrapFormat = shape.WrapFormat.Type;

                    WdRelativeHorizontalPosition horizontalPos = shape.RelativeHorizontalPosition;
                    WdRelativeVerticalPosition verticalPos = shape.RelativeVerticalPosition;

                    // Remove a imagem original
                    shape.Delete();

                    // Adiciona a nova imagem **dentro do rodapé**
                    WordInterop.Shape newShape = _wordDoc.Shapes.AddPicture(
                                                    newImagePath,
                                                    LinkToFile: MsoTriState.msoFalse,
                                                    SaveWithDocument: MsoTriState.msoTrue,
                                                    Anchor: anchor);

                    // Mantém a posição em relação à página
                    newShape.RelativeHorizontalPosition = horizontalPos;
                    newShape.RelativeVerticalPosition = verticalPos;

                    // Mantém posição e tamanho
                    newShape.Left = left;
                    newShape.Top = top;
                    newShape.Width = width;
                    newShape.Height = height;

                    // Evita deslocamento automático
                    newShape.LockAnchor = lockAnchor;
                    newShape.WrapFormat.Type = wrapFormat;

                    // Mantém a posição na ordem Z
                    newShape.ZOrder(MsoZOrderCmd.msoBringForward);
                    newShape.AlternativeText = imageAltText; // Mantém a identificação original

                    break; // Sai do loop após encontrar e substituir
                }
                
            }
        }

        public void ReplaceTableImage(string imageAltText, string newImagePath)
        {
            foreach (WordInterop.Shape shape in _wordDoc.Shapes)
            {
                if (!string.IsNullOrEmpty(shape.AlternativeText) && shape.AlternativeText == imageAltText)
                {
                    // Obtém a posição e tamanho do shape original
                    float width = shape.Width;
                    float height = shape.Height;
                    Range anchor = shape.Anchor;
                    var lockAnchor = shape.LockAnchor;
                    var wrapFormat = shape.WrapFormat.Type;
                    WdRelativeHorizontalPosition horizontalPos = shape.RelativeHorizontalPosition;
                    WdRelativeVerticalPosition verticalPos = shape.RelativeVerticalPosition;

                    // Remove o shape original
                    shape.Delete();

                    // Adiciona a nova imagem **respeitando a ancoragem**
                    WordInterop.Shape newShape = _wordDoc.Shapes.AddPicture(
                                                 newImagePath,
                                                 LinkToFile: MsoTriState.msoFalse,
                                                 SaveWithDocument: MsoTriState.msoTrue,
                                                 Anchor: anchor);

                    // Mantém a relação com a página
                    newShape.RelativeHorizontalPosition = WdRelativeHorizontalPosition.wdRelativeHorizontalPositionPage;
                    newShape.RelativeVerticalPosition = WdRelativeVerticalPosition.wdRelativeVerticalPositionPage;

                    // Mantém tamanho original
                    newShape.Width = width;
                    newShape.Height = height;

                    // Define posição exata conforme identificado manualmente
                    newShape.Left = (float)(10.01 * 28.3465); // Converte cm para pontos
                    newShape.Top = (float)(0.39 * 28.3465);   // Converte cm para pontos

                    // Evita deslocamento automático do Word
                    newShape.LockAnchor = lockAnchor;
                    newShape.WrapFormat.Type = WdWrapType.wdWrapNone;

                    // Mantém a posição na ordem Z
                    newShape.ZOrder(MsoZOrderCmd.msoBringForward);
                    newShape.AlternativeText = imageAltText; // Mantém o identificador original

                    break; // Sai do loop após encontrar e substituir
                }
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
                return BasicUtils.Captalize(newText); // Mantém o padrão (ou adicione outras regras)
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

        public void ExceptionClose()
        {
            if (_wordDoc != null)
            {
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