using Autodesk.Revit.UI;

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
            var _drenPanel = RibbonManager.CriarRibbonPanel(application, "⠀⠀Drenagem⠀⠀");




            var LoginPushButton = RibbonManager.CreateAndAddPushButton
            ("GrupoProjeta", "⠀⠀Grupo Projeta⠀⠀",
            "ProjetaHDR.Commands.Login",
            panelProjeta,
            "Sobre",
            "eggprojeta.png",
            true);

            var DevPushButton = RibbonManager.CreateAndAddPushButton
            ("Dev", "⠀⠀Dev⠀⠀",
            "ProjetaHDR.Commands.Dev",
            PanelMain,
            "Modo Desenvolvedor",
            "dev.ico",
            false);
            DevPushButton.Visible = false;

            var DiameterTagPushButton = RibbonManager.CreateAndAddPushButton
            ("DiameterTag", "⠀⠀Tag⠀⠀\n⠀⠀Diâmetro⠀⠀",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag \"PRJ HDR: Diametro\" nos tubos da vista ativa",
            "diameter.png",
            false);

            var SlopeTagPushButton = RibbonManager.CreateAndAddPushButton
            ("SlopeTag", "⠀⠀Tag⠀⠀\n⠀⠀Inclinacao⠀⠀",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag \"PRJ HDR: Inclinacao\" nos tubos da vista ativa",
            "porcentagem.png",
            false);

            var FlowTagPushButton = RibbonManager.CreateAndAddPushButton
            ("Flow Tag", "⠀⠀Tag⠀⠀\n⠀⠀Fluxo⠀⠀",
            "ProjetaHDR.Commands.FlowTag",
            PanelMain,
            "Insere a Tag \"PRJ HDR: Fluxo\" nos tubos da vista ativa",
            "setafluxo.png",
            false);

            var NestedPipeFittingsPushButton = RibbonManager.CreateAndAddPushButton
            ("NestedPF", "⠀Parametros⠀\n⠀Aninhados⠀",
            "ProjetaHDR.Commands.SanFittings",
            Tabelas,
            "Insere o sistema em conexoes aninhadas, use quando tiver problemas de luvas aparecendo simultaneamente na tabela de esgoto e pluvial",
            "parameters.png",
            false);

            var etapasSeduc = RibbonManager.CreateAndAddPushButton
            ("etapas seduc", "⠀Etapa⠀\n⠀Seduc⠀",
            "ProjetaHDR.Commands.Seduc",
            Tabelas,
            "Passa o valor do parametro compartilhado \"Etapa Seduc\" das familias mães para as familias aninhadas (ex: Luva de correr dentro de um joelho)",
            "etapaseduc.png",
            false);

            var MemoHDS = RibbonManager.CreateAndAddPushButton
            ("Memorial descritivo HDS", "⠀⠀HDS - Memorial⠀⠀\n⠀⠀Descritivo (Beta)⠀⠀",
            "ProjetaHDR.Commands.MemoHDS",
            Documents,
            "Exporta Memorial descritivo HDS",
            "blueword.ico",
            false);

            var drenWindow = RibbonManager.CreateAndAddPushButton
            ("drenwindow", "⠀⠀DrenWindow⠀⠀",
            "ProjetaHDR.Commands.Dev",
            _drenPanel,
            "drenWindow",
            "drenwindow.png",
            false);

            var setAreasGP = RibbonManager.CreatePushButtonData("SetAreasGP", "Set Intensidade Pluviometrica", "ProjetaHDR.Commands.SetAreasGP", "globalparam.png", "Botao Teste");
            var setAreasTR = RibbonManager.CreatePushButtonData("SetAreasTR", "Set Tempo Retorno", "ProjetaHDR.Commands.SetAreasGP", "tr.png", "Botao Teste");

            var drenButtons = RibbonManager.AddStackedPushButtons(_drenPanel, setAreasGP, setAreasTR);
        }
    }
}
