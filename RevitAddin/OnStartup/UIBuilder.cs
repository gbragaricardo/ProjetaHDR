using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Word;

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

            var SlopeSplitButton = RibbonManager.AddSplitButton("⠀⠀Tag⠀⠀\n⠀⠀Inclinacao⠀⠀", "Modos", PanelMain, "porcentagem.png");

            var realSlope = RibbonManager.CreatePushButtonData(
                "RealSlopeTag",
                "Inclinação\n⠀⠀Real⠀⠀",
                "ProjetaHDR.Commands.RealSlopeTag",
                "porcentagem.png",
                "Insere a Tag \"PRJ HDR: Inclinacao Real\" nos tubos da vista ativa");

            var expectedSlope = RibbonManager.CreatePushButtonData(
                "expectedSlopeTag",
                "Inclinação\nProvavel",
                "ProjetaHDR.Commands.SlopeTag",
                "porcentagem.png",
                "Insere a Tag \"PRJ HDR: Inclinacao\" nos tubos da vista ativa");

            SlopeSplitButton.AddPushButton(expectedSlope);
            SlopeSplitButton.AddPushButton(realSlope);
            SlopeSplitButton.IsSynchronizedWithCurrentItem = true;


            //var SlopeTagPushButton = RibbonManager.CreateAndAddPushButton
            //("SlopeTag", "⠀⠀Tag⠀⠀\n⠀⠀Inclinacao⠀⠀",
            //"ProjetaHDR.Commands.SlopeTag",
            //PanelMain,
            //"Insere a Tag \"PRJ HDR: Inclinacao\" nos tubos da vista ativa",
            //"porcentagem.png",
            //false);

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
            ("drenwindow", "⠀Rede⠀\n⠀Pluvial⠀",
            "ProjetaHDR.Commands.RainNetwork",
            _drenPanel,
            "Interface para cálculo de vazão da Rede Pluvial",
            "drenwindow.png",
            false);

            var setAreasGP = RibbonManager.CreatePushButtonData(
                "SetAreasGP",
                "Intensidade Pluviometrica", 
                "ProjetaHDR.Commands.SetAreasGP",
                "globalparam.png", 
                "Insere nas áreas os Parâmetros Globais K A B e C para calculo de vazão");

            var setAreasTR = RibbonManager.CreatePushButtonData(
                "SetAreasTR",
                "Tempo De Retorno", 
                "ProjetaHDR.Commands.SetAreasTR",
                "tr.png",
                "Insere nas áreas o parâmetro de tempo de retorno - 25 Cobertura - Térreo");

            var drenButtons = RibbonManager.AddStackedPushButtons(_drenPanel, setAreasGP, setAreasTR);
        }
    }
}
