class Program
{
    public enum InsE
    {
        MOVE,
        MOVEI,
        LDA,
        LDB,
        RES_FLAG,

        READA,
        READB,
        WRITEA,
        WRITEB,

        IOREADA,
        IOREADB,
        IOWRITEA,
        IOWRITEB,

        ADD,
        ADDOV,
        SUB,
        SUBOV,
        AND,
        OR,
        NOT,
        CMP,

        ADDI,
        ADDIOV,
        SUBI,
        SUBIOV,
        ANDI,
        ORI,
        //NOTI,
        CMPI,

        JEQ,
        JNEQ,
        JMP,
        HLT
    }

    public struct InstructionDef
    {
        public InsE Name;
        public byte Opcode;
        public List<ulong> Steps;
        public InstructionDef(InsE name, byte opcode, List<ulong> steps)
        {
            this.Name = name;
            this.Opcode = opcode;
            this.Steps = steps;
        }
    }

    public struct Instruction
    {
        public InsE Name;
        public byte OP_HIGH;
        public byte OP_LOW;
        public bool IsLabel;
        public string LabelName;

        public Instruction(InsE name, byte op_high, byte op_low)
        {
            this.Name = name;
            this.OP_HIGH = op_high;
            this.OP_LOW = op_low;
            this.IsLabel = false;
            LabelName= "";
        }

        public Instruction(string label)
        {
            this.Name = InsE.LDA;
            this.OP_HIGH = 0;
            this.OP_LOW = 0;
            this.IsLabel = true;
            LabelName = label;
        }

        public static Instruction RegInstruction(InsE name, byte reg1, byte reg2, byte op2)
        {
            return new Instruction(name, (byte)(reg1 << 4 | reg2), op2);
        }

        public static Instruction RegIns(InsE name, byte reg1, byte op2)
        {
            return new Instruction(name, (byte)(reg1 << 4), op2);
        }

        public static Instruction OpIns(InsE name, ushort op)
        {
            return new Instruction(name, (byte)(op >> 8), (byte)(op & 0xFF));
        }

        public static Instruction EmptyIns(InsE name)
        {
            return new Instruction(name, 0, 0);
        }

        public static Instruction Label(string name)
        {
            return new Instruction(name);
        }

        public override string ToString()
        {
            return IsLabel ? $"[LABEL]: {Name}" : $"{Name} ({OP_HIGH:X2}.{OP_LOW:X2})";
        }
    }

    #region MICROCODE
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
    #endregion

