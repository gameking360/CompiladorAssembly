using System;
using System.Collections.Generic;
using System.Text;
using Compilador.Constantes;
using Compilador.Semantico;
using Compilador.Tokens;

namespace Compilador.Sintatico
{
    internal class SintaticoService
    {


        public void Parse(Lexico.LexicoService lexico, SemanticoService semantico)
        {
           
            Token token = lexico.GetProximoToken();
            if (token != null)
            {
                semantico.ExecutarAcao(0, new Token(-1, "inicio"));
            }

            while (token != null && token.GetTokenType() != -1)
            {

                semantico.ExecutarAcao(token.GetTokenType(), token);
                token = lexico.GetProximoToken();
            }
        }

       
    }
}
