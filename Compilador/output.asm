default rel
extern ExitProcess
extern GetStdHandle
extern WriteConsoleW
extern ReadConsoleW
section .data
mascara_sinal dd 0x80000000
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
                                                    

