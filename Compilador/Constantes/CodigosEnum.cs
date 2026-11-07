using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Constantes
{
    public enum CodigosEnum
    {
        CODIGO_TIPO_STRING = 11,
        CODIGO_TIPO_INT = 12,
        CODIGO_TIPO_DECIMAL = 13,
        CODIGO_TIPO_BOOL = 14,

        CODIGO_LITERAL_INT = 21,
        CODIGO_LITERAL_STRING = 22,
        CODIGO_LITERAL_DECIMAL = 23,
        CODIGO_LITERAL_BOOL = 24,
        CODIGO_IDENTIFIER = 25,

        CODIGO_ABRE_PARENTESES,
        CODIGO_FECHA_PARENTESES
    }
}
