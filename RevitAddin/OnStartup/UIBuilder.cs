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
            "dev.ico",
            false);
            DevPushButton.Visible = false;

            var DiameterTagPushButton = RibbonManager.CriarPushButton
            ("DiameterTag", "⠀⠀Tag⠀⠀\n⠀⠀Diâmetro⠀⠀",
            "ProjetaHDR.Commands.DiameterTag",
            PanelMain,
            "Insere a Tag \"PRJ HDR: Diametro\" nos tubos da vista ativa",
            "diameter.png",
            false);

            var SlopeTagPushButton = RibbonManager.CriarPushButton
            ("SlopeTag", "⠀⠀Tag⠀⠀\n⠀⠀Inclinacao⠀⠀",
            "ProjetaHDR.Commands.SlopeTag",
            PanelMain,
            "Insere a Tag \"PRJ HDR: Inclinacao\" nos tubos da vista ativa",
            "porcentagem.png",
            false);

            var FlowTagPushButton = RibbonManager.CriarPushButton
            ("Flow Tag", "⠀⠀Tag⠀⠀\n⠀⠀Fluxo⠀⠀",
            "ProjetaHDR.Commands.FlowTag",
            PanelMain,
            "Insere a Tag \"PRJ HDR: Fluxo\" nos tubos da vista ativa",
            "setafluxo.png",
            false);

            var NestedPipeFittingsPushButton = RibbonManager.CriarPushButton
            ("NestedPF", "⠀Parametros⠀\n⠀Aninhados⠀",
            "ProjetaHDR.Commands.SanFittings",
            Tabelas,
            "Insere o sistema em conexoes aninhadas, use quando tiver problemas de luvas aparecendo simultaneamente na tabela de esgoto e pluvial",
            "parameters.png",
            false);

            var etapasSeduc = RibbonManager.CriarPushButton
            ("etapas seduc", "⠀Etapa⠀\n⠀Seduc⠀",
            "ProjetaHDR.Commands.Seduc",
            Tabelas,
            "Passa o valor do parametro compartilhado \"Etapa Seduc\" das familias mães para as familias aninhadas (ex: Luva de correr dentro de um joelho)",
            "etapaseduc.png",
            false);

            var MemoHDS = RibbonManager.CriarPushButton
            ("Memorial descritivo HDS", "⠀⠀HDS - Memorial⠀⠀\n⠀⠀Descritivo (Beta)⠀⠀",
            "ProjetaHDR.Commands.MemoHDS",
            Documents,
            "Exporta Memorial descritivo HDS",
            "blueword.ico",
            false);

            var setAreasGP = RibbonManager.CriarPushButton
            ("SetAreasGP", "⠀⠀SetAreas⠀⠀",
            "ProjetaHDR.Commands.SetAreasGP",
            Documents,
            "SetAreas",
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
