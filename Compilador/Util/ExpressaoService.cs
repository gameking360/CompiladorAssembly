using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Compilador.Constantes;
using Compilador.Tokens;

namespace Compilador.Util
{
    public static class ExpressaoService
    {

        private static readonly StringBuilder codigo = new();

        public static string CompilarExpressao(Expressao expressao)
        {
            codigo.Clear();

            CompilarNo(expressao);

            return codigo.ToString();
        }

        private static void CompilarNo(Expressao expressao)
        {
            switch (expressao) {

                case LiteralExpressao literal:
                    GerarLiteral(literal);
                    break;


                case IdentificadorExpressao identificador:
                    GerarIdentificador(identificador);
                    break;

                case BinariaExpressao binaria:
                    GerarBinario(binaria);
                    break;

                default:
                    break;
            
            }

        }

        private static void GerarLiteral(LiteralExpressao literal)
        {
            codigo.AppendLine($"mov eax, {literal.Token.GetLexeme()}");
        }

        private static void GerarIdentificador(IdentificadorExpressao identificador)
        {
            codigo.AppendLine($"mov eax, [rel {identificador.Token.GetLexeme()}]");
        }

        private static void GerarBinario(BinariaExpressao binario)
        {
            CompilarNo(binario.Esquerda);

            codigo.AppendLine("push rax");

            CompilarNo(binario.Direita);

            codigo.AppendLine("mov ecx, eax");
            codigo.AppendLine("pop rax");

            switch (binario.Operador.GetTokenType())
            {
                case 1:
                    codigo.AppendLine("add eax, ecx");
                    break;
                case 2:
                    codigo.AppendLine("sub eax, ecx");
                    break;
                case 3:
                    codigo.AppendLine("imul eax, ecx");
                    break;
                case 4:
                    codigo.AppendLine("cdq");
                    codigo.AppendLine("idiv ecx");
                    break;
                case 28:
                    codigo.AppendLine("cdq");
                    codigo.AppendLine("idiv ecx");
                    codigo.AppendLine("mov eax, edx");
                    break;
                default:
                    break;
            }

        }
    }
}
