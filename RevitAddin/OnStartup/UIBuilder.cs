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
        internal static void BuildUI(UIControlledApplication application)
        {
            var panelProjeta = RibbonManager.CriarRibbonPanel(application, "⠀⠀⠀⠀⠀Login⠀⠀⠀⠀⠀");
            var PanelMain = RibbonManager.CriarRibbonPanel(application, "Main");
            var Tabelas = RibbonManager.CriarRibbonPanel(application, "⠀⠀Tabelas⠀⠀");
            var Documents = RibbonManager.CriarRibbonPanel(application, "⠀⠀Docs⠀⠀");



            var LoginPushButton = RibbonManager.CriarPushButton
            ("GrupoProjeta", "⠀⠀Grupo Projeta⠀⠀",
            "ProjetaHDR.Commands.Login",
            panelProjeta,
            "Sobre",
            "eggprojeta.png",
            true);

            var DevPushButton = RibbonManager.CriarPushButton
            ("Dev", "⠀⠀Dev⠀⠀",
            "ProjetaHDR.Commands.Dev",
            PanelMain,
            "Modo Desenvolvedor",
            "dev.png",
            false);
            DevPushButton.Visible = false;

            var DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "⠀⠀Tag⠀⠀\n⠀⠀Diâmetro⠀⠀",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "diameter.png",
            false);

            var SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "⠀⠀Tag⠀⠀\n⠀⠀Inclinacao⠀⠀",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "porcentagem.png",
            false);

            var FlowTagPushButton = RibbonManager.CriarPushButton
            ("Flow Tag", "⠀⠀Tag⠀⠀\n⠀⠀Fluxo⠀⠀",
            "ProjetaHDR.Commands.FlowTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "setafluxo.png",
            false);

            var NestedPipeFittingsPushButton = RibbonManager.CriarPushButton
            ("NestedPF", "⠀Parametros⠀\n⠀Aninhados⠀",
            "ProjetaHDR.Commands.SanFittings",
            Tabelas,
            "Insere o sistema em conexoes aninhadas",
            "parameters.png",
            false);

            var MemoHDS = RibbonManager.CriarPushButton
            ("Memorial descritivo HDS", "⠀⠀HDS - Memorial⠀⠀\n⠀⠀Descritivo⠀⠀",
            "ProjetaHDR.Commands.MemoHDS",
            Documents,
            "Exporta Memorial descritivo HDS",
            "blueword.ico",
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
