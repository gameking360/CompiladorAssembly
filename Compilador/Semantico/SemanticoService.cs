using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Compilador.Constantes;
using Compilador.Tokens;
using Compilador.Util;

namespace Compilador.Semantico
{
    public class SemanticoService
    {
        Stack<Token> pilha;
        HashSet<string> constantes;
        Stack<string> rotulos = new Stack<string>();
        List<string> identificadores;

        private StringBuilder declaracaoVariaveis;
        private StringBuilder code;
        private StringBuilder declaracaoFuncoes;
        private StringBuilder sessaoEspecial;

        private StringBuilder funcoesPersonalizadas;
        private Dictionary<string, string> tabelaSimbolos = new Dictionary<string, string>();

        private Tipos? tipo;

        private bool isInFunctionDeclaration;

        private bool isPrintOperation;
        private List<Token> expressaoInicializacao;

        private HashSet<string> variaveis;
        private Dictionary<string,Tipos> tiposVariaveis;

        public SemanticoService()
        {
            this.declaracaoVariaveis = new StringBuilder();
            this.code = new StringBuilder();
            this.identificadores = new List<string>();
            this.pilha = new Stack<Token>();
            this.declaracaoFuncoes = new StringBuilder();
            this.sessaoEspecial = new StringBuilder();
            this.funcoesPersonalizadas = new StringBuilder();
            this.constantes = new HashSet<string>();
            this.isInFunctionDeclaration = false;
            this.isPrintOperation = false;
            this.expressaoInicializacao = new List<Token>();
            this.tipo = null;
            this.variaveis = new HashSet<string>();
            this.tiposVariaveis = new Dictionary<string, Tipos>();
        }


        public void ExecutarAcao(int action, Token token)
        {
            while (!token.IsQueueEmpty())
            {
                var primeiroDaFila = token.Dequeue();
                if (action == 6 && this.isInFunctionDeclaration) this.isInFunctionDeclaration = false;
                if (action == 7 && !this.isPrintOperation) this.isPrintOperation = true;
                this.ExecutarAcao(primeiroDaFila.GetTokenType(), primeiroDaFila);
            }

            switch (action)
            {
                case -1:
                    this.ExecutarFim();
                    break;
                case 0:
                    this.ExecuteInicio();
                    break;
                case 1:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    break;
                case 2:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    break;
                case 3:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    break;
                case 4:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    break;
                case 5:
                    this.isInFunctionDeclaration = true;
                    break;
                case 6:
                    this.ExecutaRead(token);
                    break;
                case 7:
                    this.ExecutaSaida(token);
                    break;
                case 11 or 12 or 13 or 14:
                    this.AdicionarTipo(token);
                    break;
                case 15:
                    this.FinalizarDeclaracao(token);
                    break;
                case 21:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }

                    this.DeclararConstante(token);
                    break;
                case 22:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    this.DeclararConstante(token);
                    break;
                case 23:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    this.DeclararConstante(token);
                    break;


                case 24:
                    if (this.isInFunctionDeclaration)
                    {
                        this.AdicionarExpressaoDeclaracao(token);
                        break;
                    }
                    
                    this.DeclararConstante(token);
                    break;
                    

                case 25:
                    this.AdicionarIdentificador(token);
                    break;
                default:
                    break;
            }
        }