    public static List<InstructionDef> InitInstructionSet(bool print)
    {
        List<InstructionDef> instructionSet = new List<InstructionDef>();

        #region > REGISTER MANIPULATION (0000.XXXX)
        //> MOVE
        //  0000.0000 REG1.REG2 0000.0000 (REG2 -> REG1)
        // REG1 IS HI, REG2 IS LO
        instructionSet.Add(new InstructionDef(InsE.MOVE, 0b0000_0000, new List<ulong> {
            PUT_REG_IOP0_LO_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> MOVEI
        //  0000.0001 REG1.0000 AAAA.AAAA (AAAA -> REG1)
        // REG1 IS HI
        instructionSet.Add(new InstructionDef(InsE.MOVEI, 0b0000_0001, new List<ulong> {
            PUT_VAL_IOP1_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> LDA
        //  0000.0010 AAAA.AAAA AAAA.AAAA (AAAA -> REGA)
        instructionSet.Add(new InstructionDef(InsE.LDA, 0b0000_0010, new List<ulong> {
            PUT_VAL_IOP0_BUS | STR_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_REG_1_BUS
        }));

        //> LDB
        //  0000.0011 AAAA.AAAA AAAA.AAAA (AAAA -> REGB)
        instructionSet.Add(new InstructionDef(InsE.LDB, 0b0000_0011, new List<ulong>
        {
            PUT_VAL_IOP0_BUS | STR_REG_2_BUS,
            PUT_VAL_IOP1_BUS | STR_REG_3_BUS
        }));

        //> RES FLAG
        //  0000.0100 0000.0000 0000.0000 (0    -> FLAG)
        instructionSet.Add(new InstructionDef(InsE.RES_FLAG, 0b0000_0100, new List<ulong>
        {
            ALU_FLAG_RESET
        }));
        #endregion

        #region > MEMORY MANIPULATION (0001.XXXX)
        //> READA
        //  0001.0000 REG1.0000 0000.0000 (RAM[REGA] -> REG1)
        instructionSet.Add(new InstructionDef(InsE.READA, 0b0001_0000, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | RAM_READ_BUS_A
        }));

        //> READB
        //  0001.0001 REG1.0000 0000.0000 (RAM[REGB] -> REG1)
        instructionSet.Add(new InstructionDef(InsE.READB, 0b0001_0001, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | RAM_READ_BUS_B
        }));

        //> WRITEA
        //  0001.0010 REG1.0000 0000.0000 (REG1 -> RAM[REGA])
        instructionSet.Add(new InstructionDef(InsE.WRITEA, 0b0001_0010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | RAM_WRITE_BUS_A
        }));

        //> WRITEB
        //  0001.0011 REG1.0000 0000.0000 (REG1 -> RAM[REGB])
        instructionSet.Add(new InstructionDef(InsE.WRITEB, 0b0001_0011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | RAM_WRITE_BUS_B
        }));
        #endregion

        #region > IO (0010.XXXX)
        //> IOREADA
        //  0010.0000 REG1.0000 0000.0000 (IO[REGA] -> REG1)
        instructionSet.Add(new InstructionDef(InsE.IOREADA, 0b0010_0000, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | IO_READ_BUS_A
        }));

        //> IOREADB
        //  0010.0001 REG1.0000 0000.0000 (IO[REGB] -> REG1)
        instructionSet.Add(new InstructionDef(InsE.IOREADB, 0b0010_0001, new List<ulong>
        {
            STR_REG_IOP0_HI_BUS | IO_READ_BUS_B
        }));

        //> IOWRITEA
        //  0010.0010 REG1.0000 0000.0000 (REG1 -> IO[REGA])
        instructionSet.Add(new InstructionDef(InsE.IOWRITEA, 0b0010_0010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | IO_WRITE_BUS_A
        }));

        //> IOWRITEB
        //  0010.0011 REG1.0000 0000.0000 (REG1 -> IO[REGB])
        instructionSet.Add(new InstructionDef(InsE.IOWRITEB, 0b0010_0011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | IO_WRITE_BUS_B
        }));
        #endregion

