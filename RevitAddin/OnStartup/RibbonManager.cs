using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace ProjetaHDR.Startup
{
    internal static class RibbonManager
    {
        internal static readonly string TabName = "ProjetaHDR";
        private static readonly string ThisAssemblyPath = Assembly.GetExecutingAssembly().Location;
        internal static List<PushButton> PushButtonsList = new List<PushButton>();
        internal static List<SplitButton> SplitButtonList = new List<SplitButton>();

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
        internal static PushButton CreateAndAddPushButton
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

            // Cria a imagem do ícone
            BitmapImage bitmap = ResourceImage.GetIcon(nomeImagem);

            // Define a imagem como o ícone do botão
            pushButton.LargeImage = bitmap;
            PushButtonsList.Add(pushButton);

            return pushButton;
        }

        internal static IList<PushButton> AddStackedPushButtons(RibbonPanel panel, PushButtonData button1, PushButtonData button2, PushButtonData button3 = null)
        {
            IList<RibbonItem> stackedItems;
            IList<PushButton> stackedPushButtons = new List<PushButton>();

            if (button3 == null)
                stackedItems = panel.AddStackedItems(button1, button2);
            else
                stackedItems = panel.AddStackedItems(button1, button2, button3);

            foreach (RibbonItem item in stackedItems)
                stackedPushButtons.Add(item as PushButton);
            

            foreach (var push in stackedPushButtons)
            {
                PushButtonsList.Add(push);
                push.Enabled = false;
            }

            return stackedPushButtons;
        }

        internal static SplitButton AddSplitButton(string name, string text, RibbonPanel panel, string imageName)
        {
            var splitButtonData = new SplitButtonData(name, text);

            BitmapImage bitmap = ResourceImage.GetIcon(imageName);


            SplitButton splitButton = panel.AddItem(splitButtonData) as SplitButton;
            splitButton.LargeImage = bitmap;
            splitButton.Enabled = false;
            SplitButtonList.Add(splitButton);

            return splitButton;
        }

        internal static PushButtonData CreatePushButtonData(string dataName, string nameOnUI, string fullClassName, string imageName, string toolTip)
        {
            var pushButtonData = new PushButtonData(dataName, nameOnUI, ThisAssemblyPath, fullClassName);

            BitmapImage bitmap = ResourceImage.GetIcon(imageName);
            pushButtonData.LargeImage = bitmap;
            pushButtonData.Image = bitmap;
            pushButtonData.ToolTip = toolTip;

            return pushButtonData;  
        }

    }
}
