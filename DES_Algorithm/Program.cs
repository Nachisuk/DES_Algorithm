using System;

namespace DES_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string text = "1101001100110100010101110111100110011011101111001101110001111111";
            string key = "1001101100111100010101110111100110011011101111011101111111110001";

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
            algorytm1.Prepare16RoundsKey();
            algorytm1.assignCipherTextAndKey(text3, key1);
            algorytm1.startDecryption();
            int[] text4 = algorytm1.getDecryption();
            Console.WriteLine(text + " -nasz input");
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text4[i]);
            }
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
            Console.ReadKey();
        }
    }
}
