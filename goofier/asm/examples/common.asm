#ruledef
{
	nop                  => 0x000000`24
	
	move  r{a}, r{b}     => 0x00`8 @ a`4 @ b`4 @ 0x00`8
	movei r{a}, {value}  => 0x01`8 @ a`4 @ 0x0`4 @ value`8
	lda {value}          => 0x02`8 @ value`16
	ldb {value}          => 0x03`8 @ value`16
	res_flag             => 0x04`8 @ 0x0000`16
	
	reada r{a}           => 0x10`8 @ a`4 @ 0x0`4 @ 0x00`8
	readb r{a}           => 0x11`8 @ a`4 @ 0x0`4 @ 0x00`8
	writea r{a}          => 0x12`8 @ a`4 @ 0x0`4 @ 0x00`8
	writeb r{a}          => 0x13`8 @ a`4 @ 0x0`4 @ 0x00`8
	
	ioreada              => 0x20`8 @ a`4 @ 0x0`4 @ 0x00`8
	ioreadb              => 0x21`8 @ a`4 @ 0x0`4 @ 0x00`8
	iowritea             => 0x22`8 @ a`4 @ 0x0`4 @ 0x00`8
	iowriteb             => 0x23`8 @ a`4 @ 0x0`4 @ 0x00`8
	
	add  r{a}, r{b}      => 0x30`8 @ a`4 @ b`4 @ 0x00`8
	addov r{a}, r{b}     => 0x31`8 @ a`4 @ b`4 @ 0x00`8
	sub r{a}, r{b}       => 0x32`8 @ a`4 @ b`4 @ 0x00`8
	subov r{a}, r{b}     => 0x33`8 @ a`4 @ b`4 @ 0x00`8
	and r{a}, r{b}       => 0x34`8 @ a`4 @ b`4 @ 0x00`8
	or r{a}, r{b}        => 0x35`8 @ a`4 @ b`4 @ 0x00`8
	not r{a}, r{b}       => 0x36`8 @ a`4 @ b`4 @ 0x00`8
	cmp r{a}, r{b}       => 0x37`8 @ a`4 @ b`4 @ 0x00`8
	
	
	addi r{a}, {value}   => 0x38`8 @ a`4 @ 0x0`4 @ value`8
	addiov r{a}, {value} => 0x39`8 @ a`4 @ 0x0`4 @ value`8
	subi r{a}, {value}   => 0x3a`8 @ a`4 @ 0x0`4 @ value`8
	subiov r{a}, {value} => 0x3b`8 @ a`4 @ 0x0`4 @ value`8
	andi r{a}, {value}   => 0x3c`8 @ a`4 @ 0x0`4 @ value`8
	ori r{a}, {value}    => 0x3d`8 @ a`4 @ 0x0`4 @ value`8
	cmpi r{a}, {value}   => 0x3f`8 @ a`4 @ 0x0`4 @ value`8
	
	jeq  {addr}          => 0x40`8 @ addr`16
	jneq {addr}          => 0x41`8 @ addr`16
	jmp  {addr}          => 0x42`8 @ addr`16
	hlt                  => 0x43`8 @ 0x0000`16
}