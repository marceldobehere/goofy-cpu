#include "./common.asm"


goofy_loop:
    .loop:
		addi r0, 1
		cmpi r0, 0x22
		jneq .loop
	hlt