using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Semantico
{
    public static class StringSemanticoService
    {


        public static string ConverterParaDb(string input)
        {
            var textoBruto = input.Replace("\"", "");

            textoBruto = textoBruto.Replace("\\n", "\n");
            textoBruto = textoBruto.Replace("\\r", "\r");

            var bytes = Encoding.Unicode.GetBytes(textoBruto);
            return string.Join(", ", bytes);
        }
    }
}
