using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Tokens
{
    public abstract class Expressao
    {
    }

    public class LiteralExpressao : Expressao
    {
        public Token Token { get; set; }
    }

    public class IdentificadorExpressao : Expressao
    {
        public Token Token { get; set; }
    }

    public class BinariaExpressao : Expressao
    {
        public Expressao Esquerda { get; set; }
        public Token Operador { get; set; }
        public Expressao Direita { get; set; }
    }
}
