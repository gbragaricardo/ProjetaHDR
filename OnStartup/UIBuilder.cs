using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.OnStartup
{
    internal static class UIBuilder
    {
        internal static PushButton DevPushButton { get; set; }

        internal static void BuildUI(UIControlledApplication application)
        {
            RibbonPanel panelMain = RibbonManager.CriarRibbonPanel(application, "Main");

            DevPushButton = RibbonManager.CriarPushButton
            ("Dev", "Dev",
            "ProjetaHDR.Commands.Dev",
            panelMain,
            "Modo Desenvolvedor",
            "dev.ico",
            true);


            //var nomeBotao = RibbonManager.CriarPushButton
            //("NomeInterno", "NomeExibido",
            //"NameSpace.Classe",
            //RibbonPanel,
            //"Dica de uso",
            //"imagem.ico",
            //Enable = false);
        }
    }
}
