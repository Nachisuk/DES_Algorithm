using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DES_Algorithm
{
    public class DES2
    {
        //Initial Permutation
        int[] initialPermutationTable = new int[] { 58,50,42,34,26,18,10,2,60,52,44,36,28,20,12,4,
                                       62,54,46,38,30,22,14,6,64,56,48,40,32,24,16,8,
                                       57,49,41,33,25,17,9,1,59,51,43,35,27,19,11,3,
                                       61,53,45,37,29,21,13,5,63,55,47,39,31,23,15,7 };

        //Przesunięcie w lewo podczas rund(kolejek) dla szyfrowania
        int[] circuralLeftShiftTable = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        //Przesunięcie w prawo podczas rund(kolejek) dla odszyfrowania 
        int[] circuralRightShiftTable = new int[] { 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1, 1 };

        //PermutedChoice1
        int[] permutedChoice1 = new int[] { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42,
            34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23,
            15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };

        //PermutedChoice2  
        int[] permutedChoice2 = new int[] { 14,17,11,24,1,5,3,28,15,6,21,10,
                                        23,19,12,4,26,8,16,7,27,20,13,2,
                                        41,52,31,37,47,55,30,40,51,45,33,48,
                                        44,49,39,56,34,53,46,42,50,36,29,32 };

        //Expansion Permutation Table  
        int[] expansionPermutationTable = new int[] { 32,1,2,3,4,5,4,5,6,7,8,9,
                                        8,9,10,11,12,13,12,13,14,15,16,17,
                                        16,17,18,19,20,21,20,21,22,23,24,25,
                                        24,25,26,27,28,29,28,29,30,31,32,1 };

        //S Box Tables ( Actual 2D S Box Tables have been converted to 1D S Box Tables for easier computation )  
        int[,] sBoxTable = new int[8, 64] { { 14,4,13,1,2,15,11,8,3,10,6,12,5,9,0,7,
                                                0,15,7,4,14,2,13,1,10,6,12,11,9,5,3,8,
                                                4,1,14,8,13,6,2,11,15,12,9,7,3,10,5,0,
                                                15,12,8,2,4,9,1,7,5,11,3,14,10,0,6,13 },
                                              { 15,1,8,14,6,11,3,4,9,7,2,13,12,0,5,10,
                                                3,13,4,7,15,2,8,14,12,0,1,10,6,9,11,5,
                                                0,14,7,11,10,4,13,1,5,8,12,6,9,3,2,15,
                                                13,8,10,1,3,15,4,2,11,6,7,12,0,5,14,9 },
                                              { 10,0,9,14,6,3,15,5,1,13,12,7,11,4,2,8,
                                                13,7,0,9,3,4,6,10,2,8,5,14,12,11,15,1,
                                                13,6,4,9,8,15,3,0,11,1,2,12,5,10,14,7,
                                                1,10,13,0,6,9,8,7,4,15,14,3,11,5,2,12 },
                                              { 7,13,14,3,0,6,9,10,1,2,8,5,11,12,4,15,
                                                13,8,11,5,6,15,0,3,4,7,2,12,1,10,14,9,
                                                10,6,9,0,12,11,7,13,15,1,3,14,5,2,8,4,
                                                3,15,0,6,10,1,13,8,9,4,5,11,12,7,2,14 },
                                              { 2,12,4,1,7,10,11,6,8,5,3,15,13,0,14,9,
                                                14,11,2,12,4,7,13,1,5,0,15,10,3,9,8,6,
                                                4,2,1,11,10,13,7,8,15,9,12,5,6,3,0,14,
                                                11,8,12,7,1,14,2,13,6,15,0,9,10,4,5,3 },
                                              { 12,1,10,15,9,2,6,8,0,13,3,4,14,7,5,11,
                                                10,15,4,2,7,12,9,5,6,1,13,14,0,11,3,8,
                                                9,14,15,5,2,8,12,3,7,0,4,10,1,13,11,6,
                                                4,3,2,12,9,5,15,10,11,14,1,7,6,0,8,13 },
                                              { 4,11,2,14,15,0,8,13,3,12,9,7,5,10,6,1,
                                                13,0,11,7,4,9,1,10,14,3,5,12,2,15,8,6,
                                                1,4,11,13,12,3,7,14,10,15,6,8,0,5,9,2,
                                                6,11,13,8,1,4,10,7,9,5,0,15,14,2,3,12 },
                                              { 13,2,8,4,6,15,11,1,10,9,3,14,5,0,12,7,
                                                1,15,13,8,10,3,7,4,12,5,6,11,0,14,9,2,
                                                7,11,4,1,9,12,14,2,0,6,10,13,15,3,5,8,
                                                2,1,14,7,4,10,8,13,15,12,9,0,3,5,6,11 } };

        //P Box Table  
        int[] pBoxTable = new int[] { 16,7,20,21,29,12,28,17,1,15,23,26,5,18,31,10,
                                         2,8,24,14,32,27,3,9,19,13,30,6,22,11,4,25 };

        //Final Permutation Table  
        int[] InversePermutationTable = new int[] { 40,8,48,16,56,24,64,32,39,7,47,15,55,23,63,31,
                                       38,6,46,14,54,22,62,30,37,5,45,13,53,21,61,29,
                                       36,4,44,12,52,20,60,28,35,3,43,11,51,19,59,27,
                                       34,2,42,10,50,18,58,26,33,1,41,9,49,17,57,25 };

        int[] plaintext = new int[64]; // do zaszyfrowania
        int[] ciphertext = new int[64]; // do odszyfrowania
        int[] keytext = new int[64]; //klucz

        int[] afterInitialPermutationText = new int[64]; //po pierwszej permutacji szyfrowania
        int[] afterInitialPermutationDecipher = new int[64]; // po pierwszej permutacja dla odszyfrowania

        int[] plaintextRight = new int[32];  //szyfrowania prawa
        int[] plaintextLeft = new int[32];   // szyfrowania lewa

        int[] plaintextRightDC = new int[32]; //deszyfrowanie prawa
        int[] plaintextLeftDC = new int[32];  //deszyfrowanie lewa

        int[] keyPermutedChoice56 = new int[56]; //klucz po permutated choice 1

        int[] tempRPT = new int[32];  //pomocnicza prawa textu
        int[] tempLPT = new int[32]; //pomocnicza lewa textu

        int[] Ckey = new int[28];  //lewa czesc klucza
        int[] Dkey = new int[28];  //prawa czesc klucza

        int[] keyPermutedChoice48 = new int[48]; //klucz po permutated choice 2
        int[] rightExpanded48 = new int[48];  //szyfrowanie - prawa czesc po rozszerzeoniu

        int[] leftExpanded48DC = new int[48]; // deszyfrowanie ^

        int[] XoredRPT = new int[48]; // prawa czesc textu po zxorowaniu z kluczem
        int[] XoredLPT = new int[48]; // lewa czesc textu po zxorowaniu z kluczem

        //zmienne do sboxa
        int rowIndex, columnIndex;
        int[] row = new int[2];
        int[] column = new int[4];
        int sBoxValue;

        //tymczasowe pzrechowywanie 4 bitow wychodzacych z sboxa
        int[] tempSBox = new int[4];

        //prawa czesc po wyjsciu z sboxa
        int[] plaintextSboxRPT = new int[32];
        //lewa czesc po wyjsciu z sboxa decipher
        int[] plaintextSboxLPT = new int[32];
        //prawa czesc po wyjscu z pboxa
        int[] plaintextPboxRPT = new int[32];
        //lewa czesc po wyjsciu z sboxa decipher
        int[] plaintextPboxLPT = new int[32];

        //klucz po polaczeniu po pboxie - szyfrowanie
        int[] afterPboxJoin = new int[64];

        //klucz po poloczeniu po pboxie - odszyfrowanie
        int[] afterPboxJoinDC = new int[64];

        //wynik szyfrowania
        int[] finalPermutation = new int[64];

        //wynik deszyfrowania   
        int[] finalPermutationDC = new int[64];

        int[,] allRoundsKey = new int[16, 48];

        public void assignPlainTextAndKey(int[] text, int[] key)
        {
            for(int i=0;i<64;i++)
            {
                plaintext[i] = text[i];
                keytext[i] = key[i];
            }
        }

        public void assignCipherTextAndKey(int[] text, int[] key)
        {
            for (int i = 0; i < 64; i++)
            {
                ciphertext[i] = text[i];
                keytext[i] = key[i];
            }
        }
        public int[] getEnrypction()
        {
            return this.finalPermutation;
        }

        public int[] getDecryption()
        {
            return this.finalPermutationDC;
        }

        public int[,] getAllSubkeys()
        {
            return this.allRoundsKey;
        }

        //funkcja startująca szyfrowanie
        public void startEncryption()
        {
            InitialPermutation(plaintext, afterInitialPermutationText);  //pierwsza permutacja plaintextu
            DivideTextoRLPT(afterInitialPermutationText,plaintextLeft,plaintextRight); //podzielenie textu na lewo i prawo
            TransformKeyTo56Bit(); //przekształcenie klucza do 56 bitów
            StartSixteenRounds(); // rozpoczęcie 16 rund naszego algorytmu

            JoinRPTandLPT(plaintextLeft, plaintextRight, afterPboxJoin); //polaczenie lewej i prawej po przejsciu przez 16 rund
            InversePermutation(afterPboxJoin, finalPermutation); //ostatnia odwrocona permutacja
        }

        //funkcja startujaca deszyfrowanie
        public void startDecryption()
        {
            InitialPermutation(ciphertext, afterInitialPermutationDecipher);
            DivideTextoRLPT(afterInitialPermutationDecipher, plaintextLeftDC, plaintextRightDC);

            StartReverseSixteenRounds();

            JoinRPTandLPT(plaintextLeftDC, plaintextRightDC, afterPboxJoinDC);
            InversePermutation(afterPboxJoinDC, finalPermutationDC);
        }

        private void InversePermutation(int[] afterPboxJoin, int[] finalPerm)
        {
            int temp;
            for (int i = 0; i < 64; i++)
            {
                temp = InversePermutationTable[i];
                finalPerm[i] =afterPboxJoin[temp - 1];
            }
        }

        private void JoinRPTandLPT(int[] plaintextLeft, int[] plaintextRight, int[] afterPboxJoin)
        {
            for (int i = 0; i < 32; i++)
            {
                afterPboxJoin[i] = plaintextRight[i];
            }
            int k = 32;
            for (int i = 0; i < 32; i++)
            {
                afterPboxJoin[k] =plaintextLeft[i];
                k++;
            }
        }
        //część główna algorytmu  szyfrowania
        private void StartSixteenRounds()
        {
            int val;
            for(int i=0;i<16;i++)
            {
                SaveRightPlainText(plaintextRight, tempRPT);

                val = circuralLeftShiftTable[i];

                DivideKey();

                for(int j=0;j<val;j++)
                {
                    LeftShift(Ckey);
                    LeftShift(Dkey);
                }

                CombineDividedKeys();
                TransformKeyTo48Bit();

                expansionPermutation(plaintextRight,rightExpanded48);
                XOR(keyPermutedChoice48, rightExpanded48, XoredRPT, 48);
                SboxChange(XoredRPT, plaintextSboxRPT);
                PboxChange(plaintextSboxRPT, plaintextPboxRPT);
                XOR(plaintextPboxRPT, plaintextLeft, plaintextRight, 32);

                SwapLeftRight(tempRPT, plaintextLeft);
            }
        }
        //część główna algorytmu odszyfrowania
        /*
        private void StartReverseSixteenRounds()
        {
            int val;
            for(int i=0;i<16;i++)
            {
                SaveRightPlainText(plaintextLeftDC, tempLPT);
                TransformKeyTo48Bit();
                expansionPermutation(plaintextLeftDC, leftExpanded48DC);
                XOR(keyPermutedChoice48, leftExpanded48DC, XoredLPT, 48);
                SboxChange(XoredLPT, plaintextSboxLPT);
                PboxChange(plaintextSboxLPT, plaintextPboxLPT);
                XOR(plaintextPboxLPT, plaintextRightDC, plaintextLeftDC, 32);

                SwapLeftRight(tempLPT, plaintextRightDC);

                val = circuralRightShiftTable[i];
                DivideKey();

                for (int j = 0; j < val; j++)
                {
                    RightShift(Ckey);
                    RightShift(Dkey);
                }

                CombineDividedKeys();
            }
        }
        */
        private void StartReverseSixteenRounds()
        {
            for (int i = 0; i < 16; i++)
            {
                SaveRightPlainText(plaintextRightDC, tempRPT);

                int[] key48 = new int[48];

                for (int j = 0; j < 48; j++)
                {
                    key48[j] = allRoundsKey[15 - i, j];
                }

                expansionPermutation(plaintextRightDC, rightExpanded48);
                XOR(key48, rightExpanded48, XoredRPT, 48);
                SboxChange(XoredRPT, plaintextSboxRPT);
                PboxChange(plaintextSboxRPT, plaintextPboxRPT);
                XOR(plaintextPboxRPT, plaintextLeftDC, plaintextRightDC, 32);

                SwapLeftRight(tempRPT, plaintextLeftDC);
            }
        }
    //Przygotowanie wszystkich 16 kluczy
    public void Prepare16RoundsKey()
        {
            int[] tempPermutedKey = new int[56];
            int temp;
            for (int i = 0; i < 56; i++)
            {
                temp = permutedChoice1[i];
                tempPermutedKey[i] = keytext[temp - 1];
            }         

            int[] Ctempkey = new int[28];
            int[] Dtempkey = new int[28];

            int val;

            for (int i=0; i<16;i++)
            {
                //dzielenie
                for (int z = 0; z < 28; z++)
                {
                    Ctempkey[z] = tempPermutedKey[z];
                }
                int k = 0;
                for (int z = 28; z < 56; z++)
                {
                    Dtempkey[k] = tempPermutedKey[z];
                    k++;
                }
                //przesuniecie
                val = circuralLeftShiftTable[i];
                for(int h=0;h<val;h++)
                {
                    LeftShift(Ctempkey);
                    LeftShift(Dtempkey);
                }

                //polaczenie
                for (int x = 0; x < 28; x++)
                {
                    tempPermutedKey[x] = Ctempkey[x];
                }
                k = 28;
                for (int x = 0; x < 28; x++)
                {
                    tempPermutedKey[k] = Dtempkey[x];
                    k++;
                }

                for (int x = 0; x < 48; x++)
                {
                    temp = permutedChoice2[x];
                    allRoundsKey[i,x] = tempPermutedKey[temp - 1];
                }
            }
        }

        private void RightShift(int[] halfkey)
        {
            int i, last = halfkey[27];
            for(i = 27; i >=1;--i)
            {
                halfkey[i] = halfkey[i - 1];
            }
            halfkey[i] = last;
        }

        private void SwapLeftRight(int[] tempRPT, int[] plaintextLeft)
        {
            for(int i=0;i<32;i++)
            {
                plaintextLeft[i] = tempRPT[i];
            }
        }

        private void PboxChange(int[] plaintextSboxRPT, int[] plaintextPboxRPT)
        {
            int temp;
            for (int i = 0; i < 32; i++)
            {
                temp = pBoxTable[i];
                plaintextPboxRPT[i] = plaintextSboxRPT[temp - 1];
            }
        }

        private void SboxChange(int[] xoredRPT, int[] plaintextSboxRPT)
        {
            int j=0, z = 0,r,q=0;
            for(int i=0;i<48;)
            {
                row[0] = xoredRPT[i];
                row[1] = xoredRPT[i + 5];
                rowIndex = 2 * row[0] + row[1];

                column[0] = xoredRPT[i+1];
                column[1] = xoredRPT[i+2];
                column[2] = xoredRPT[i+3];
                column[3] = xoredRPT[i+4];
                columnIndex = 8 * column[0] + 4 * column[1] + 2 * column[2] + column[3];

                int temp = ((16 * rowIndex) + columnIndex);

                sBoxValue = sBoxTable[j++, temp];

                string array = Convert.ToString(sBoxValue, 2).PadLeft(4, '0').ToString();
                for (int x = 0; x < 4; x++) tempSBox[x] = (int)(array[x] - '0');

                r = q * 4;

                AssignSboxBitsToPT(tempSBox, plaintextSboxRPT, r);

                ++q;
                i = i + 6;
            }
        }

        private void AssignSboxBitsToPT(int[] tempSBox, int[] plaintextSboxRPT, int arrayStartIndex)
        {
            int temp = arrayStartIndex;
            for(int i=0;i<4;i++)
            {
                plaintextSboxRPT[temp++] = tempSBox[i];
            }
        }

        private void XOR(int[]temp1, int[] temp2, int[] temp3, int tempSize)
        {
            for(int i=0;i<tempSize;i++)
            {
                temp3[i] = temp1[i] ^ temp2[i];
            }
        }

        private void expansionPermutation(int[] plaintextRight, int[] rightExpanded48)
        {
            int temp;
            for (int i = 0; i < 48; i++)
            {
                temp = expansionPermutationTable[i];
                rightExpanded48[i] = tempRPT[temp - 1];
            }
        }

        private void TransformKeyTo48Bit()
        {
            int temp;
            //Console.WriteLine();
            //Console.WriteLine("Supa key");
            for (int i = 0; i < 48; i++)
            {
                temp = permutedChoice2[i];
                keyPermutedChoice48[i] = keyPermutedChoice56[temp-1];
                //Console.Write(keyPermutedChoice56[temp - 1]);
            }
            //Console.WriteLine();
        }

        private void CombineDividedKeys()
        {
            for (int i = 0; i < 28; i++)
            {
                keyPermutedChoice56[i] = Ckey[i];
            }
            int k = 28;
            for (int i = 0; i < 28; i++)
            {
                keyPermutedChoice56[k] = Dkey[i];
                k++;
            }
        }

        private void DivideKey()
        {
            for(int i=0;i<28;i++)
            {
                Ckey[i] = keyPermutedChoice56[i];
            }
            int k = 0;
            for(int i=28;i<56;i++)
            {
                Dkey[k] = keyPermutedChoice56[i];
                k++;
            }
        }

        private void LeftShift(int[] halfkey)
        {
            int first = halfkey[0];
            for(int i=0;i<27;i++)
            {
                halfkey[i] = halfkey[i + 1];
            }
            halfkey[27] = first;
        }

        private void SaveRightPlainText(int[] plaintextRight, int[] tempRPT)
        {
            for(int i=0;i<32;i++)
            {
                tempRPT[i] = plaintextRight[i];
            }
        }

        private void TransformKeyTo56Bit()
        {
            int temp;
            for(int i=0;i<56;i++)
            {
                temp = permutedChoice1[i];
                keyPermutedChoice56[i] = keytext[temp - 1];
            }
        }

        private void DivideTextoRLPT(int[] dividedArray, int[] LPT, int[] RPT)
        {
            for(int i=0;i<32;i++)
            {
                LPT[i] = dividedArray[i];
            }
            int k = 0;
            for(int i=32;i<64;i++)
            {
                RPT[k] = dividedArray[i];
                k++;
            }
        }

        private void InitialPermutation(int[] text, int[] saveArray)
        {
            int temp;
            for (int i = 0; i < 64; i++)
            {
                temp = initialPermutationTable[i];
                saveArray[i] = text[temp - 1];
            }
        }
    }

   
}
