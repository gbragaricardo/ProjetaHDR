using System.Reflection;

namespace ProjetaHDR
{
    public static class ResourceAssembly
    {
        /// <summary>
        /// Obtém o Assembly atual (DLL onde os recursos estão embutidos)
        /// </summary>
        public static Assembly GetAssembly() => Assembly.GetExecutingAssembly();

        /// <summary>
        /// Obtém o namespace base onde os recursos estão armazenados
        /// </summary>
        public static string GetNamespace() => typeof(ResourceAssembly).Namespace + ".";
    }
}
