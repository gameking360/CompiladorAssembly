using Compilador.Constantes;
using Compilador.Tokens;

namespace Compilador.Semantico
{
    public  class VariavelService
    {
        private int ContadorToken;
        private Token? TokenAtual { get
            {
                return ContadorToken < TokensExpressao.Count ?
                                        TokensExpressao[ContadorToken] : null; ;
            } 
        
        }


        private List<Token> TokensExpressao;
        

        public VariavelService(List<Token> tokens)
        {
            this.TokensExpressao = tokens;
            this.ContadorToken = 0;
        }


        public Expressao AnalisarExpressao()
        {

            return AnalisarAdicao();
        }


        private Expressao AnalisarAdicao()
        {
            Expressao esquerda = AnalisarMultiplicacao();

            while (EhSomaOuSubtracao(TokenAtual))
            {
                Token? operador = ConsumirToken();
                Expressao direita = AnalisarMultiplicacao();

                esquerda = new BinariaExpressao
                {
                    Esquerda = esquerda,
                    Direita = direita,
                    Operador = operador
                };
            }
            return esquerda;
        }

        private Expressao AnalisarMultiplicacao()
        {
            Expressao esquerda = AnalisarPrimario();

            while (EhMultiplicacaoOuDivisao(TokenAtual))
            {

                Token operador = ConsumirToken();
                Expressao direita = AnalisarPrimario();

                esquerda = new BinariaExpressao
                {
                    Esquerda = esquerda,
                    Direita = direita,
                    Operador = operador
                };
            }
            return esquerda;
        }

        private Expressao AnalisarPrimario()
        {
            if (EhLiteral(TokenAtual))
            {
                return new LiteralExpressao
                {
                    Token = ConsumirToken()
                };
            }

            if (EhIdentificador(TokenAtual))
            {
                return new IdentificadorExpressao
                {
                    Token = ConsumirToken()
                };
            }

            if (EhAbreParenteses(TokenAtual))
            {
                ConsumirToken();

                Expressao expressao = AnalisarExpressao();

                ExigirFechaParenteses();

                return expressao;
            }

            throw new Exception("Expressão inválida.");
        }
        private bool EhSomaOuSubtracao(Token? token)
        {
            if (token == null) return false;

            return new int[] { 1, 2 }.Contains(token.GetTokenType());
        }

        private bool EhMultiplicacaoOuDivisao(Token? token)
        {
            if (token == null) return false;

            return new int[] { 3, 4 }.Contains(token.GetTokenType());
        }

        private bool EhLiteral(Token? token)
        {
            if (token == null) return false;
            var tipo = token.GetTokenType();
            return new int[]
            {
                (int)CodigosEnum.CODIGO_LITERAL_BOOL,
                (int)CodigosEnum.CODIGO_LITERAL_DECIMAL,
                (int)CodigosEnum.CODIGO_LITERAL_INT,
                (int)CodigosEnum.CODIGO_LITERAL_STRING
            }.Contains(tipo);
        }

        private bool EhAbreParenteses(Token token)
        {
            return (int)CodigosEnum.CODIGO_ABRE_PARENTESES == token.GetTokenType();
        }

        private bool EhIdentificador(Token? token)
        {
            if (token == null) return false;
            return (int)CodigosEnum.CODIGO_IDENTIFIER == token.GetTokenType();
        }

        private Token? ConsumirToken()
        {
            var tokenAntigo = this.TokenAtual;
            if (this.TokenAtual != null) ContadorToken++;
            return tokenAntigo;
        }

        private void ExigirFechaParenteses()
        {
            if (TokenAtual?.GetTokenType() != (int)CodigosEnum.CODIGO_FECHA_PARENTESES)
            {
                throw new Exception(
                    "Esperado ')' na expressão."
                );
            }

            ConsumirToken();
        }



    }
}
