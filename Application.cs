using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using ProjetaHDR.OnStartup;

namespace ProjetaHDR
{
    public class Application : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;

        }

        public Result OnStartup(UIControlledApplication application)
        {

            try { AddinAppLoader.StartupMain(application); }
            catch { TaskDialog.Show("ProjetaHDR", "Erro ao inicializar Plugin ProjetaHDR"); }

            return Result.Succeeded;


        }
    }
}
