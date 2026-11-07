using System.Text;
using Compilador.Constantes;
using Compilador.Tokens;

namespace Compilador.Lexico
{
    internal ref struct LexicoService
    {
        private ReadOnlySpan<char> Input;
        private int Position;

        public LexicoService()
        {
            this.Position = 0;
        }

        public void SetInput(string input)
        {
            this.Input = input.AsSpan();
        }


        public Token GetProximoToken()
        {

            if (!TemEntrada())
                return null;

            StringBuilder sb = new StringBuilder();
            while (TemEntrada())
            { 
                var atual = this.Input[Position++];

                if (char.IsWhiteSpace(atual) && sb.Length == 0) continue;

                if (char.IsWhiteSpace(atual))
                {
                    var token = GetToken(sb.ToString());

                    if (Constants.TOKENS_RESERVADOS.ContainsKey(sb.ToString()))
                    {

                        if (Constants.NECESSITA_PARENTESES.IndexOf(token.GetLexeme()) >= 0)
                        {
                            Position++;
                            while (TemEntrada() && this.Input[Position] != ')')
                            {
                                token.Enqueue(GetProximoToken());
                            }

                        }
                    }
                    return token;
                }

                if(atual == ';')
                {
                    if (sb.Length == 0)
                        return GetToken(atual.ToString());

                    Position--;
                    return GetToken(sb.ToString());
                }
                    


                sb.Append(atual);

                if (atual == '"')
                {

                    atual = this.Input[Position];

                    while (TemEntrada() && atual != '"')
                    {
                        sb.Append(atual);
                        Position++;
                        if(TemEntrada()) atual = this.Input[Position];

                    }
                    sb.Append(atual);
                    Position++;
                    return  GetToken(sb.ToString());
                }

                if (Constants.TOKENS_RESERVADOS.ContainsKey(sb.ToString()))
                {
                    var token = GetToken(sb.ToString());
                    if (Constants.NECESSITA_PARENTESES.IndexOf(token.GetLexeme()) >= 0)
                    {
                        Position++;
                        while (TemEntrada() &&  !new[] {')', ';'}.Contains(this.Input[Position]))
                        {
                            token.Enqueue(GetProximoToken());
                            this.Position++;
                        }
                        return token;

                    }
                }



            }

            return GetToken(sb.ToString());

        }

        


        private Token GetToken( string lexeme)
        {
            return new Token(Token.GetTokenTypeByLexeme(lexeme), lexeme);
        }
        private bool TemEntrada() => (Position) < (Input.Length);
        
        public void EscanearEspacosEmBranco()
        {
            while (TemEntrada() && char.IsWhiteSpace(this.Input[this.Position])) this.Position++; 
        }
    }
}
