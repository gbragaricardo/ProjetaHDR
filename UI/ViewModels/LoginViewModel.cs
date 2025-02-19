using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
using ProjetaHDR.UI.Services;
using System.Windows.Input;
using ProjetaHDR.RevitContext;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using ProjetaHDR.OnStartup;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;

namespace ProjetaHDR.UI.ViewModels
{
    internal class LoginViewModel : ObservableObject
    {
        private string _username;
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

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }



        public BitmapImage ImagesPath{ get; set; }

        RevitContext.RevitContext _context;
        private readonly AuthService _authService;
        public RelayCommand LoginCommand { get; }

        public LoginViewModel(RevitContext.RevitContext context)
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(ExecuteLogin);
            _context = context;


            string imagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UIResources", "logo-projeta-main.png");

            if (File.Exists(imagePath))
            {
                ImagesPath = new BitmapImage(new Uri(imagePath));
            }
        }

        private void ExecuteLogin(object parameter)
        {
            if (parameter is PasswordBox passwordBox)
            {
                string password = passwordBox.Password;

                if (_authService.Authenticate(Username, password))
                {
                    Message = "Login bem-sucedido!";
                    EnableUI();
                }
                else
                {
                    Message = "Usuário ou senha inválidos!";
                }
            }
        }

        internal void EnableUI()
        {
            using (Transaction trans = new Transaction(_context.Doc, "Ativar Botões"))
            {
                trans.Start();
                
                foreach (var pushButton in RibbonManager.PushButtonsList)
                {
                    if (pushButton.Name != "Dev")
                        pushButton.Enabled = true;
                }

                trans.Commit();
            }
        }
    }
}
