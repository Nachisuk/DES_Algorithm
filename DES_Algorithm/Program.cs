using System;

namespace DES_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string text = "1101001100110100010101110111100110011011101111001101111111111111";
            string key = "1001001100111100010101110111100110011011101111011101111111110001";

            Console.WriteLine(text);
            Console.WriteLine(key);

            int[] text1 = new int[64];
            int[] key1 = new int[64];
            
                for(int i=0;i<64;i++)
            {
                text1[i] = (int)(text[i] - '0');
                key1[i] = (int)(key[i] - '0');

               // Console.WriteLine("Reee " + text1[i] + " " + key[i]+" "+i);
            }
            DES algotytm = new DES(text1,key1);
            algotytm.DES_Cipher_Prepare();

            text1 = algotytm.inputArray;
            for (int i = 0; i < 64; i++)
            {
                Console.Write(text1[i]);
            }
            Console.WriteLine();

           // DES algotytm2 = new DES(text1, key1);
           // algotytm2.DES_PrepareDecipher();
           //text1 = algotytm2.inputArray;
           // for (int i = 0; i < 64; i++)
          //  {
               // Console.Write(text1[i]);
          //  }
            Console.ReadKey();
        }
    }
}