        private void ExecutaSaida(Token token)
        {

              this.AdicionarCodigo(@"pop rax
                                    mov r8d, eax
                                    pop rdx
                                    call Print");

           // this.pilha.Pop();
            //this.tipo = null;
        }




        private void DeclararConstante(Token token)
        {
            this.AdicionarConstantes(token);
            var nome = this.constantes.Last();


            //this.tipo = token.GetTipoToken();
            switch (token.GetTokenType())
            {
                case (int)CodigosEnum.CODIGO_LITERAL_STRING:
                    this.declaracaoVariaveis.AppendLine($"{nome} db {StringSemanticoService.ConverterParaDb(token.GetLexeme())}, 0");
                    this.declaracaoVariaveis.AppendLine($"{nome}_len equ ($ - {nome}) / 2");
                    //this.pilha.Push(nome);
                    this.AdicionarAPilhaString(token);
                    this.pilha.Push(token);
                    break;
                case (int)CodigosEnum.CODIGO_LITERAL_INT or (int)CodigosEnum.CODIGO_LITERAL_DECIMAL or 
                     (int) CodigosEnum.CODIGO_LITERAL_BOOL:


                    this.declaracaoVariaveis.AppendLine($"{nome} {GetTypeAssemby()} {token.GetLexeme()}");
                    break;

                default:
                    break;

            }
            //this.tipo = null;

            
        }


        private void AdicionarConstantes(Token token)
        {
            var nome = "constante_"  + (this.constantes.Count + 1);
            this.constantes.Add(nome);
            this.pilha.Push(token);

        }

        private void AdicionarIdentificador(Token token)
        {
            if (isInFunctionDeclaration) this.expressaoInicializacao.Add(token);
            this.identificadores.Add(token.GetLexeme());

            if(this.tipo is not null)this.tiposVariaveis.TryAdd(token.GetLexeme(), this.tipo.Value);


            if (this.isPrintOperation)
            {

                var temTipo = this.tiposVariaveis.TryGetValue(token.GetLexeme(), out var tipoAux);

                switch (tipoAux)
                {
                    case Tipos.INT:

                        this.AdicionarCodigo($"mov ecx, [rel {token.GetLexeme()}]");
                        this.AdicionarCodigo(@"call ConverterInt");
                        break;

                    case Tipos.DECIMAL:
                        this.AdicionarCodigo($"mov eax, [rel {token.GetLexeme()}]");
                        this.AdicionarCodigo(@"movd xmm0, eax
                                               call ConverterDecimal");
                        break;

                    default:
                        this.AdicionarAPilhaString(token, false);
                        break;
                
                }


            }

        }

      


        private void AdicionarAPilhaString(Token token, bool isConstant = true)
        {

            if (isConstant)
            {
                this.AdicionarCodigo($"lea rax, [{constantes.Last()}] \n" +
    $"push rax\n" +
    $"mov eax, {constantes.Last()}_len\n" +
    $"push rax");
                return;
            }

            this.AdicionarCodigo($"lea rax, [rel {token.GetLexeme()}]\n" +
                $"push rax\n" +
                $"mov eax, [rel chars_read]\n" +
                $"push rax");

            





        }


        private void ExecuteInicio()
        {
            this.declaracaoVariaveis.AppendLine("section .data");
            this.declaracaoVariaveis.AppendLine("mascara_sinal dd 0x80000000");
            this.declaracaoVariaveis.AppendLine(@"fator_100       dd 100.0
meio_decimal    dd 0.5
limite_decimal  dd 9.0e16
erro_decimal    dw 69, 82, 82, 79, 0 ; ERRO");
            this.CriarCodigoBase();
            this.declaracaoFuncoes.AppendLine("default rel");
            this.declaracaoFuncoes.AppendLine("extern ExitProcess");
            this.AdicionarFuncao("extern GetStdHandle");
            this.AdicionarFuncao("extern WriteConsoleW");
            this.AdicionarFuncao("extern ReadConsoleW");
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

            this.code.AppendLine(" mov ecx, -10");
            this.code.AppendLine(" call GetStdHandle");
            this.code.AppendLine("mov [console_input], rax");

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
            this.sessaoEspecial.AppendLine("console_input resq 1");
            this.sessaoEspecial.AppendLine(@"buffer_input resw 257
                                            chars_read   resd 1");
            this.sessaoEspecial.AppendLine("buffer_print_decimal resw 32");
            this.sessaoEspecial.AppendLine("buffer_print_int resw 12");
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

            this.funcoesPersonalizadas.AppendLine(@"ConverterInt:
    ; Guarda o endereço de retorno
    pop r11

    ; Preserva registradores não voláteis
    push rbx
    push rsi

    ; Recebe INT32 em ECX
    movsxd rax, ecx

    xor ebx, ebx

    ; Verifica sinal
    test rax, rax
    jns .positivo

    mov ebx, 1
    neg rax

.positivo:
    ; Começa pela última posição do buffer
    lea rsi, [rel buffer_print_int + 22]

    ; Terminador UTF-16
    mov word [rsi], 0

    mov ecx, 10

.converter:
    xor edx, edx
    div rcx

    add dl, '0'

    ; Cada caractere UTF-16 ocupa 2 bytes
    sub rsi, 2
    mov [rsi], dx

    test rax, rax
    jnz .converter

    ; Adiciona sinal negativo
    test ebx, ebx
    jz .fim

    sub rsi, 2
    mov word [rsi], '-'

.fim:
    ; Calcula quantidade de caracteres
    lea rdx, [rel buffer_print_int + 22]
    sub rdx, rsi
    shr rdx, 1

    ; Guarda endereço do texto
    mov r8, rsi

    ; Restaura registradores
    pop rsi
    pop rbx

    ; Retorna endereço e tamanho na pilha
    push r8
    push rdx
    push r11
    ret");

            this.funcoesPersonalizadas.AppendLine(@"ConverterDecimal:
    ; Guarda o endereço de retorno
    pop r11

    ; Recupera o sinal do float
    movd eax, xmm0
    mov r9d, eax
    shr r9d, 31

    ; Remove o sinal para trabalhar com valor positivo
    and eax, 0x7FFFFFFF
    movd xmm0, eax

    ; Verifica se o valor está dentro do limite suportado
    ; Também rejeita NaN e infinito
    ucomiss xmm0, [rel limite_decimal]
    jp .erro
    ja .erro

    ; Multiplica por 100 para trabalhar com duas casas
    mulss xmm0, [rel fator_100]

    ; Arredondamento
    addss xmm0, [rel meio_decimal]

    ; Converte para inteiro de 64 bits
    cvttss2si rax, xmm0

    ; Divide por 100:
    ; RAX = parte inteira
    ; RDX = parte decimal (0 a 99)
    xor edx, edx
    mov ecx, 100
    div rcx

    ; Guarda parte inteira
    mov r10, rax

    ; Inicia buffer pelo final
    lea r8, [rel buffer_print_decimal + 62]

    ; Terminador UTF-16
    mov word [r8], 0

    ; Converte as duas casas decimais
    mov eax, edx
    xor edx, edx
    mov ecx, 10
    div ecx

    ; Última casa decimal
    add dl, '0'
    sub r8, 2
    mov [r8], dx

    ; Primeira casa decimal
    add al, '0'
    sub r8, 2
    mov [r8], ax

    ; Separador decimal
    sub r8, 2
    mov word [r8], '.'

    ; Converte parte inteira
    mov rax, r10
    mov ecx, 10

.inteiro:
    xor edx, edx
    div rcx

    add dl, '0'
    sub r8, 2
    mov [r8], dx

    test rax, rax
    jnz .inteiro

    ; Adiciona sinal negativo
    test r9d, r9d
    jz .finalizar

    sub r8, 2
    mov word [r8], '-'

.finalizar:
    ; Calcula quantidade de caracteres
    lea rdx, [rel buffer_print_decimal + 62]
    sub rdx, r8
    shr rdx, 1

    jmp .retornar

.erro:
    ; Exibe ERRO para valores inválidos ou fora do limite
    lea r8, [rel erro_decimal]
    mov edx, 4

.retornar:
    ; Retorna endereço e tamanho na pilha
    push r8
    push rdx

    ; Restaura endereço de retorno
    push r11
    ret");

            this.funcoesPersonalizadas.AppendLine(@"ReadInt:
    push rsi

    sub rsp, 48
    mov [rsp + 32], rcx
    lea rsi, [rel buffer_input]
    mov rcx, rsi
    call Read

    xor eax, eax
    xor ecx, ecx
    xor r8d, r8d

    mov edx, [rel chars_read]

    test edx, edx
    jz .int_erro

    cmp word [rsi], '-'
    jne .int_loop

    mov r8d, 1
    inc ecx

.int_loop:
    cmp ecx, edx
    jge .int_fim

    movzx r9d, word [rsi + rcx*2]

    cmp r9d, '0'
    jl .int_erro

    cmp r9d, '9'
    jg .int_erro

    sub r9d, '0'

    imul eax, eax, 10
    jo .int_erro

    add eax, r9d
    jo .int_erro

    inc ecx
    jmp .int_loop

.int_fim:
    test r8d, r8d
    jz .int_gravar

    neg eax

.int_gravar:
    mov rcx, [rsp + 32]
    mov [rcx], eax

    add rsp, 48
    pop rsi

    ret

.int_erro:
    xor eax, eax
    jmp .int_gravar
                                                     ");

            this.funcoesPersonalizadas.AppendLine(@"    
    ReadDecimal:
  push rbx
  push rsi
    sub rsp, 56

lea rsi, [rel buffer_input]

; Preserva o endereço da variável decimal que receberá o resultado
mov [rsp + 32], rcx

; Passa o buffer de entrada para Read
mov rcx, rsi
call Read

    xor eax, eax            ; Acumulador
    xor ecx, ecx            ; Índice
    xor r8d, r8d            ; Casas decimais
    xor r9d, r9d            ; Sinal negativo
    xor r10d, r10d          ; Encontrou ponto?
    xor r11d, r11d          ; Quantidade de dígitos

    mov edx, [rel chars_read]

    test edx, edx
    jz .erro

    ; Verifica sinal negativo
    cmp word [rsi], '-'
    jne .loop

    mov r9d, 1
    inc ecx

.loop:
    cmp ecx, edx
    jge .fim

    movzx ebx, word [rsi + rcx*2]

    ; Verifica ponto decimal
    cmp ebx, '.'
    je .ponto

    ; Verifica se é dígito
    cmp ebx, '0'
    jl .erro

    cmp ebx, '9'
    jg .erro

    sub ebx, '0'

    ; Acumulador = acumulador * 10 + dígito
    imul eax, eax, 10
    jo .erro

    add eax, ebx
    jo .erro

    inc r11d

    ; Conta casas decimais se já passou do ponto
    cmp r10d, 1
    jne .proximo

    inc r8d

.proximo:
    inc ecx
    jmp .loop

.ponto:
    ; Não permite dois pontos
    cmp r10d, 1
    je .erro

    ; Não permite ponto sem dígitos antes
    test r11d, r11d
    jz .erro

    mov r10d, 1
    inc ecx
    jmp .loop

.fim:
    ; Exige pelo menos um dígito
    test r11d, r11d
    jz .erro

    ; Não permite ponto sem casas decimais
    cmp r10d, 1
    jne .converter

    test r8d, r8d
    jz .erro

.converter:
    ; Converte acumulador para float
    cvtsi2ss xmm0, eax

    ; Calcula potência de 10
    mov eax, 1

.fator:
    test r8d, r8d
    jz .dividir

    imul eax, eax, 10
    jo .erro

    dec r8d
    jmp .fator

.dividir:
    cvtsi2ss xmm1, eax
    divss xmm0, xmm1

    ; Aplica sinal negativo
    test r9d, r9d
    jz .gravar

    mov eax, 0x80000000
    movd xmm1, eax
    xorps xmm0, xmm1

.gravar:
    mov rcx, [rsp + 32]
    movss [rcx], xmm0

    add rsp, 56
    pop rbx
    pop rsi
    ret

.erro:
    xorps xmm0, xmm0
    jmp .gravar");

            this.funcoesPersonalizadas.AppendLine(@" Read:
    push rsi
    sub rsp, 48

    ; RCX recebe o endereço da variável
    mov rsi, rcx

    mov rcx, [rel console_input]
    mov rdx, rsi
    mov r8d, 256
    lea r9, [rel chars_read]
    mov qword [rsp + 32], 0

    call ReadConsoleW

    test eax, eax
    jz .erro

    mov eax, [rel chars_read]

    ; Remove LF
    test eax, eax
    jz .fim

    cmp word [rsi + rax*2 - 2], 10
    jne .verifica_cr

    dec eax

.verifica_cr:
    test eax, eax
    jz .fim

    cmp word [rsi + rax*2 - 2], 13
    jne .fim

    dec eax

.fim:
    mov [rel chars_read], eax
    mov word [rsi + rax*2], 0

    add rsp, 48
    pop rsi
    ret

.erro:
    mov dword [rel chars_read], 0
    add rsp, 48
    pop rsi
    ret");

        }

        private void AdicionarFuncaoPersonalizada(string codigo)
        {
            this.funcoesPersonalizadas.AppendLine(codigo);
        }

        private void AdicionarSessaoEspecial(string codigo)
        {
            this.sessaoEspecial.AppendLine(codigo);
        }



        private void AdicionarTipo(Token token)
        {
            this.tipo = Enum.Parse<Tipos>(token.GetLexeme().ToUpper());
        }


        
        private void ExecutaRead(Token token)
        {

            if(this.pilha.Count > 0)
            {
                this.ExecutaSaida(token);
            }


            var identificador = identificadores.FirstOrDefault();

            this.AdicionarCodigo($"lea rcx, [rel {identificador}]");

            switch (tipo)
            {
                case Tipos.BOOL:

                    break;

                case Tipos.STRING:
                    this.AdicionarCodigo("call Read");
                    break;
                case Tipos.INT:
                    this.AdicionarCodigo("call ReadInt");
                    break;
                case Tipos.DECIMAL:
                    this.AdicionarCodigo("call ReadDecimal");
                    break;
            }



            //tipo = null;



        }

        private void FinalizarDeclaracao(Token token)
        {

            var tipo = this.GetTypeAssemby();

            foreach (var identificador in identificadores)
            {
                if (this.isInFunctionDeclaration)
                {
                    if(!this.variaveis.TryGetValue(identificador, out _))
                    {

                        
                        if(this.tipo != Tipos.STRING )
                        {

                            this.declaracaoVariaveis.Append($"{identificador} {tipo} ");

                            var valor = this.expressaoInicializacao.Count == 1 ? this.expressaoInicializacao.First().GetLexeme()
                                                                               : "0";

                            this.declaracaoVariaveis.Append(valor + "\n");

                        }
                        else
                        {
                            this.AdicionarSessaoEspecial($"{identificador} resw 257");
                            this.AdicionarSessaoEspecial($"{identificador}_len resd 1");
                            //this.declaracaoVariaveis.Append($"{identificador}_len equ ($ - {identificador}) / 2");

                        }


                        this.variaveis.Add(identificador);
                    }


                    if(this.expressaoInicializacao.Count > 1)
                    {
                        
                        var codigoGerado = ExpressaoService.CompilarExpressao(new VariavelService(this.expressaoInicializacao).AnalisarExpressao());
                        
                        this.AdicionarCodigo(codigoGerado);
                        this.AdicionarCodigo($"mov [rel {identificador}], eax");
                    }


                }
                else
                {
                    if (!this.variaveis.TryGetValue(identificador, out _))
                    {

                        if(this.tipo != Tipos.STRING )
                        {

                            this.declaracaoVariaveis.Append($"{identificador} {tipo} ");

                            var valor = "0";

                            this.declaracaoVariaveis.Append(valor + "\n");
                        }
                        else
                        {
                            this.AdicionarSessaoEspecial($"{identificador} resw 257");
                            this.AdicionarSessaoEspecial($"{identificador}_len resd 1");


                        }
                        this.variaveis.Add(identificador);


                    }
                }
            }

            identificadores.Clear();
            this.expressaoInicializacao.Clear();
            this.isInFunctionDeclaration = false;
            this.isPrintOperation = false;
            this.tipo = null;
        }



        private void AdicionarExpressaoDeclaracao(Token token)
        {
            this.expressaoInicializacao.Add(token);
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



        private string GetTypeAssemby()
        {
            if (tipo == null) return "";


            switch (tipo) {
                case Tipos.BOOL:
                    return "db";
                case Tipos.DECIMAL:
                    return "dq";
                case Tipos.INT:
                    return "dd";
                case Tipos.STRING:
                    return "db";
                default:
                    return "";
            
            }

        }

    }
}
