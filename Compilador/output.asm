default rel
extern ExitProcess
extern GetStdHandle
extern WriteConsoleW
section .data
constante_01 db 111, 0, 105, 0, 105, 0, 105, 0, 105, 0, 13, 0, 10, 0, 0
constante_01_len equ ($ - constante_01) / 2
constante_11 db 79, 0, 108, 0, 225, 0, 32, 0, 109, 0, 117, 0, 110, 0, 100, 0, 111, 0, 13, 0, 10, 0, 0
constante_11_len equ ($ - constante_11) / 2
constante_21 db 84, 0, 101, 0, 115, 0, 116, 0, 101, 0, 32, 0, 116, 0, 105, 0, 13, 0, 10, 0, 0
constante_21_len equ ($ - constante_21) / 2

section .bss
written resd 1
console_handle resq 1

section .text
global main
main:
    sub  rsp, 40
    mov  ecx, -11
    call GetStdHandle
    mov  [console_handle], rax
lea rdx, [constante_01]
mov r8d, constante_01_len
call Print
lea rdx, [constante_11]
mov r8d, constante_11_len
call Print
lea rdx, [constante_21]
mov r8d, constante_21_len
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

