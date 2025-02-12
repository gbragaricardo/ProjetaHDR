using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;

namespace ProjetaHDR.Commands.Helpers
{
    public static class DocHandler
    {
        internal static string NewPath { get; set; }
        internal static string RootPath { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"Docs", "mmd.docx");
        public static string ObterCaminhoSalvar()
        {
            // Abrir janela "Salvar Como"
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Arquivos Word (*.docx)|*.docx",
                Title = "Salvar documento como",
                FileName = "MMD-XXXXX-EXE-HDS-0101-REV0X.docx" // Nome sugerido

            };

            if (saveFileDialog.ShowDialog() == true)
            {
                NewPath = saveFileDialog.FileName;
            }
            else
            {
                TaskDialog.Show("Aviso", "Operação Cancelada");
            }

            return NewPath;
        }

        public static void CarregarDocumento(string exportPath)
        {
            if (!File.Exists(RootPath))
                TaskDialog.Show("Aviso", "Arquivo Base Nao Encontrado");

            // Copiar o arquivo base para o destino (mantendo um documento original)
            File.Copy(RootPath, exportPath, overwrite: true);
        }

        public static void AbrirDocumento(string exportPath)
        {
            if (File.Exists(exportPath))
            {
                Process.Start(new ProcessStartInfo(exportPath) { UseShellExecute = true });
            }
            else
            {
                TaskDialog.Show("Aviso", "Não foi possível abrir o arquivo");
            }
        }

    }
}
