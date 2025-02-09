using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
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
        internal static PushButton DiameterTagPushButton { get; set; }
        internal static PushButton SlopeTagPushButton { get; set; }
        internal static PushButton FlowTagPushButton { get; set; }

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

            DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "Tag\nDiâmetro",
            "ProjetaHDR.Commands.DiameterTag",
            panelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            true);

            SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "Tag\nInclinacao",
            "ProjetaHDR.Commands.SlopeTag",
            panelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            true);

            FlowTagPushButton = RibbonManager.CriarPushButton
            ("Flow Tag", "Tag\nFluxo",
            "ProjetaHDR.Commands.FlowTag",
            panelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
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
