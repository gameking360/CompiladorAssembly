default rel
extern ExitProcess
extern GetStdHandle
extern WriteConsoleW
extern ReadConsoleW
section .data
mascara_sinal dd 0x80000000
fator_100       dd 100.0
meio_decimal    dd 0.5
limite_decimal  dd 9.0e16
erro_decimal    dw 69, 82, 82, 79, 0 ; ERRO
a dd 0
constante_1 db 68, 0, 105, 0, 103, 0, 105, 0, 116, 0, 101, 0, 32, 0, 117, 0, 109, 0, 32, 0, 118, 0, 97, 0, 108, 0, 111, 0, 114, 0, 0
constante_1_len equ ($ - constante_1) / 2
b dd 0
c dd 0

section .bss
written resd 1
console_handle resq 1
console_input resq 1
buffer_input resw 257
                                            chars_read   resd 1
buffer_print_decimal resw 32
buffer_print_int resw 12

section .text
global main
main:
    sub  rsp, 40
    mov  ecx, -11
    call GetStdHandle
    mov  [console_handle], rax
 mov ecx, -10
 call GetStdHandle
mov [console_input], rax
mov eax, 20
push rax
mov eax, 5
mov ecx, eax
pop rax
add eax, ecx

mov [rel a], eax
lea rax, [constante_1] 
push rax
mov eax, constante_1_len
push rax
pop rax
                                    mov r8d, eax
                                    pop rdx
                                    call Print
lea rcx, [rel b]
call ReadInt
mov eax, [rel a]
push rax
mov eax, [rel b]
mov ecx, eax
pop rax
add eax, ecx

mov [rel c], eax
mov eax, [rel a]
push rax
mov eax, [rel b]
mov ecx, eax
pop rax
add eax, ecx

mov [rel a], eax
mov eax, [rel a]
push rax
mov eax, [rel b]
mov ecx, eax
pop rax
add eax, ecx

mov [rel b], eax
mov ecx, [rel c]
call ConverterInt
pop rax
                                    mov r8d, eax
                                    pop rdx
                                    call Print

 xor ecx, ecx
    call ExitProcess
Print: 
 sub rsp, 40
 mov rcx, [console_handle]
    lea r9, [written]
    mov qword[rsp + 32], 0
    call WriteConsoleW
    add rsp, 40
    ret
ConverterInt:
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
    ret
ConverterDecimal:
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
    ret
ReadInt:
    push rsi

    sub rsp, 48
    mov [rsp + 32], rcx
    lea rsi, [rel buffer_input]

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
                                                     
    
    ReadDecimal:
  push rbx
  push rsi
    sub rsp, 56

    lea rsi, [rel buffer_input]

    mov [rsp + 32], rcx
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
    jmp .gravar
 Read:
    push rsi
    sub rsp, 48
    lea rsi, [rel buffer_input]

    mov rcx, [rel console_input]
    lea rdx, [rel buffer_input]
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
    ret
                                                    

