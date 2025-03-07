using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;

namespace ProjetaHDR.UI.ViewModels
{
    internal class WordExportViewModel : ObservableObject
    {
        public string ExportPath { get; set; }

        private string _cidade = "Belo Horizonte";
        public string InputCidade
        {
            get => _cidade;
            set
            {
                if (_cidade != value)
                {
                    _cidade = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _estado = "Minas Gerais";
        public string InputEstado
        {
            get => _estado;
            set
            {
                if (_estado != value)
                {
                    _estado = value;
                    OnPropertyChanged();
                }
            }
        }

        public Action CloseWindow { get; set; }

        public RelayCommand ExportCommand { get; }
        private readonly RevitContext _context;
        private Document _doc;

        public WordExportViewModel(RevitContext context)
        {

            ExportCommand = new RelayCommand(Replace);

            _context = context;
            _doc = _context.Doc;
            ExportPath = DocHandler.GetSavePath();

            if (ExportPath == null)
                return;

            DocHandler.LoadDocument(ExportPath);

        }

        private void Replace(object parameter)
        {
            using (var handler = new WordHandler(_doc.ProjectInformation, ExportPath))
            {
                try
                {
                    var sheet = new Sheets();
                    string titleBlock = sheet.GetTitleBlockName(_doc);
                    var consorcioFullName = sheet.ValidateTitleBlock(titleBlock);

                    handler.OpenWordDocument();


                    handler.ReplaceText("ProjectName", "Nome do projeto");
                    handler.ReplaceText("Contratante", "Nome do Contratante");
                    handler.ReplaceText("Date", "Data do Projeto");
                    handler.ReplaceTextInFooter("TITLE", "Título do Arquivo");
                    handler.ReplaceText("City", InputCidade);
                    handler.ReplaceText("State", InputEstado);
                    if (consorcioFullName != null)
                    {
                        handler.ReplaceTextInFooter("Consorcio", consorcioFullName);
                        handler.ReplaceText("Consorcio", consorcioFullName);

                        var consorcioImagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", $"{sheet.Consorcio}.png");

                        if (File.Exists(consorcioImagePath))
                        {
                            handler.ReplaceImage("consorcio capa", consorcioImagePath);
                            handler.ReplaceFooterImage("consorcio rodape", consorcioImagePath);
                            handler.ReplaceTableImage("consorcio resumo", consorcioImagePath);
                        }
                    }

                    handler.SaveAndClose();
                    DocHandler.OpenDocument(ExportPath);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    handler.ExceptionClose();
                    handler.Dispose();
                }

                CloseWindow?.Invoke();

            }
        }

    }
}
