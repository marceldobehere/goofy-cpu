class Program
{
    public struct Instruction
    {
        public string Name;
        public byte Opcode;
        public List<long> Steps;
        public Instruction(string name, byte opcode, List<long> steps)
        {
            this.Name = name;
            this.Opcode = opcode;
            this.Steps = steps;

            if (Steps.Count > 0 && Steps[Steps.Count - 1] != DONE)
                Steps.Add(DONE);
        }
    }

    public const long DONE = 0b0000_0000_0000_0000;

    public const long PUT_REG_IOP0_LO_BUS = 0b0000_0000_0000_0001;
    public const long PUT_REG_IOP0_HI_BUS = 0b0000_0000_0000_0010;

    public const long STR_REG_IOP0_LO_BUS = 0b0000_0000_0000_0011;
    public const long STR_REG_IOP0_HI_BUS = 0b0000_0000_0000_0100;

    public const long PUT_VAL_IOP0_BUS = 0b0000_0000_0000_0101;
    public const long PUT_VAL_IOP1_BUS = 0b0000_0000_0000_0110;

    public const long STR_REG_0__BUS = 0b0000_0000_0000_0111;
    public const long STR_REG_1__BUS = 0b0000_0000_0000_1000;
    public const long STR_REG_2__BUS = 0b0000_0000_0000_1001;
    public const long STR_REG_3__BUS = 0b0000_0000_0000_1010;

    public static void Main(string[] args)
    {
        List<Instruction> instructions = new List<Instruction>();


        #region > REGISTER MANIPULATION (0000.XXXX)

        //> MOVE
        //  0000.0000 REG1.REG2 0000.0000 (REG2 -> REG1)
        // REG1 IS HI, REG2 IS LO
        instructions.Add(new Instruction("MOVE", 0b0000_0000, new List<long> {
            PUT_REG_IOP0_LO_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> MOVEI
        //  0000.0001 REG1.0000 AAAA.AAAA (AAAA -> REG1)
        // REG1 IS HI
        instructions.Add(new Instruction("MOVEI", 0b0000_0001, new List<long> {
            PUT_VAL_IOP1_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> LDA
        //  0000.0010 AAAA.AAAA AAAA.AAAA (AAAA -> REGA)
        instructions.Add(new Instruction("LDA", 0b0000_0010, new List<long> {
            PUT_VAL_IOP0_BUS | STR_REG_0__BUS,
            PUT_VAL_IOP1_BUS | STR_REG_1__BUS
        }));

        //> LDB
        //  0000.0011 AAAA.AAAA AAAA.AAAA (AAAA -> REGB)
        instructions.Add(new Instruction("LDB", 0b0000_0011, new List<long>
        {
            PUT_VAL_IOP0_BUS | STR_REG_2__BUS,
            PUT_VAL_IOP1_BUS | STR_REG_3__BUS
        }));




        #endregion




        // List Instructions
        Console.WriteLine("Instructions: ");
        foreach (var instruction in instructions)
        {
            Console.WriteLine($"{instruction.Name} - {instruction.Opcode}");
            foreach (var step in instruction.Steps)
                Console.WriteLine($"  {Convert.ToString(step, 2).PadLeft(16, '0')}");
            Console.WriteLine(  );
        }

    }
}



/*
REGISTERS (16)
RXX

REGA = (REG0, REG1)
REGB = (REG2, REG3)

RIP (16 bit)

FLAG (3 bit) 
 0 - EQ
 1 - OVERFLOW
 2 - HLT

INSTRUCT  REG1 REG2 OP2
INSTRUCT  OP___HIGH OP____LOW
0000.0000 0000.0000 0000.0000


> REGISTER MANIPULATION (0000.XXXX)
 > MOVE
   0000.0000 REG1.REG2 0000.0000 (REG2 -> REG1)   
 > MOVEI
   0000.0001 REG1.0000 AAAA.AAAA (AAAA -> REG1)
 > LDA
   0000.0010 AAAA.AAAA AAAA.AAAA (AAAA -> REGA)
 > LDB
   0000.0011 AAAA.AAAA AAAA.AAAA (AAAA -> REGB)
 > RES FLAG
   0000.0100 0000.0000 0000.0000 (0    -> FLAG)


> MEMORY MANIPULATION (0001.XXXX)
 > READA
   0001.0000 REG1.0000 0000.0000 (RAM[REGA] -> REG1)
 > READB
   0001.0001 REG1.0000 0000.0000 (RAM[REGB] -> REG1)
 > WRITEA
   0001.0010 REG1.0000 0000.0000 (REG1 -> RAM[REGA])
 > WRITEB
   0001.0011 REG1.0000 0000.0000 (REG1 -> RAM[REGB])

> IO (0010.XXXX)
 > IOREADA
   0010.0000 REG1.0000 0000.0000 (IO[REGA] -> REG1)
 > IOREADB
   0010.0001 REG1.0000 0000.0000 (IO[REGB] -> REG1)
 > IOWRITEA
   0010.0010 REG1.0000 0000.0000 (REG1 -> IO[REGA])
 > IOWRITEB
   0010.0011 REG1.0000 0000.0000 (REG1 -> IO[REGB])


> MATH (0011.XXXX)
 > ADD
   0011.0000 REG1.REG2 0000.0000 (REG1 + REG2) -> REG1, FLAG(OVERFLOW)
 > ADDOV
   0011.0001 REG1.REG2 0000.0000 (REG1 + REG2) -> REG1 (+1 IF FLAG(OVERFLOW))
 > SUB
   0011.0010 REG1.REG2 0000.0000 (REG1 - REG2) -> REG1
 > SUBOV
   0011.0011 REG1.REG2 0000.0000 (REG1 - REG2) -> REG1
 > AND
   0011.0100 REG1.REG2 0000.0000 (REG1 & REG2) -> REG1
 > OR
   0011.0101 REG1.REG2 0000.0000 (REG1 | REG2) -> REG1
 > NOT
   0011.0110 REG1.REG2 0000.0000 (   ~REG2   ) -> REG1
 > CMP
   0011.0111 REG1.REG2 0000.0000 (REG1 ? REG2) -> FLAG(EQ)

 > ADDI
   0011.1000 REG1.0000 AAAA.AAAA (REG1 + AAAA) -> REG1
 > ADDIOV
   0011.1001 REG1.0000 AAAA.AAAA (REG1 + AAAA) -> REG1, FLAG(OVERFLOW)
 > SUBI
   0011.1010 REG1.0000 AAAA.AAAA (REG1 - AAAA) -> REG1
 > SUBIOV
   0011.1011 REG1.0000 AAAA.AAAA (REG1 - AAAA) -> REG1
 > ANDI
   0011.1100 REG1.0000 AAAA.AAAA (REG1 & AAAA) -> REG1
 > ORI
   0011.1101 REG1.0000 AAAA.AAAA (REG1 | AAAA) -> REG1
 > NOTI
   ????.???? ????.???? ????.???? (???????????) -> ????
 > CMPI
   0011.1111 REG1.0000 AAAA.AAAA (REG1 ? AAAA) -> FLAG(EQ)



> CONTROL FLOW (0100.XXXX)
 > JEQ
   0100.0000 AAAA.AAAA AAAA.AAAA
 > JNEQ
   0100.0001 AAAA.AAAA AAAA.AAAA
 > HLT
   0100.0010 0000.0000 0000.0000 
*/