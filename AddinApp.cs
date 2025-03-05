using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using ProjetaHDR.Startup;

namespace ProjetaHDR
{
    public class AddinApp : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;

        }

        public Result OnStartup(UIControlledApplication application)
        {

            try 
            { 
                AddinAppLoader.StartupMain(application); 
            }
            catch (Exception ex)
            { 
                TaskDialog.Show("ProjetaHDR", $"Erro ao inicializar Plugin ProjetaHDR: {ex.Message}"); 
            }

            return Result.Succeeded;


        }
    }
}
