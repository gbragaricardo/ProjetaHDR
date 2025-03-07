using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Word;

namespace ProjetaHDR.UI.Services
{
    internal class SaveImageNamesToFile
    {
        public void ExecuteTESTE(string wordFilePath, string outputTxtPath)
        {
            // Verifica se o arquivo existe
            if (!File.Exists(wordFilePath))
            {
                throw new FileNotFoundException("O arquivo Word não foi encontrado.", wordFilePath);
            }

            // Inicia a aplicação do Word
            Application wordApp = new Application();
            Document doc = null;

            try
            {
                wordApp.Visible = false; // Mantém a execução em segundo plano
                doc = wordApp.Documents.Open(wordFilePath);

                List<string> imageNames = new List<string>();

                // Percorre todas as imagens do documento
                foreach (InlineShape inlineShape in doc.InlineShapes)
                {
                    imageNames.Add(string.IsNullOrWhiteSpace(inlineShape.AlternativeText) ? "Imagem sem nome - Inline" : $"{inlineShape.AnchorID} - {inlineShape.AlternativeText} - Inline Shape");
                }

                foreach (Microsoft.Office.Interop.Word.Shape shape in doc.Shapes)
                {
                    if (shape.Type == MsoShapeType.msoPicture)
                    {
                        imageNames.Add(string.IsNullOrWhiteSpace(shape.AlternativeText) ? "Imagem sem nome - Shape" : $"{shape.ID} - {shape.AlternativeText} - Shape");
                    }
                }

                // Salva os nomes das imagens em um arquivo TXT
                if (imageNames.Count > 0)
                {
                    File.WriteAllLines(outputTxtPath, imageNames);
                    System.Diagnostics.Process.Start("notepad.exe", outputTxtPath); // Abre o TXT automaticamente
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao processar o documento Word: " + ex.Message);
            }
            finally
            {
                // Fecha e libera recursos
                doc?.Close(false);
                wordApp.Quit();
                Marshal.ReleaseComObject(doc);
                Marshal.ReleaseComObject(wordApp);
            }
        }

    }
}
