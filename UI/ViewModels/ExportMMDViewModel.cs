using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
using ProjetaHDR.UI.Services;
using System.Windows.Input;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using ProjetaHDR.OnStartup;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using ProjetaHDR.Commands.Helpers;
using ProjetaHDR.Commands.MemosExport.Helpers;

namespace ProjetaHDR.UI.ViewModels
{
    internal class ExportMMDViewModel : ObservableObject
    {
        private string _username = "admin";
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }


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

        private bool _reforma;
        public bool Reforma
        {
            get => _reforma;
            set
            {
                if (_reforma != value)
                {
                    _reforma = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand ExportCommand { get; }
        private readonly RevitContext _context;
        private string _exportPath;
        private Document _doc;


        public ExportMMDViewModel(RevitContext context)
        {

            ExportCommand = new RelayCommand(Replace);
            _context = context;
            _exportPath = DocHandler.ObterCaminhoSalvar();
            DocHandler.CarregarDocumento(_exportPath);
            _doc = _context.Doc;

        }

        private void Replace(object parameter)
        {
            using (var handler = new WordHandler(_doc.ProjectInformation, _exportPath))
            {
                string titleBlock = Sheets.GetTitleBlockName(_doc);
                var consorcio = Sheets.ValidateTitleBlock(titleBlock);

                handler.OpenWordDocument();


                handler.ReplaceText("NOME DO PROJETO", "Nome do projeto");
                //handler.ReplaceText("Contratante", "Nome do Contratante");
                //handler.ReplaceText("Date", "Data do Projeto");
                //handler.ReplaceText("TITLE", "Título do Arquivo");
                handler.ReplaceText("Cidade", InputCidade);
                handler.ReplaceText("UF", InputEstado);
                //handler.ReplaceText("Consorcio", consorcio);

                if (Reforma)
                    handler.DeleteSpecificParagraph("Reforma");
                else
                    handler.DeleteTags("Reforma");

                handler.SaveAndClose();

                DocHandler.AbrirDocumento(_exportPath);
            }
        }



        //private void ExecuteLogin(object parameter)
        //{
        //    if (parameter is PasswordBox passwordBox)
        //    {
        //        string password = passwordBox.Password;

        //        if (_authService.Authenticate(Username, password))
        //        {
        //            Message = "Login bem-sucedido!";
        //            IsLoggedOff = false;
        //            EnableUI();
        //        }
        //        else
        //        {
        //            Message = "Usuário ou senha inválidos!";
        //        }
        //    }
        //}

        //internal void EnableUI()
        //{
        //    using (Transaction trans = new Transaction(_context.Doc, "Ativar Botões"))
        //    {
        //        trans.Start();

        //        foreach (var pushButton in RibbonManager.PushButtonsList)
        //        {
        //            if (pushButton.Name != "Dev")
        //                pushButton.Enabled = true;
        //        }

        //        trans.Commit();
        //    }
        //}
    }
}
