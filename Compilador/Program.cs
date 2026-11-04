


using System.ComponentModel;
using System.Text;
using Compilador;

String conteudo = string.Empty;

if (args.Length == 0)
{
    Console.WriteLine("Digite o código a ser compilado: ");
     conteudo = Console.ReadLine();

}
else
{
    conteudo = await FileService.LerArquivo(args[0]);
}
Compilador.Compilador compilador = new Compilador.Compilador();
compilador.Compilar(conteudo);


/*
 * 
 * default rel                     ; [FERR] na prática obrigatório (explicado abaixo)

extern GetStdHandle             ; [FERR] obrigatório para cada função externa usada;
extern WriteConsoleA            ;        os nomes são exatos e diferenciam maiúsculas
extern ExitProcess

section .data                   ; [LIVRE] onde ficam os dados (.data, .rdata...)
msg     db "Ola do Windows 11!", 13, 10   ; [LIVRE] texto, nome do rótulo, o 13,10 (CRLF)
msg_len equ $ - msg             ; [LIVRE] só uma conveniência para calcular o tamanho

section .bss                    ; [LIVRE] poderia estar em .data também
written resd 1                  ; [LIVRE] nome e local; para WriteConsoleA é até opcional

section .text                   ; [FERR] o código precisa estar numa seção executável
global main                     ; [FERR] o ponto de entrada precisa ser exportado
main:                           ; [LIVRE] o nome, desde que bata com /entry:



    mov  rcx, rax               ; [ABI] 1º arg → rcx
    lea  rdx, [msg]             ; [ABI] 2º arg → rdx
    mov  r8d, msg_len           ; [ABI] 3º arg → r8
    lea  r9,  [written]         ; [ABI] 4º arg → r9
    mov  qword [rsp+32], 0      ; [ABI] 5º arg → [rsp+32]; a API exige NULL aqui
    call WriteConsoleA

    xor  ecx, ecx               ; [ABI] 1º arg em rcx; o valor 0 é [LIVRE]
    call ExitProcess            ; recomendado (explicado abaixo)
*/