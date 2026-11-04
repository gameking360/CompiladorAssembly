using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Compilador.Semantico
{
    public class SemanticoService
    {
        Stack<string> pilha;
        Stack<string> rotulos = new Stack<string>();
        List<string> identificadores;

        private StringBuilder declaracaoVariaveis;
        private StringBuilder code;
        private StringBuilder declaracaoFuncoes;
        private StringBuilder sessaoEspecial;

        private StringBuilder funcoesPersonalizadas;
        private Dictionary<string, string> tabelaSimbolos = new Dictionary<string, string>();
        private string operadorRelacional;
        private string tipo;


        public SemanticoService()
        {
            this.declaracaoVariaveis = new StringBuilder();
            this.code = new StringBuilder();
            this.identificadores = new List<string>();
            this.pilha = new Stack<string>();
            this.declaracaoFuncoes = new StringBuilder();
            this.sessaoEspecial = new StringBuilder();
            this.funcoesPersonalizadas = new StringBuilder();
        }


        public void ExecutarAcao(int action, Token token)
        {
            switch (action)
            {
                case -1:
                    this.ExecutarFim();
                    break;
                case 0:
                    this.ExecuteInicio();
                    break;
                case 1:
                    this.tipo = token.GetLexeme();
                    break;
                case 2:
                    this.identificadores.Add(token.GetLexeme());
                    break;
                case 3:
                    this.declaracaoVariaveis.AppendLine($"var {token.GetLexeme()}: {this.tipo};");
                    this.tabelaSimbolos[token.GetLexeme()] = this.tipo;
                    break;
                case 4:
                    this.operadorRelacional = token.GetLexeme();
                    break;
                case 6:

                case 7:
                    this.ExecutaSaida(token);
                    break;
                case 22:

                case 25:
                default:
                    break;
            }
        }


        private void ExecutaSaida(Token token)
        {
            while(!token.IsQueueEmpty())
            {
                var atual = token.Dequeue();
                string nome = string.Empty;
                if (atual.GetTokenType() == 25)
                {
                   nome = this.DeclararVariavel(atual);
                }
                else if (atual.GetTokenType() == 22)
                {
                   nome = this.DeclararConstante(atual);
                }



                    this.AdicionarCodigo($"lea rdx, [{nome}]");

                    this.AdicionarCodigo($"mov r8d, {nome}_len");
                    this.AdicionarCodigo("call Print");


            }

            
        }

        private string DeclararConstante(Token token)
        {
            var nome = "constante_" + pilha.Count + 1;
            this.declaracaoVariaveis.AppendLine($"{nome} db {StringSemanticoService.ConverterParaDb(token.GetLexeme())}, 0");
            this.declaracaoVariaveis.AppendLine($"{nome}_len equ ($ - {nome}) / 2");
            this.pilha.Push(nome);
            return nome;
        }

        private string DeclararVariavel(Token token)
        {
            //if (!this.tabelaSimbolos.ContainsKey(token.GetLexeme()))
            //{
            //    throw new Exception($"Variável {token.GetLexeme()} não declarada.");
            //}
            var nome = "variavel_" + pilha.Count + 1;
            this.declaracaoVariaveis.AppendLine($"{nome} db 0");
            this.pilha.Push(nome);
            return nome;
        }


        private void ExecuteInicio()
        {
            this.declaracaoVariaveis.AppendLine("section .data");
            this.CriarCodigoBase();
            this.declaracaoFuncoes.AppendLine("default rel");
            this.declaracaoFuncoes.AppendLine("extern ExitProcess");
            this.AdicionarFuncao("extern GetStdHandle");
            this.AdicionarFuncao("extern WriteConsoleW");
            this.IniciarSessaoEspecial();
            this.IniciarDeclaracaoFuncoes();
        }


        private void CriarCodigoBase()
        {
            this.code.AppendLine("section .text");
            this.code.AppendLine("global main");
            this.code.AppendLine("main:");
            this.code.AppendLine("    sub  rsp, 40");
            this.code.AppendLine("    mov  ecx, -11");
            this.code.AppendLine("    call GetStdHandle");
            this.code.AppendLine("    mov  [console_handle], rax");
        }

        private void AdicionarFuncao(string funcao)
        {
            this.declaracaoFuncoes.AppendLine(funcao);
        }


        private void AdicionarCodigo(string codigo)
        {
            this.code.AppendLine(codigo);
        }

        private void IniciarSessaoEspecial()
        {
            this.sessaoEspecial.AppendLine("section .bss");
            this.sessaoEspecial.AppendLine("written resd 1");
            this.sessaoEspecial.AppendLine("console_handle resq 1");
        }

        private void IniciarDeclaracaoFuncoes()
        {
            this.funcoesPersonalizadas.AppendLine("Print: ");
            this.funcoesPersonalizadas.AppendLine(" sub rsp, 40");
            this.funcoesPersonalizadas.AppendLine(" mov rcx, [console_handle]");
            this.funcoesPersonalizadas.AppendLine("    lea r9, [written]");
            this.funcoesPersonalizadas.AppendLine("    mov qword[rsp + 32], 0");
            this.funcoesPersonalizadas.AppendLine("    call WriteConsoleW");
            this.funcoesPersonalizadas.AppendLine("    add rsp, 40");
            this.funcoesPersonalizadas.AppendLine("    ret");
        }

        private void AdicionarFuncaoPersonalizada(string codigo)
        {
            this.funcoesPersonalizadas.AppendLine(codigo);
        }

        private void AdicionarSessaoEspecial(string codigo)
        {
            this.sessaoEspecial.AppendLine(codigo);
        }


        public string ExecutarFim()
        {

            return this.declaracaoFuncoes.AppendLine(this.declaracaoVariaveis.ToString())
                                         .AppendLine(this.sessaoEspecial.ToString())
                                         .AppendLine(this.code.ToString())
                                         .AppendLine(" xor ecx, ecx\r\n    call ExitProcess")
                                         .AppendLine(this.funcoesPersonalizadas.ToString())
                                         .ToString();
        }


    }
}
