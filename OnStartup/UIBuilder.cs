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
        internal static RibbonPanel PanelMain { get; set; }
        internal static RibbonPanel Tabelas { get; set; }
        internal static PushButton DevPushButton { get; set; }
        internal static PushButton DiameterTagPushButton { get; set; }
        internal static PushButton SlopeTagPushButton { get; set; }
        internal static PushButton FlowTagPushButton { get; set; }
        internal static PushButton NestedPipeFittingsPushButton { get; set; }

        internal static void BuildUI(UIControlledApplication application)
        {
            PanelMain = RibbonManager.CriarRibbonPanel(application, "Main");
            Tabelas = RibbonManager.CriarRibbonPanel(application, "Tabelas");



            DevPushButton = RibbonManager.CriarPushButton
            ("Dev", "Dev",
            "ProjetaHDR.Commands.Dev",
            PanelMain,
            "Modo Desenvolvedor",
            "dev.ico",
            true);

            DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "Tag\nDiâmetro",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            true);

            SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "Tag\nInclinacao",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            true);

            FlowTagPushButton = RibbonManager.CriarPushButton
            ("Flow Tag", "Tag\nFluxo",
            "ProjetaHDR.Commands.FlowTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            true);

            NestedPipeFittingsPushButton = RibbonManager.CriarPushButton
            ("Nested PF", "Nested\nFittings",
            "ProjetaHDR.Commands.NestedPipeFittings",
            Tabelas,
            "Insere o sistema em conexoes aninhadas",
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
