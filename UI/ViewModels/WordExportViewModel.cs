using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR.UI.ViewModels
{
    internal class WordExportViewModel : ObservableObject
    {

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

        public RelayCommand ExportCommand { get; }
        private readonly RevitContext _context;
        private string _exportPath;
        private Document _doc;

        public WordExportViewModel(RevitContext context)
        {

            ExportCommand = new RelayCommand(Replace);

            _context = context;
            _doc = _context.Doc;

            _exportPath = DocHandler.GetSavePath();
            DocHandler.LoadDocument(_exportPath);

        }

        private void Replace(object parameter)
        {
            using (var handler = new WordHandler(_doc.ProjectInformation, _exportPath))
                
            {
                try
                {
                    string titleBlock = Sheets.GetTitleBlockName(_doc);
                    var consorcio = Sheets.ValidateTitleBlock(titleBlock);

                    handler.OpenWordDocument();


                    handler.ReplaceText("ProjectName", "Nome do projeto");
                    handler.ReplaceText("Contratante", "Nome do Contratante");
                    handler.ReplaceText("Date", "Data do Projeto");
                    handler.ReplaceTextInFooter("TITLE", "Título do Arquivo");
                    handler.ReplaceTextInFooter("Consorcio", consorcio);
                    handler.ReplaceText("City", InputCidade);
                    handler.ReplaceText("State", InputEstado);
                    handler.ReplaceText("Consorcio", consorcio);

                    handler.SaveAndClose();

                    DocHandler.OpenDocument(_exportPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    handler.ExceptionClose();
                    handler.Dispose();
                }
            }
        }

    }
}
