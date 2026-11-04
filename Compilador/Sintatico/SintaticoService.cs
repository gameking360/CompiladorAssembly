using System;
using System.Collections.Generic;
using System.Text;
using Compilador.Semantico;

namespace Compilador.Sintatico
{
    internal class SintaticoService
    {


        public void Parse(Lexico.LexicoService lexico, SemanticoService semantico)
        {
            Token token = lexico.GetProximoToken();
            if(token != null) semantico.ExecutarAcao(0, token);
            while (token != null && token.GetTokenType() != -1)
            {
                Console.WriteLine($"Token: {token.GetLexeme()} | Tipo: {token.GetTokenType()}");
                semantico.ExecutarAcao(token.GetTokenType(), token);
                token = lexico.GetProximoToken();
            }
        }
    }
}
