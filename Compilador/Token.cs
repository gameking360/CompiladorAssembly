using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador
{
    public class Token
    {
        private int tokenType;
        private string lexeme;

        private Stack<Token> pilha;
        
        private Queue<Token> fila;

        public Token(int tokenType, string lexeme)
        {
            this.tokenType = tokenType;
            this.lexeme = lexeme;
            this.pilha = new Stack<Token>();
            this.fila = new Queue<Token>();

        }


        public string GetLexeme()
        {
            return this.lexeme;
        }

        public int GetTokenType()
        {
            return this.tokenType;
        }

        public static int GetTokenTypeByLexeme(string lexeme)
        {
            if (Constants.TOKENS_RESERVADOS.ContainsKey(lexeme))
            {
                return Constants.TOKENS_RESERVADOS[lexeme];
            }
            else if(lexeme.StartsWith("\"") && lexeme.EndsWith("\""))
            {
                return 22; // String literal
            }
            else if(int.TryParse(lexeme, out _))
            {
                return 21; // Integer literal
            }
            else if(decimal.TryParse(lexeme, out _))
            {
                return 23; // Decimal literal
            }
            else if (lexeme == "true" || lexeme == "false")
            {
                return 24; // Boolean literal
            }
            return 25; // Identifier
        }
    
        public void Push(Token token)
        {
            this.pilha.Push(token);
        }


        public bool IsEmpty()
        {
            return this.pilha.Count == 0;
        }

        public bool IsQueueEmpty()
        {
            return this.fila.Count == 0;
        }

        public Token Pop()
        {
            return this.pilha.Pop();
            
        }


        /// <summary>
        /// Adiciona um item na fila
        /// </summary>
        /// <param name="token"></param>
        public void Enqueue(Token token)
        {
            this.fila.Enqueue(token);
        }



        /// <summary>
        /// Retira um item da fila e o retorna. Se a fila estiver vazia, lança uma exceção.
        /// </summary>
        /// <returns></returns>
        public Token Dequeue()
        {
            return this.fila.Dequeue();
        }

        public void MostrarCodigos()
        {
            Console.WriteLine($"Token: {this.lexeme} | Tipo: {this.tokenType} - PILHA");
            foreach (var token in this.pilha)
            {
                token.MostrarCodigos();
            }
        }
    }
}
