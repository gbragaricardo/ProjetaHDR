using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ProjetaHDR.OnStartup
{
    internal static class RibbonManager
    {
        internal static readonly string TabName = "ProjetaHDR";
        private static readonly string ThisAssemblyPath = Assembly.GetExecutingAssembly().Location;
        internal static List<PushButton> PushButtonsList = new List<PushButton>();


        // Criar painel
        internal static RibbonPanel CriarRibbonPanel(UIControlledApplication application, string nomePainel)
        {
            try
            {
                return application.CreateRibbonPanel(TabName, nomePainel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao criar o painel '{nomePainel}': {ex.Message}");
                return null;
            }
        }

        // Criar botões no painel
        internal static PushButton CriarPushButton
            (string nomeInterno, string nomeExibido,
             string nomeClasse, RibbonPanel painel,
             string dica, string nomeImagem,
             bool enableOption = false)
        {

            if (painel == null)
            {
                Debug.WriteLine($"Painel inválido para criar o botão {nomeInterno}");
                return null;
            }

            PushButtonData pushButtonData = new PushButtonData(nomeInterno, nomeExibido, ThisAssemblyPath, nomeClasse);
            if (pushButtonData == null)
                return null;


            PushButton pushButton = painel.AddItem(pushButtonData) as PushButton;
            pushButton.Enabled = enableOption;
            pushButton.ToolTip = dica;

            // Define o caminho para o ícone do botão
            string iconPath = Path.Combine(Path.GetDirectoryName(ThisAssemblyPath), "Icons", nomeImagem);

            // Cria a imagem do ícone
            Uri uri = new Uri(iconPath);
            BitmapImage bitmap = new BitmapImage(uri);
            // Define a imagem como o ícone do botão
            pushButton.LargeImage = bitmap;
            PushButtonsList.Add(pushButton);

            return pushButton;
        }
    }
}
