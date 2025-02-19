using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ProjetaHDR.RevitAddin.RevitContext
{
    public static class RevitStaticContext
    {
        public static UIApplication UiApp { get; set; }
        public static UIDocument UiDoc { get; set; }
        public static Autodesk.Revit.ApplicationServices.Application App { get; set; }
        public static Document Doc { get; set; }
        public static View ActiveView { get; set; }

        // Atualiza o contexto sempre que um comando é chamado
        public static void Update(UIApplication uiApp)
        {
            UiApp = uiApp;
            UiDoc = uiApp.ActiveUIDocument;
            App = uiApp.Application;
            Doc = UiDoc?.Document;
            ActiveView = Doc.ActiveView;
        }
    }
}