        #region > MATH (0011.XXXX)
        //> ADD
        //  0011.0000 REG1.REG2 0000.0000 (REG1 + REG2) -> REG1, FLAG(OVERFLOW)
        instructionSet.Add(new InstructionDef(InsE.ADD, 0b0011_0000, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ADDOV
        //  0011.0001 REG1.REG2 0000.0000 (REG1 + REG2) -> REG1 (+1 IF FLAG(OVERFLOW))
        instructionSet.Add(new InstructionDef(InsE.ADDOV, 0b0011_0001, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUB
        //  0011.0010 REG1.REG2 0000.0000 (REG1 - REG2) -> REG1
        instructionSet.Add(new InstructionDef(InsE.SUB, 0b0011_0010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUBOV
        //  0011.0011 REG1.REG2 0000.0000 (REG1 - REG2) -> REG1
        instructionSet.Add(new InstructionDef(InsE.SUBOV, 0b0011_0011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> AND
        //  0011.0100 REG1.REG2 0000.0000 (REG1 & REG2) -> REG1
        instructionSet.Add(new InstructionDef(InsE.AND, 0b0011_0100, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_AND | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> OR
        //  0011.0101 REG1.REG2 0000.0000 (REG1 | REG2) -> REG1
        instructionSet.Add(new InstructionDef(InsE.OR, 0b0011_0101, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_OR | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> NOT
        //  0011.0110 REG1.REG2 0000.0000 (   ~REG2   ) -> REG1
        instructionSet.Add(new InstructionDef(InsE.NOT, 0b0011_0110, new List<ulong>
        {
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_NOT | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> CMP
        //  0011.0111 REG1.REG2 0000.0000 (REG1 ? REG2) -> FLAG(EQ)
        instructionSet.Add(new InstructionDef(InsE.CMP, 0b0011_0111, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_REG_IOP0_LO_BUS | STR_ALU_REG_1_BUS,
            ALU_CMP
        }));

        //> ADDI
        //  0011.1000 REG1.0000 AAAA.AAAA (REG1 + AAAA) -> REG1
        instructionSet.Add(new InstructionDef(InsE.ADDI, 0b0011_1000, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ADDIOV
        //  0011.1001 REG1.0000 AAAA.AAAA (REG1 + AAAA) -> REG1, FLAG(OVERFLOW)
        instructionSet.Add(new InstructionDef(InsE.ADDIOV, 0b0011_1001, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_ADD_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUBI
        //  0011.1010 REG1.0000 AAAA.AAAA (REG1 - AAAA) -> REG1
        instructionSet.Add(new InstructionDef(InsE.SUBI, 0b0011_1010, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> SUBIOV
        //  0011.1011 REG1.0000 AAAA.AAAA (REG1 - AAAA) -> REG1
        instructionSet.Add(new InstructionDef(InsE.SUBIOV, 0b0011_1011, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_SUB_OV | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ANDI
        //  0011.1100 REG1.0000 AAAA.AAAA (REG1 & AAAA) -> REG1
        instructionSet.Add(new InstructionDef(InsE.ANDI, 0b0011_1100, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_AND | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> ORI
        //  0011.1101 REG1.0000 AAAA.AAAA (REG1 | AAAA) -> REG1
        instructionSet.Add(new InstructionDef(InsE.ORI, 0b0011_1101, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_OR | PUT_ALU_RES_BUS | STR_REG_IOP0_HI_BUS
        }));

        //> NOTI
        //  ????.???? ????.???? ????.???? (???????????) -> ????


        //> CMPI
        //  0011.1111 REG1.0000 AAAA.AAAA (REG1 ? AAAA) -> FLAG(EQ)
        instructionSet.Add(new InstructionDef(InsE.CMPI, 0b0011_1111, new List<ulong>
        {
            PUT_REG_IOP0_HI_BUS | STR_ALU_REG_0_BUS,
            PUT_VAL_IOP1_BUS | STR_ALU_REG_1_BUS,
            ALU_CMP
        }));

        #endregion

        #region > CONTROL FLOW (0100.XXXX)
        //> JEQ
        //  0100.0000 AAAA.AAAA AAAA.AAAA
        instructionSet.Add(new InstructionDef(InsE.JEQ, 0b0100_0000, new List<ulong>
        {
            JEQ
        }));

        //> JNEQ
        //  0100.0001 AAAA.AAAA AAAA.AAAA
        instructionSet.Add(new InstructionDef(InsE.JNEQ, 0b0100_0001, new List<ulong>
        {
            JNEQ
        }));

        //> JMP
        //  0100.0010 AAAA.AAAA AAAA.AAAA
        instructionSet.Add(new InstructionDef(InsE.JMP, 0b0100_0010, new List<ulong>
        {
            JMP
        }));

        //> HLT
        //  0100.0011 0000.0000 0000.0000
        instructionSet.Add(new InstructionDef(InsE.HLT, 0b0100_0011, new List<ulong>
        {
            HLT
        }));
        #endregion

        // Print Instruction set
        if (print)
        {
            Console.WriteLine("Instruction Set: ");
            foreach (var inst in instructionSet)
            {
                Console.WriteLine($" > {inst.Name} ({inst.Opcode}) ({inst.Steps.Count} Steps):");
                foreach (ulong step in inst.Steps)
                    Console.WriteLine($"   - {Convert.ToString((long)step, 16).PadLeft(16, '0')}");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        return instructionSet;
    }

    public static ushort LabelAddr(string label, List<Instruction> instructions)
    {
        ushort addr = 0;
        for (int i = 0; i < instructions.Count; i++)
        {
            Instruction inst = instructions[i];
            if (!inst.IsLabel)
            {
                addr++;
                continue;
            }
            if (instructions[i].LabelName == label)
                return addr;
        }
        throw new Exception($"Label {label} not found!");
    }

    public static List<Instruction> AddProgram(bool print)
    {
        List<Instruction> ins = new List<Instruction>();

        ins.Add(Instruction.Label("LOOP"));
        ins.Add(Instruction.RegIns(InsE.ADDI, 0, 1));
        ins.Add(Instruction.RegIns(InsE.CMPI, 0, 0xFF));
        ins.Add(Instruction.OpIns(InsE.JNEQ, LabelAddr("LOOP", ins)));
        ins.Add(Instruction.EmptyIns(InsE.HLT));


        // Print Program
        if (print)
        {
            Console.WriteLine("Program: ");
            foreach (var inst in ins)
                Console.WriteLine($" > {inst.ToString()}");
            Console.WriteLine();
        }

        return ins;
    }

    public static ulong MixStepAndOperands(ulong step, byte opHi, byte opLo)
    {
        // ABXXXXXX
        // 16 BIT OPERAND
        // 48 BIT STEP

        ulong mix = step;

        // Clear operands
        mix &= ~((ulong)0xFFFF << 48);

        // Set operands
        mix |= ((ulong)opHi << 56);
        mix |= ((ulong)opLo << 48);

        return mix;
    }

    public static List<ulong> CompileInstructions(List<Instruction> instructions, List<InstructionDef> instructionSet, bool print)
    {
        List<ulong> result = new List<ulong>();

        for (int i = 0; i < instructions.Count; i++)
        {
            Instruction inst = instructions[i];
            if (print)
                Console.WriteLine($"> COMPILING: {inst}");
            if (inst.IsLabel)
                continue;

            InstructionDef def = instructionSet.Find(x => x.Name == inst.Name);
            for (int i2 = 0; i2 < def.Steps.Count; i2++)
            {
                ulong temp = MixStepAndOperands(def.Steps[i2], inst.OP_HIGH, inst.OP_LOW);
                if (print)
                    Console.WriteLine($"  - {Convert.ToString((long)temp, 16).PadLeft(16, '0')}");
                result.Add(temp);
            }
        }


        return result;
    }

    public static void Main(string[] args)
    {
        List<InstructionDef> instructionSet = InitInstructionSet(true);

        List<Instruction> instructions = AddProgram(true);

        List<ulong> result = CompileInstructions(instructions, instructionSet, true);

        ExportMicrocode("microcode.hex", result);

        Console.WriteLine("Done!");
    }

    public static void ExportMicrocode(string name, List<ulong> data)
    {
        ExportData(name, FillData(data));
    }

    public static ulong[] FillData(List<ulong> data)
    {
        ulong[] allData = new ulong[65536];
        for (int i = 0; i < data.Count; i++)
            allData[i] = data[i];
        return allData;
    }

    public static string DataToString(ulong[] data, int offset)
    {
        string line = "";
        for (int i = offset; i < offset + 8; i++)
            line += Convert.ToString((long)data[i], 16).PadLeft(16, '0') + " ";
        return line;
    }

    public static void ExportData(string name, ulong[] data)
    {
        // Header
        // Then 8 steps per line
        // -> 2 lines per instruction

        using (StreamWriter sw = new StreamWriter(name))
        {
            sw.WriteLine("v3.0 hex words plain");

            for (int ins = 0; ins < data.Length; ins += 8)
            {
                sw.WriteLine(DataToString(data, ins));
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