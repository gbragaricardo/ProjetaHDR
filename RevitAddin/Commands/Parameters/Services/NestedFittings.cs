using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ProjetaHDR
{
    internal static class NestedFittings
    {
        /// <summary>
        /// Atribui o valor do parâmetro da família hospedeira para a família aninhada.
        /// </summary>
        internal static int InserirSistemaFamiliaAninhada(IList<Element> instancias, string nomeParametroHospedeiro, string nomeParametroAninhado)
        {
            int contador = 0;

            foreach (Element element in instancias)
            {
                if (element is FamilyInstance conexaoInstancia)
                {
                    FamilyInstance familiaHospedeira = conexaoInstancia.SuperComponent as FamilyInstance;
                    if (familiaHospedeira == null) continue;

                    Parameter parametroHospedeiro = familiaHospedeira.LookupParameter(nomeParametroHospedeiro);
                    if (parametroHospedeiro == null) continue;

                    string valorParametroHospedeiro = parametroHospedeiro.AsString();
                    if (string.IsNullOrEmpty(valorParametroHospedeiro)) continue;

                    Parameter parametroAninhado = conexaoInstancia.LookupParameter(nomeParametroAninhado);
                    if (parametroAninhado != null && parametroAninhado.AsString() != valorParametroHospedeiro)
                    {
                        parametroAninhado.Set(valorParametroHospedeiro);
                        contador++;
                    }
                }
            }

            return contador;
        }
    }
}
