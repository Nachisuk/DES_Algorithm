using System;
using System.IO;

namespace DES_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            MainMenu();
        }

        public static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("KRYPTOSYSTEMY SYMETRYCZNE - ALGORYTM DES");
                Console.WriteLine("1. Zakoduj plik binarny");
                Console.WriteLine("2. Rozkoduj plik binarny");
                Console.WriteLine("3. Obsługa plików");
                String c = Console.ReadLine();

                switch (c)
                {
                    case "1":
                        EncryptMenu();
                        break;
                    case "2":
                        DecryptMenu();
                        break;
                    case "3":
                        ObslugaPlikow();
                        break;
                }

            }
        }

        public static void EncryptMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Zakoduj:");
                Console.WriteLine("1. Z wlasnym kluczem");
                Console.WriteLine("2. Z losowym kluczem");
                Console.WriteLine("3. Powrót");
                String c = Console.ReadLine();

                switch (c)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Podaj 64-bitowy klucz (ciąg 0/1):");
                        string textKey = Console.ReadLine();

                        if(textKey.Length!=64)
                        {
                            Console.WriteLine("Twoj klucz nie ma 64 bitów! ");
                            Console.ReadKey();
                            break;
                        }

                        int[] key = new int[64];
                        for (int i = 0; i < 64; i++)
                        {
                            key[i] = (int)(textKey[i] - '0');
                        }

                        Console.WriteLine("Podaj nazwę pliku do zakodowania (wejściowego):");
                        string input = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę pliku - wyjściowego:");
                        string output = Console.ReadLine();

                        EncryptFromBinFileToBinFile(input, output, key);

                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("Twój klucz to:");
                        string textKey2="";
                        int[] key2;
                        DES2 algorytm = new DES2();
                        algorytm.generateAndAssignRandomKey();
                        key2 = algorytm.getKey();
                        for (int i = 0; i < 64; i++)
                        {
                            textKey2 = textKey2 + key2[i];
                        }
                        Console.WriteLine(textKey2);

                        Console.WriteLine("Podaj nazwę pliku do zakodowania (wejściowego):");
                        string input2 = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę pliku - wyjściowego:");
                        string output2 = Console.ReadLine();

                        EncryptFromBinFileToBinFile(input2, output2, key2);

                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();
                        break;
                    case "3":
                        return;
                }

            }
        }

        public static void DecryptMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Rozkoduj:");
                Console.WriteLine("1. Z wlasnym kluczem");
                Console.WriteLine("2. Z losowym kluczem");
                Console.WriteLine("3. Powrót");
                String c = Console.ReadLine();

                switch (c)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Podaj 64-bitowy klucz (ciąg 0/1):");
                        string textKey = Console.ReadLine();

                        if (textKey.Length != 64)
                        {
                            Console.WriteLine("Twoj klucz nie ma 64 bitów! ");
                            Console.ReadKey();
                            break;
                        }

                        int[] key = new int[64];
                        for (int i = 0; i < 64; i++)
                        {
                            key[i] = (int)(textKey[i] - '0');
                        }

                        Console.WriteLine("Podaj nazwę pliku do rozkodowania (wejściowego):");
                        string input = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę pliku - wyjściowego:");
                        string output = Console.ReadLine();

                        DecryptFromBinFileToBinFile(input, output, key);

                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("Twój klucz to:");
                        string textKey2 = "";
                        int[] key2;
                        DES2 algorytm = new DES2();
                        algorytm.generateAndAssignRandomKey();
                        key2 = algorytm.getKey();
                        for (int i = 0; i < 64; i++)
                        {
                            textKey2 = textKey2 + key2[i];
                        }
                        Console.WriteLine(textKey2);

                        Console.WriteLine("Podaj nazwę pliku do rozkodowania (wejściowego):");
                        string input2 = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę pliku - wyjściowego:");
                        string output2 = Console.ReadLine();

                        DecryptFromBinFileToBinFile(input2, output2, key2);

                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();
                        break;
                    case "3":
                        return;
                }

            }
        }

        public static void EncryptFromBinFileToBinFile(string inputFileName, string outputFileName, int[] _key)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(outputFileName, FileMode.Create)))
            using (BinaryReader br = new BinaryReader(File.Open(inputFileName, FileMode.Open)))
            {
                int pos = 0, i, j;
                int length = (int)br.BaseStream.Length;
                int[] textToEncrypt = new int[64];
                byte v;
                bool areFullBlocks = true;

                while (pos < length)
                {
                    int indxOfInput = 0;
                    for (j=0; j < 8; j++) //8 bajtów bo 8 bajtów po 8 bitów to 8*8=64 bity
                    {
                        if (pos < length) 
                        {
                            v = br.ReadByte();

                            for (i = 0; i < 8; i++)
                            {
                                int valueOfBit = ((v & (1 << i)) == 0) ? 0 : 1;

                                textToEncrypt[indxOfInput] = valueOfBit;
                                indxOfInput++;
                            }
                        }
                        else //w przypadku gdy skoczyl sie plik ale mamy zaczety blok - dopelnij blok zerami
                        {
                            areFullBlocks = false;
                            for (i = 0; i < 8; i++)
                            {
                                int valueOfBit = 0;

                                textToEncrypt[indxOfInput] = valueOfBit;
                                indxOfInput++;
                            }
                        }
                        pos += sizeof(byte);
                    }

                    if(!areFullBlocks)
                    {
                        Console.WriteLine("Twój plik wejściowy nie miał pełnych 64-bitowych bloków - dopełniono go zerami");
                    }

                    DES2 algorytmDes = new DES2();
                    algorytmDes.assignPlainTextAndKey(textToEncrypt, _key);
                    algorytmDes.startEncryption();
                    int[] outputEncrypted = algorytmDes.getEnrypction();

                    indxOfInput = 0;
                    for (j = 0; j < 8; j++) 
                    {
                        byte outByte = 0;

                        for (i = 0; i < 8; i++)
                        {
                            if (outputEncrypted[indxOfInput] == 1)
                            {
                                outByte = (byte)(outByte | (1 << i));
                            }
                            indxOfInput++;
                        }

                        bw.Write(outByte);
                    }
                }

            }
            Console.WriteLine("Zapisano wyniki w pliku \"{0}\".", outputFileName);
        }


        public static void DecryptFromBinFileToBinFile(string inputFileName, string outputFileName, int[] _key)
        {
            using (BinaryWriter bw = new BinaryWriter(File.Open(outputFileName, FileMode.Create)))
            using (BinaryReader br = new BinaryReader(File.Open(inputFileName, FileMode.Open)))
            {
                int pos = 0, i, j;
                int length = (int)br.BaseStream.Length;
                int[] textToDecrypt = new int[64];
                byte v;

                while (pos < length)
                {
                    int indxOfInput = 0;
                    for (j = 0; j < 8; j++) //8 bajtów bo 8 bajtów po 8 bitów to 8*8=64 bity
                    {
                        v = br.ReadByte();

                        for (i = 0; i < 8; i++)
                        {
                            int valueOfBit = ((v & (1 << i)) == 0) ? 0 : 1;

                            textToDecrypt[indxOfInput] = valueOfBit;
                            indxOfInput++;
                        }
                        pos += sizeof(byte);
                    }


                    DES2 algorytmDes = new DES2();
                    algorytmDes.assignCipherTextAndKey(textToDecrypt, _key);
                    algorytmDes.startDecryption();
                    int[] outputDecrypted = algorytmDes.getDecryption();

                    indxOfInput = 0;
                    for (j = 0; j < 8; j++)
                    {
                        byte outByte = 0;

                        for (i = 0; i < 8; i++)
                        {
                            if (outputDecrypted[indxOfInput] == 1)
                            {
                                outByte = (byte)(outByte | (1 << i));
                            }
                            indxOfInput++;
                        }

                        bw.Write(outByte);
                    }
                }

            }
            Console.WriteLine("Zapisano wyniki w pliku \"{0}\".", outputFileName);
        
        }

        //--------------------------------------------------------- DODATKOWA FUNKCJONALNOSC---------------------------------------------------------------------

        static void PrintBinaryFileContentBitByBit(string inputFileName)
        {
            using (BinaryReader b = new BinaryReader(File.Open(inputFileName, FileMode.Open)))
            {
                int pos = 0;

                int length = (int)b.BaseStream.Length;
                while (pos < length)
                {
                    byte v = b.ReadByte();
                    for (int i = 0; i < 8; i++)
                    {
                        int valueOfBit = ((v & (1 << i)) == 0) ? 0 : 1;
                        Console.Write(valueOfBit + " ");
                    }
                    Console.Write(" :" + v + "\n");

                    pos += sizeof(byte);
                }
            }
        }

        static void CreateTxtFileFromBinaryBitByBit(string inputFileName, string outputFileName)
        {
            using (StreamWriter wr = new StreamWriter(outputFileName, false))
            using (BinaryReader b = new BinaryReader(File.Open(inputFileName, FileMode.Open)))
            {
                int pos = 0;

                int length = (int)b.BaseStream.Length;
                while (pos < length)
                {
                    byte v = b.ReadByte();
                    int[] bitSeq = new int[8];
                    for (int i = 0; i < 8; i++)
                    {
                        int valueOfBit = ((v & (1 << i)) == 0) ? 0 : 1;
                        bitSeq[i] = valueOfBit;
                    }
                    wr.WriteLine(string.Join(" ", bitSeq));

                    pos += sizeof(byte);
                }
            }
            Console.WriteLine("Przekonwertowano do tekstowego \"{0}\".", outputFileName);
        }

        static void CreateBinaryFileFromTextBitByBit(string inputFileName, string outputFileName)
        {
            using (StreamReader rd = new StreamReader(inputFileName))
            using (BinaryWriter bw = new BinaryWriter(File.Open(outputFileName, FileMode.Create)))
            {
                while (true)
                {
                    string line = rd.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    string[] breakedLine = line.Split(' ');
                    //mamy stringa wiec:
                    byte outByte = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (breakedLine[i] == "1")
                        {
                            outByte = (byte)(outByte | (1 << i));
                        }
                    }
                    bw.Write(outByte);
                }
            }
            Console.WriteLine("Przekonwertowano do binarnego \"{0}\".", outputFileName);
        }

        static void ObslugaPlikow()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Wypisz plik binarny bit po bicie");
                Console.WriteLine("2. Stworz plik tekstowy z pliku binarnego bit po bicie");
                Console.WriteLine("3. Stworz plik binarny z pliku tekstowego");
                Console.WriteLine("4. Powrót");
                String c = Console.ReadLine();

                switch (c)
                {
                    case "1":
                        Console.Clear();
                        Console.WriteLine("Podaj nazwę pliku, który chcesz wypisać");
                        string inputFileName = Console.ReadLine();
                        PrintBinaryFileContentBitByBit(inputFileName);

                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();
                        break;
                    case "2":

                        Console.Clear();
                        Console.WriteLine("Podaj nazwę pliku wejściowego");
                        inputFileName = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę pliku wyjściowego");
                        string outputFileName = Console.ReadLine();

                        CreateTxtFileFromBinaryBitByBit(inputFileName, outputFileName);
                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();

                        break;
                    case "3":
                        Console.Clear();
                        Console.WriteLine("Podaj nazwę pliku wejściowego");
                        inputFileName = Console.ReadLine();

                        Console.WriteLine("Podaj nazwę pliku wyjściowego");
                        outputFileName = Console.ReadLine();

                        CreateBinaryFileFromTextBitByBit(inputFileName, outputFileName);
                        Console.WriteLine("Naciśnij klawisz aby kontynuować...");
                        Console.ReadKey();
                        break;
                    case "4":
                        return;
                }
            }
        }
    }
}
