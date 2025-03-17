using System.IO;
using System.Windows.Media.Imaging;

namespace ProjetaHDR
{
    public static class ResourceImage
    {
        /// <summary>
        /// Carrega uma imagem da pasta "Icons" embutida no Assembly
        /// </summary>
        public static BitmapImage GetIcon(string fileName)
        {
            return LoadImage("RevitAddin.Icons", fileName);
        }

        /// <summary>
        /// Carrega uma imagem da pasta "Resources" embutida no Assembly
        /// </summary>
        public static BitmapImage GetResource(string fileName)
        {
            return LoadImage("RevitAddin.Resources", fileName);
        }

        /// <summary>
        /// Método genérico para carregar uma imagem de qualquer pasta do Assembly
        /// </summary>
        private static BitmapImage LoadImage(string folder, string fileName)
        {
            string resourcePath = ResourceAssembly.GetNamespace() + $"{folder}.{fileName}";

            using (Stream stream = ResourceAssembly.GetAssembly().GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                    throw new FileNotFoundException($"Recurso {resourcePath} não encontrado.");

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }
    }
}
