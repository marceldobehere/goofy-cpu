class Program
{
    public struct Instruction
    {
        public string Name;
        public byte Opcode;
        public List<ulong> Steps;
        public Instruction(string name, byte opcode, List<ulong> steps)
        {
            this.Name = name;
            this.Opcode = opcode;
            this.Steps = steps;

            if (Steps.Count == 0 || Steps[Steps.Count - 1] != DONE)
                Steps.Add(DONE);
        }
    }

    public const ulong DONE = (ulong)1 << 63;

    public const ulong PUT_REG_IOP0_LO_BUS = 1 << 0;
    public const ulong PUT_REG_IOP0_HI_BUS = 1 << 1;

    public const ulong STR_REG_IOP0_LO_BUS = 1 << 2;
    public const ulong STR_REG_IOP0_HI_BUS = 1 << 3;

    public const ulong PUT_VAL_IOP0_BUS = 1 << 4;
    public const ulong PUT_VAL_IOP1_BUS = 1 << 5;

    public const ulong STR_REG_0_BUS = 1 << 6;
    public const ulong STR_REG_1_BUS = 1 << 7;
    public const ulong STR_REG_2_BUS = 1 << 8;
    public const ulong STR_REG_3_BUS = 1 << 9;

    public const ulong RAM_WRITE_BUS_A = 1 << 10;
    public const ulong RAM_WRITE_BUS_B = 1 << 11;

    public const ulong RAM_READ_BUS_A = 1 << 12;
    public const ulong RAM_READ_BUS_B = 1 << 13;

    public const ulong IO_WRITE_BUS_A = 1 << 14;
    public const ulong IO_WRITE_BUS_B = 1 << 15;

    public const ulong IO_READ_BUS_A = 1 << 16;
    public const ulong IO_READ_BUS_B = 1 << 17;

    public const ulong STR_ALU_REG_0_BUS = 1 << 18;
    public const ulong STR_ALU_REG_1_BUS = 1 << 19;

    public const ulong ALU_ADD = 1 << 20;
    public const ulong ALU_ADD_OV = 1 << 21;
    public const ulong ALU_SUB = 1 << 22;
    public const ulong ALU_SUB_OV = 1 << 23;
    public const ulong ALU_AND = 1 << 24;
    public const ulong ALU_OR = 1 << 25;
    public const ulong ALU_NOT = 1 << 26;
    public const ulong ALU_CMP = 1 << 27;
    public const ulong ALU_FLAG_RESET = 1 << 28;
    public const ulong PUT_ALU_RES_BUS = 1 << 29;

    public const ulong JEQ = (ulong)1 << 30;
    public const ulong JNEQ = (ulong)1 << 31;
    public const ulong HLT = (ulong)1 << 32;
    public const ulong JMP = (ulong)1 << 33;

    public static void Main(string[] args)
    {
        Console.WriteLine(DONE);
        List<Instruction> instructions = new List<Instruction>();


        #region > REGISTER MANIPULATION (0000.XXXX)
        //> MOVE
        //  0000.0000 REG1.REG2 0000.0000 (REG2 -> REG1)
        // REG1 IS HI, REG2 IS LO
        instructions.Add(new Instruction("MOVE", 0b0000_0000, new List<ulong> {
            PUT_REG_IOP0_LO_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> MOVEI
        //  0000.0001 REG1.0000 AAAA.AAAA (AAAA -> REG1)
        // REG1 IS HI
        instructions.Add(new Instruction("MOVEI", 0b0000_0001, new List<ulong> {
            PUT_VAL_IOP1_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> LDA
        //  0000.0010 AAAA.AAAA AAAA.AAAA (AAAA -> REGA)
        instructions.Add(new Instruction("LDA", 0b0000_0010, new List<ulong> {
            PUT_VAL_IOP0_BUS | STR_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_REG_1_BUS
        }));

        //> LDB
        //  0000.0011 AAAA.AAAA AAAA.AAAA (AAAA -> REGB)
        instructions.Add(new Instruction("LDB", 0b0000_0011, new List<ulong>
        {
            PUT_VAL_IOP0_BUS | STR_REG_2_BUS,
            PUT_VAL_IOP1_BUS | STR_REG_3_BUS
        }));

        //> RES FLAG
        //  0000.0100 0000.0000 0000.0000 (0    -> FLAG)
        instructions.Add(new Instruction("RES FLAG", 0b0000_0100, new List<ulong>
        {
            ALU_FLAG_RESET
        }));
        #endregion

        #region > MEMORY MANIPULATION (0001.XXXX)
        //> READA
        //  0001.0000 REG1.0000 0000.0000 (RAM[REGA] -> REG1)
        instructions.Add(new Instruction("READA", 0b0001_0000, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | RAM_READ_BUS_A
        }));

        //> READB
        //  0001.0001 REG1.0000 0000.0000 (RAM[REGB] -> REG1)
        instructions.Add(new Instruction("READB", 0b0001_0001, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | RAM_READ_BUS_B
        }));

        //> WRITEA
        //  0001.0010 REG1.0000 0000.0000 (REG1 -> RAM[REGA])
        instructions.Add(new Instruction("WRITEA", 0b0001_0010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | RAM_WRITE_BUS_A
        }));

        //> WRITEB
        //  0001.0011 REG1.0000 0000.0000 (REG1 -> RAM[REGB])
        instructions.Add(new Instruction("WRITEB", 0b0001_0011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | RAM_WRITE_BUS_B
        }));
        #endregion

        #region > IO (0010.XXXX)
        //> IOREADA
        //  0010.0000 REG1.0000 0000.0000 (IO[REGA] -> REG1)
        instructions.Add(new Instruction("IOREADA", 0b0010_0000, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | IO_READ_BUS_A
        }));

        //> IOREADB
        //  0010.0001 REG1.0000 0000.0000 (IO[REGB] -> REG1)
        instructions.Add(new Instruction("IOREADB", 0b0010_0001, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | IO_READ_BUS_B
        }));

        //> IOWRITEA
        //  0010.0010 REG1.0000 0000.0000 (REG1 -> IO[REGA])
        instructions.Add(new Instruction("IOWRITEA", 0b0010_0010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | IO_WRITE_BUS_A
        }));

        //> IOWRITEB
        //  0010.0011 REG1.0000 0000.0000 (REG1 -> IO[REGB])
        instructions.Add(new Instruction("IOWRITEB", 0b0010_0011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | IO_WRITE_BUS_B
        }));
        #endregion

        #region > MATH (0011.XXXX)
        //> ADD
        //  0011.0000 REG1.REG2 0000.0000 (REG1 + REG2) -> REG1, FLAG(OVERFLOW)
        instructions.Add(new Instruction("ADD", 0b0011_0000, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ADDOV
        //  0011.0001 REG1.REG2 0000.0000 (REG1 + REG2) -> REG1 (+1 IF FLAG(OVERFLOW))
        instructions.Add(new Instruction("ADDOV", 0b0011_0001, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUB
        //  0011.0010 REG1.REG2 0000.0000 (REG1 - REG2) -> REG1
        instructions.Add(new Instruction("SUB", 0b0011_0010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUBOV
        //  0011.0011 REG1.REG2 0000.0000 (REG1 - REG2) -> REG1
        instructions.Add(new Instruction("SUBOV", 0b0011_0011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> AND
        //  0011.0100 REG1.REG2 0000.0000 (REG1 & REG2) -> REG1
        instructions.Add(new Instruction("AND", 0b0011_0100, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_AND | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> OR
        //  0011.0101 REG1.REG2 0000.0000 (REG1 | REG2) -> REG1
        instructions.Add(new Instruction("OR", 0b0011_0101, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_OR | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> NOT
        //  0011.0110 REG1.REG2 0000.0000 (   ~REG2   ) -> REG1
        instructions.Add(new Instruction("NOT", 0b0011_0110, new List<ulong>
        {
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_NOT | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> CMP
        //  0011.0111 REG1.REG2 0000.0000 (REG1 ? REG2) -> FLAG(EQ)
        instructions.Add(new Instruction("CMP", 0b0011_0111, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_CMP
        }));

        //> ADDI
        //  0011.1000 REG1.0000 AAAA.AAAA (REG1 + AAAA) -> REG1
        instructions.Add(new Instruction("ADDI", 0b0011_1000, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ADDIOV
        //  0011.1001 REG1.0000 AAAA.AAAA (REG1 + AAAA) -> REG1, FLAG(OVERFLOW)
        instructions.Add(new Instruction("ADDIOV", 0b0011_1001, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUBI
        //  0011.1010 REG1.0000 AAAA.AAAA (REG1 - AAAA) -> REG1
        instructions.Add(new Instruction("SUBI", 0b0011_1010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUBIOV
        //  0011.1011 REG1.0000 AAAA.AAAA (REG1 - AAAA) -> REG1
        instructions.Add(new Instruction("SUBIOV", 0b0011_1011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ANDI
        //  0011.1100 REG1.0000 AAAA.AAAA (REG1 & AAAA) -> REG1
        instructions.Add(new Instruction("ANDI", 0b0011_1100, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_AND | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ORI
        //  0011.1101 REG1.0000 AAAA.AAAA (REG1 | AAAA) -> REG1
        instructions.Add(new Instruction("ORI", 0b0011_1101, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_OR | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> NOTI
        //  ????.???? ????.???? ????.???? (???????????) -> ????
        

        //> CMPI
        //  0011.1111 REG1.0000 AAAA.AAAA (REG1 ? AAAA) -> FLAG(EQ)
        instructions.Add(new Instruction("CMPI", 0b0011_1111, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_CMP
        }));

        #endregion

        #region > CONTROL FLOW (0100.XXXX)
        //> JEQ
        //  0100.0000 AAAA.AAAA AAAA.AAAA
        instructions.Add(new Instruction("JEQ", 0b0100_0000, new List<ulong>
        {
            JEQ
        }));

        //> JNEQ
        //  0100.0001 AAAA.AAAA AAAA.AAAA
        instructions.Add(new Instruction("JNEQ", 0b0100_0001, new List<ulong>
        {
            JNEQ
        }));

        //> JMP
        //  0100.0010 AAAA.AAAA AAAA.AAAA
        instructions.Add(new Instruction("JMP", 0b0100_0010, new List<ulong>
        {
            JMP
        }));
        #endregion

        //> HLT
        //  0100.0011 0000.0000 0000.0000
        instructions.Add(new Instruction("HLT", 0b0100_0011, new List<ulong>
        {
            HLT
        }));

        // List Instructions
        Console.WriteLine("Instructions: ");
        foreach (var inst in instructions)
        {
            Console.WriteLine($" > {inst.Name} ({inst.Opcode}) ({inst.Steps.Count} Steps):");
            foreach (ulong step in inst.Steps)
                Console.WriteLine($"   - {Convert.ToString((long)step, 16).PadLeft(16, '0')}");
            Console.WriteLine(  );
        }


        ExportMicrocode("microcode.hex", instructions);
    }

    public static ulong[,] ConvertMicrocode(List<Instruction> instructions)
    {
        // THE ROM HAS A 12 BIT ADDRESS
        // HI 8 BITS ARE THE OPCODE
        // LO 4 BITS ARE THE STEP INDEX
        // PER ADRESS IT STORES A 32 BIT "STEP" VALUE
        ulong[,] data = new ulong[256, 16];
        for (int i = 0; i < 256; i++)
            data[i, 0] = DONE;

        foreach (var inst in instructions)
        {
            if (inst.Steps.Count > 16)
                throw new Exception("Instruction has too many steps!");

            for (int i = 0; i < inst.Steps.Count; i++)
                data[inst.Opcode, i] = (ulong)inst.Steps[i];
        }

        return data;
    }

    public static void ExportMicrocode(string name, List<Instruction> instructions)
    {
        ExportData(name, ConvertMicrocode(instructions));
    }

    public static string DataToString(ulong[,] data, int yIndex, int xStart)
    {
        string line = "";
        for (int x = xStart; x < xStart + 8; x++)
            line += Convert.ToString((long)data[yIndex, x], 16).PadLeft(16, '0') + " ";
        return line;
    }

    public static void ExportData(string name, ulong[,] data)
    {
        // Header
        // Then 8 steps per line
        // -> 2 lines per instruction

        using (StreamWriter sw = new StreamWriter(name))
        {
            sw.WriteLine("v3.0 hex words plain");

            for (int ins = 0; ins < data.GetLength(0); ins++)
            {
                sw.WriteLine(DataToString(data, ins, 0));
                sw.WriteLine(DataToString(data, ins, 8));
            }
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
 > JMP
   0100.0010 AAAA.AAAA AAAA.AAAA
 > HLT
   0100.0011 0000.0000 0000.0000
*/