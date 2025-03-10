using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
using ProjetaHDR.UI.Services;
using System.Windows.Input;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using ProjetaHDR.Startup;
using System.Reflection;
using System.IO;
using System.Windows.Media.Imaging;
using System;
using System.Drawing;
using System.Windows;

namespace ProjetaHDR.UI.ViewModels
{
    internal class LoginViewModel : ObservableObject
    {
        private string _username = "Hidro";
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

        private bool isLoggedOff = true;
        public bool IsLoggedOff
        {
            get => isLoggedOff;
            set
            {
                if (isLoggedOff != value)
                {
                    isLoggedOff = value;
                    OnPropertyChanged();
                }
            }
        }

        public Action CloseWindow { get; set; }

        public BitmapImage ImagesPath { get; set; }

        private readonly RevitContext _context;
        private readonly AuthService _authService;
        public RelayCommand LoginCommand { get; }

        public LoginViewModel(RevitContext context)
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(ExecuteLogin);
            _context = context;


            string imagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources", "logo-projeta-main.png");

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
                    IsLoggedOff = false;
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

                if (Username.ToLower() == "dev")
                {
                    foreach (var pushButton in RibbonManager.PushButtonsList)
                    {
                        pushButton.Visible = true;
                            pushButton.Enabled = true;
                    }
                }
                else
                {
                    foreach (var pushButton in RibbonManager.PushButtonsList)
                    {
                        if (pushButton.Name != "Dev")
                            pushButton.Enabled = true;
                    }
                }

                trans.Commit();
                CloseWindow?.Invoke();
            }
        }
    }
}