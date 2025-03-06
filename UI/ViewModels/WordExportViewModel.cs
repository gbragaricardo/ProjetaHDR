using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;

namespace ProjetaHDR.UI.ViewModels
{
    internal class WordExportViewModel : ObservableObject
    {
        public string ExportPath { get; set; }

        private string _cidade;
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

        private string _estado;
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

            var test = new Services.SaveImageNamesToFile();
            test.ExecuteTESTE(@"C:\Users\Usuario\Desktop\LIXO\MMD-XXXXX-EXE-HDS-0101-REV0X.docx", @"C:\Users\Usuario\Desktop\resultado.txt");

            //using (var handler = new WordHandler(_doc.ProjectInformation, ExportPath))
            //{
            //    try
            //    {
            //        string titleBlock = Sheets.GetTitleBlockName(_doc);
            //        var consorcio = Sheets.ValidateTitleBlock(titleBlock);

            //        handler.OpenWordDocument();


            //        handler.ReplaceText("ProjectName", "Nome do projeto");
            //        handler.ReplaceText("Contratante", "Nome do Contratante");
            //        handler.ReplaceText("Date", "Data do Projeto");
            //        handler.ReplaceTextInFooter("TITLE", "Título do Arquivo");
            //        handler.ReplaceText("City", InputCidade);
            //        handler.ReplaceText("State", InputEstado);
            //        if (consorcio != null)
            //        {
            //            handler.ReplaceTextInFooter("Consorcio", consorcio);
            //            handler.ReplaceText("Consorcio", consorcio);
            //        }
                    

            //        handler.SaveAndClose();
            //        DocHandler.OpenDocument(ExportPath);

            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.ToString());
            //        handler.ExceptionClose();
            //        handler.Dispose();
            //    }

            //    CloseWindow?.Invoke();

            //}
        }

    }
}
