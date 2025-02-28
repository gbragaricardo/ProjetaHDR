using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ProjetaHDR.Startup
{
    internal static class UIBuilder
    {
        internal static RibbonPanel PanelMain { get; set; }
        internal static RibbonPanel Tabelas { get; set; }
        internal static RibbonPanel Documents { get; set; }
        internal static PushButton DevPushButton { get; set; }
        internal static PushButton DiameterTagPushButton { get; set; }
        internal static PushButton SlopeTagPushButton { get; set; }
        internal static PushButton FlowTagPushButton { get; set; }
        internal static PushButton NestedPipeFittingsPushButton { get; set; }
        internal static PushButton MemoHDS { get; set; }

        internal static void BuildUI(UIControlledApplication application)
        {
            var panelProjeta = RibbonManager.CriarRibbonPanel(application, "⠀⠀⠀⠀⠀Login⠀⠀⠀⠀⠀");
            var PanelMain = RibbonManager.CriarRibbonPanel(application, "Main");
            var Tabelas = RibbonManager.CriarRibbonPanel(application, "Tabelas");
            var Documents = RibbonManager.CriarRibbonPanel(application, "Documentos");



            var LoginPushButton = RibbonManager.CriarPushButton
            ("GrupoProjeta", "Grupo Projeta",
            "ProjetaHDR.Commands.Login",
            panelProjeta,
            "Sobre",
            "projeta.png",
            true);

            var DevPushButton = RibbonManager.CriarPushButton
            ("Dev", "⠀Dev⠀",
            "ProjetaHDR.Commands.Dev",
            PanelMain,
            "Modo Desenvolvedor",
            "dev.png",
            false);
            DevPushButton.Visible = false;

            var DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "Tag\n⠀Diâmetro⠀",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.png",
            false);

            var SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "Tag\n⠀Inclinacao⠀",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.png",
            false);

            var FlowTagPushButton = RibbonManager.CriarPushButton
            ("Flow Tag", "Tag\n⠀Fluxo⠀",
            "ProjetaHDR.Commands.FlowTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            false);

            var NestedPipeFittingsPushButton = RibbonManager.CriarPushButton
            ("Nested PF", "⠀Nested⠀\n⠀Fittings⠀",
            "ProjetaHDR.Commands.SanFittings",
            Tabelas,
            "Insere o sistema em conexoes aninhadas",
            "dev.ico",
            false);

            var MemoHDS = RibbonManager.CriarPushButton
            ("Memorial descritivo HDS", "⠀HDS - Memorial⠀\n⠀Descritivo⠀",
            "ProjetaHDR.Commands.MemoHDS",
            Documents,
            "Exporta Memorial descritivo HDS",
            "dev.ico",
            false);

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
