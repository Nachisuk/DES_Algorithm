using System;
using System.IO;

namespace DES_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string text = "1101001100110100010101110111100110011011101111001101110001110111";
            string key = "1000011100111100010101110111100110011011101111011101111111110001";

            Console.WriteLine("Plain text ");
            Console.WriteLine(text);

            Console.WriteLine("Klucz ");
            Console.WriteLine(key);

            int[] text1 = new int[64];
            int[] key1 = new int[64];
            
                for(int i=0;i<64;i++)
            {
                text1[i] = (int)(text[i] - '0');
                key1[i] = (int)(key[i] - '0');

               // Console.WriteLine("Reee " + text1[i] + " " + key[i]+" "+i);
            }
            /*
        DES algotytm = new DES(text1,key1);
        algotytm.DES_Cipher_Prepare();
        Console.WriteLine();
        Console.WriteLine("Wynik1 ");
        int[] text2 = algotytm.inputArray;
        for (int i = 0; i < 64; i++)
        {
            Console.Write(text2[i]);
        }
        Console.WriteLine();
        */
            Console.WriteLine("Szyfrowanie po linii binarnej 64");
            Console.WriteLine();
            DES2 algorytm1 = new DES2();
            algorytm1.assignPlainTextAndKey(text1, key1);
            algorytm1.startEncryption();
            int[] text3 = algorytm1.getEnrypction();
            Console.WriteLine("Wynik2 ");
            Console.WriteLine(text+" -nasz input");
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text3[i]);
            }
            Console.Write(" - szyfrowanie");
            Console.WriteLine();
            /*
            algorytm1.assignCipherTextAndKey(text3, key1);
            algorytm1.startDecryption();
            int[] text4 = algorytm1.getDecryption();
            Console.WriteLine(text + " -nasz input");
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text4[i]);
            }
            Console.WriteLine();
            
            */
            Console.WriteLine("Deszyfrowanie po linii binarnej 64");
            Console.WriteLine();
            Console.WriteLine();
            DES2 algorytm2 = new DES2();
            
            algorytm2.assignCipherTextAndKey(text3, key1);
            algorytm2.Prepare16RoundsKey();
            algorytm2.startDecryption();
            int[] text4 = algorytm2.getDecryption();
            Console.WriteLine(text + " -nasz input");
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text4[i]);
            }
            Console.Write(" - odszyfrowanie");
            Console.WriteLine();
            Console.WriteLine();
            /*
            int[,] subkeys = algorytm1.getAllSubkeys();

            for(int i=0;i<16;i++)
            {
                for(int j=0;j<48;j++)
                {
                    Console.Write(subkeys[i, j]);
                }
                Console.WriteLine();
            }
            */
            Console.WriteLine("Szyfrowanie po string key i string tekst");
            Console.WriteLine();
            string n = algorytm1.EncryptFromString("abbadese", "kreda");
            Console.Write(n +" -wynik szyfrowania stringu");
            Console.WriteLine();
            Console.Write(algorytm1.DecryptFromString(n, "kreda")+" - wynik deszyfrowania");

            Console.WriteLine();
            EncryptFromBinFileToBinFile("test.bin", "wynik.bin", key1);
            DecryptFromBinFileToBinFile("wynik.bin", "output.bin", key1);

            Console.ReadKey();
        }




        public static void EncryptFromBinFileToBinFile(string inputFileName, string outputFileName, int[] _key)
        {
            /*
            Console.WriteLine("Szyfrowanie po linii binarnej 64");
            Console.WriteLine();
            DES2 algorytm1 = new DES2();
            algorytm1.assignPlainTextAndKey(text1, key1);
            algorytm1.startEncryption();
            int[] text3 = algorytm1.getEnrypction();
            Console.WriteLine("Wynik2 ");
            Console.WriteLine(text+" -nasz input");
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text3[i]);
            }
            Console.Write(" - szyfrowanie");
            Console.WriteLine();
             */

            /*
            using (BinaryWriter bw = new BinaryWriter(File.Open(outputFileName, FileMode.Create)))
            using (BinaryReader br = new BinaryReader(File.Open(inputFileName, FileMode.Open)))
            {
                int pos = 0;
                int length = (int)br.BaseStream.Length;


                while (pos < length)
                {
                    byte v = br.ReadByte();

                    byte outByte = 0;
                    for (i = 0; i < 8; i++)
                    {
                        int valueOfBit = ((v & (1 << i)) == 0) ? 0 : 1;

                        tmp = 0;
                        foreach (int indx in indexesToXOR)
                        {
                            tmp ^= actualMemo[indx];
                        }

                        tmp ^= valueOfBit;

                        if (tmp == 1)
                        {
                            outByte = (byte)(outByte | (1 << i));
                        }

                        Array.Copy(actualMemo, 0, actualMemo, 1, actualMemo.Length - 1);
                        actualMemo[0] = tmp;
                    }
                    bw.Write(outByte);

                    pos += sizeof(byte);
                }
            }

            Console.WriteLine("Zapisano wyniki w pliku \"{0}\".", outputFileName);
            */

            using (BinaryWriter bw = new BinaryWriter(File.Open(outputFileName, FileMode.Create)))
            using (BinaryReader br = new BinaryReader(File.Open(inputFileName, FileMode.Open)))
            {
                int pos = 0, i, j;
                int length = (int)br.BaseStream.Length;
                int[] textToEncrypt = new int[64];
                byte v;

                while (pos < length)
                {
                    int indxOfInput = 0;
                    for (j=0; j < 8; j++) //8 bajtów bo 8 bajtów po 8 bitów to 8*8=64 bity
                    {
                        v = br.ReadByte();

                        for (i = 0; i < 8; i++)
                        {
                            int valueOfBit = ((v & (1 << i)) == 0) ? 0 : 1;

                            textToEncrypt[indxOfInput] = valueOfBit;
                            indxOfInput++;
                        }
                        pos += sizeof(byte);
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
            /*
            Console.WriteLine("Deszyfrowanie po linii binarnej 64");
            Console.WriteLine();
            Console.WriteLine();
            algorytm1.Prepare16RoundsKey();
            algorytm1.assignCipherTextAndKey(text3, key1);
            algorytm1.startDecryption();
            int[] text4 = algorytm1.getDecryption();
            Console.WriteLine(text + " -nasz input");
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text4[i]);
            }
            Console.Write(" - odszyfrowanie");
            Console.WriteLine();
            Console.WriteLine();
            */

            

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
                    algorytmDes.Prepare16RoundsKey();
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
