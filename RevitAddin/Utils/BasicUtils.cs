using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetaHDR.Utils
{
    internal static class BasicUtils
    {
        public static string Captalize(string texto)
        {
            // Lista de palavras que devem permanecer em minúsculas (a menos que sejam a primeira palavra)
            string[] palavrasIgnoradas = { "de", "da", "do", "das", "dos", "em", "no", "na", "nos", "nas", "para", "e" };

            // Quebrar o texto em palavras
            var palavras = texto.ToLower().Split(' ');

            for (int i = 0; i < palavras.Length; i++)
            {
                // A primeira palavra sempre começa com maiúscula
                if (i == 0 || !palavrasIgnoradas.Contains(palavras[i]))
                {
                    palavras[i] = palavras[i].Substring(0, 1).ToUpper() + palavras[i].Substring(1);
                }
            }

            // Reunir as palavras em uma frase
            return string.Join(" ", palavras);
        }
    }
}
