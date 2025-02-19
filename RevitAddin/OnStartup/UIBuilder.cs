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

        internal static void BuildUI(UIControlledApplication application)
        {
            var PanelMain = RibbonManager.CriarRibbonPanel(application, "Main");
            var Tabelas = RibbonManager.CriarRibbonPanel(application, "Tabelas");
            var Documents = RibbonManager.CriarRibbonPanel(application, "Documentos");

            var LoginPushButton = RibbonManager.CriarPushButton
            ("Home", "Grupo\nProjeta",
            "ProjetaHDR.Commands.Login",
            PanelMain,
            "Sobre",
            "dev.ico",
            true);

            var DevPushButton = RibbonManager.CriarPushButton
            ("Dev", "Dev",
            "ProjetaHDR.Commands.Dev",
            PanelMain,
            "Modo Desenvolvedor",
            "dev.ico",
            false);

            var DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "Tag\nDiâmetro",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            false);

            var SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "Tag\nInclinacao",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            false);

            var FlowTagPushButton = RibbonManager.CriarPushButton
            ("Flow Tag", "Tag\nFluxo",
            "ProjetaHDR.Commands.FlowTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.ico",
            false);

            var NestedPipeFittingsPushButton = RibbonManager.CriarPushButton
            ("Nested PF", "Nested\nFittings",
            "ProjetaHDR.Commands.NestedPipeFittings",
            Tabelas,
            "Insere o sistema em conexoes aninhadas",
            "dev.ico",
            false);

            var MemoHDS = RibbonManager.CriarPushButton
            ("Memorial descritivo HDS", "HDS - Memorial\nDescritivo",
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
