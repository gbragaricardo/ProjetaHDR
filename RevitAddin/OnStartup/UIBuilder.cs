using Autodesk.Revit.UI;
using ProjetaHDR.Commands;
using ProjetaHDR.UI.ViewModels;
using ProjetaHDR.UI.Views;
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
            "dev.png",
            true);

            var DevPushButton = RibbonManager.CriarPushButton
            ("Dev", "Dev",
            "ProjetaHDR.Commands.Dev",
            PanelMain,
            "Modo Desenvolvedor",
            "dev.png",
            false);

            var DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "Tag\nDiâmetro",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.png",
            false);

            var SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "Tag\nInclinacao",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag PRJ HDR: Diametro nos tubos da vista ativa",
            "dev.png",
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

            var FamiliesCommand = RibbonManager.CriarPushButton
            ("FamiliesCommand", "ShowFamiliesPage",
            "ProjetaHDR.Commands.ShowFamiliesPage",
            Documents,
            "FamiliesCommand",
            "dev.png",
            false);

            //var nomeBotao = RibbonManager.CriarPushButton
            //("NomeInterno", "NomeExibido",
            //"NameSpace.Classe",
            //RibbonPanel,
            //"Dica de uso",
            //"imagem.ico",
            //Enable = false);

            DockablePaneId paneId = new DockablePaneId(new Guid("5072DFD8-7026-4CE6-A7C8-E56336EB21B2"));

            // Criando a View que será exibida no Dockable Panel
            var familiesViewModel = new FamiliesViewModel();

            // Criar a View e passar a ViewModel para ela

            var familiesView = new FamiliesView(familiesViewModel);

            // Registrando o DockablePane corretamente
            application.RegisterDockablePane(paneId, "Painel de Famílias", familiesView);
        }
    }
}
