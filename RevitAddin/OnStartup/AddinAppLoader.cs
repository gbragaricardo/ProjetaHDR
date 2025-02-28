using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Startup
{
    internal static class AddinAppLoader
    {
        internal static void StartupMain(UIControlledApplication application)
        {
            // Configuração de carregamento de assemblies
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string folderPath = @"C:\Users\Usuario\AppData\Roaming\Autodesk\Revit\Addins\2024\ProjetaHDR";
                string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
                return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
            };

            // Criar aba
            try
            {
                application.CreateRibbonTab(RibbonManager.TabName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Aba '{RibbonManager.TabName}' já existe ou houve um erro ao criá-la: {ex.Message}");
            }

            // Inicializar interface (botões e painéis)
            UIBuilder.BuildUI(application);
        }
    }
}
