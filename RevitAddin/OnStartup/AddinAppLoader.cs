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
            // 1. Pega a versão do Revit dinamicamente (ex: "2024")
            string revitVersion = application.ControlledApplication.VersionNumber;

            // 2. Pega o caminho da pasta do add-in 
            string addinPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Autodesk",
                "Revit",
                "Addins",
                revitVersion,
                "ProjetaHDR"
            );

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = new AssemblyName(args.Name).Name;
                string assemblyPath = Path.Combine(addinPath, $"{assemblyName}.dll");

                return File.Exists(assemblyPath)
                    ? Assembly.LoadFrom(assemblyPath)
                    : null;
            };


            ////
            

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
