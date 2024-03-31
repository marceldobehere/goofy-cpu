# Goofy CPU (GOOFIER EDITION)
A sillier version of [my goofy cpu](../) which runs microcode directly instead of instructions. 

This leads to a higher performance but also has way larger programs.



## Microcode / Instruction Set
The instructions can be found in the `plan.txt` file from the original cpu.

The cpu executes microcode directly which is generated using a c# project that turns the list of instructions into the corresponding microcode:

![This is an example of the code that increments the first register until it reaches 0xFF](./imgs/ins.PNG)

This program can be found in the `Goofy Microcode gen` folder.

There is also the compiler from binary asm to microcode, which can be found in `Goofy Microcode comp`.

## Developing

You can now write assembly and compile it for the cpu. The stuff can be found in the `asm` folder. 
You can compile an asm file (like `examples/inc_hlt.asm`) using the `compile.bat` file and then use the generated output and load that into the microcode rom.

NOTE: You need to include the `common.asm` file for customasm to work properly!

NOTE: This currently windows only (kinda?) due to the silly compile setup with the batch file.

### IMPORTANT NOTE
AAAAAA Currently JUMPS WILL JUMP TO THE WRONG ADDRESS (IN BOTH COMPILING DIRECTLY OR INDIRECTLY FROM A BIN)!!! (Will fix soon)

## Images/GIFs
![An image of the full cpu](./imgs/cpu.png)

![The cpu running a test program that adds 1 to the first register until it reaches 0xFF](./imgs/cpu%20test%202.gif)
