using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DES_Algorithm
{
    class DES
    {

        /*
         *Ogólnie możemy podzielić to na dwie części zaczniemy od klucza
         * Kroki działania dla klucza:
         * 1. Mamy klucz
         * 2. Wykonujemy Permuted Choice 1
         * 3. Dzielimy klucz na dwie części
         * 4. Na każdej z części wykonujemy left shift
         * 5. Łączymy części  i wykonujemy Permuted Choice 2
         * 6. Kroki 3-5 wykonujemy 16 razy bo algorytm ma 16 rund, w których to tworzymy 16 48 bitowych subkeyów
         * 7. Trzeba zapamiętywać części klucza, bo to na nich wykonujemy cały czas LeftShift, PermutedChoice1 wykonywany jest jeden raz
         * 
         * Kroki działania dla naszego tekstu:
         * 1.Mamy nasz tekst
         * 2.Wykonujemy na nim Initial permutaion
         * 3.Dzielimy go na dwie części to jest lewą i prawą
         * 4. Rozszerzamy prawą część do 48 bitów
         * 5. XORujemy prawą część z subkeyem aktualnej rundy
         * 6. Dostajemy wyniki 48-bitowy prawej części
         * 7. Wkładamy nasz 48-bitowy wynik do Substitution boxów, które dają nam 32 bitowy wynik (jeszcze tego nie roszyfrowałem do końca
         * ale mamy 8 S-boxów, do każdego z nich wkładamy po 6 bitów (6*8=48), a każdy z tych S-boxów wypluwa nam 4 bity (z wcześniejszych 6) co daje nam 32 bity na wyjście)
         * 
         * Przerwa na opis S-Boxów:
         * S-box jest tabelką, ma 4 wiersze i 16 kolumn
         * 
         * S-box przyjmuje 6 bitów załóżmy 100110
         * 
         * na podstawie tych 6 bitów wybieramy wiersz i kolumne z której będziemy wybierać bity na wyjście
         * 
         * numer wiersza z którego odczytujemy wartości na wyjście, otrzymujemy poprzez odczytanie wartości PIERWSZEGO I OSTATNIEGO bitu (czyli wiersz zapisujemy za pomocą dwóch bitów, maksymalnie 4 co zgadza się z wielkością sboxa)
         * numer kolumny odczytujemy z pozostałych czterech bitów ( co maksymalnie daje na 16 kolumn)
         * 
         * W naszym przykładzie  Kolumna: bity(10) co daje nam 2
         * Wiersze: bity(0011) co daje nam 3
         * 
         * Teraz patrzymy jaką wartość mamy w danym S-boxie dla kolumny 2, wiersza 3
         * Dla S1-box jest to 8
         * 
         * Na wyjście dajemy zapis binarny odpowiadający dla liczby stojącej na danym miejscu  w S-boxie w naszym przypadku 8(10) = 1000(2)
         * 
         * Czyli na wyjście wychodzą 4-bity - 1000
         * Robimy to dla 8 sboxów
         * 
         * 8. Po otrzymaniu wyników z wszystkich Sboxów ponownie mamy 32 bity 
         * 9. Wykonujemy kolejną permutacje na naszych otrzymanch 32 bitach (W pdf permutacja P, strona 18)
         * 10. XORujemy naszą prawą stronę (po tych wszystkich krokach, z lewą stroną)
         * 11. Wynik naszego XORowania zostaje naszą nową prawą stroną, natomiast oryginalna prawa strona zostaje lewą stroną
         * 12. Przechodzimy do następnej rundy (nie powtarzamy już initial premuttation bo to initial, czekamy na kolejny subkey dla rundy, znowu xorujemy  prawą z kluczem, robimy sboxy, permutacje p, xorujemy z lewą i przechodzimy do następnej rundy)
         * 13. Tak robimy całe 16 rund
         * 14. Po 16 rundzie musimy zrobić swap/switch to jest prawa strona ma mieć wartość lewej strony, natomiast lewa strona ma mieć wartość prawej
         * 16. Po zmienieniu stron miejscami łączymy je do 64 bitów
         * 17. Wykonujemy na wyniku Inverse Initial Permutation
         * 18. Wykonaliśmy poprawnie szyfrowanie.
         * 
         */

        public int[] inputArray = new int[64];
        int[] keyArray = new int[64];
        int[] initialpermutation;
        int[] KEY56;
        int[] roundShift;
        int[] rightExpansion, XORarray,PostSbox;
        int round;
        int[,] LEFT, RIGHT, SUBKEYS, XorToSBox;

        public DES(int[] array, int[] key)
        {
            Array.Copy(array, inputArray, array.Length);
            Array.Copy(key, keyArray, key.Length);

            initialpermutation = new int[64];
            LEFT = RIGHT = new int[17, 32];
            KEY56 = new int[56];
            SUBKEYS = new int[17,48];
            XorToSBox = new int[8, 6];
            round = 0;
            rightExpansion = new int[48];
            XORarray = new int[48];
            PostSbox = new int[32];
            roundShift = new int[16] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };
        }

        //główna funckja algorytmu będzie wykonywała po kolei kroki szyfrowania
        public void DES_Cipher_Prepare()
        {
            //pierwsze przetasowanie
            InitialPermutation();

            //podzielenie tekstu na dwie części
            for(int i=0;i<64;i++)
            {
                if(i < 32)
                {
                    LEFT[0,i] = initialpermutation[i];
                }
                else
                {
                    RIGHT[0, i - 32] = initialpermutation[i];
                }
            }

            //początek przygotowywania kluczy, robimy klucz 56 bitowy
            KeyPermutedChoice1();

            //zainicjowanie stworzenia 16 SUBKEYÓW potrzebnych do każdej z rund kodowania
            LeftShift();

            //rozpoczęcie kodowania tekstu
            for(int j = 0; j<16 ;j++)
            {
                round = j;
                Cipher(j, 0);

                for(int i =0;i<32;i++)
                {
                    LEFT[round + 1,i] = RIGHT[round,i];
                }
            }

            //zamienienie stronami 
            for(int i=0;i<64;i++)
            {
                if (i < 32) inputArray[i] = RIGHT[16, i];
                else inputArray[i] = LEFT[16, i-32];
            }

            InverseInitialPermutation();
        }

        private void Cipher(int round, int mode)
        {
            //rozszerzamy prawą część tekstu do 48 bitów
            ExpansionPermutation(round);

            //xorujemy rozszerzoną prawą część tekstu z subkeyem danej rundy
            for(int i=0; i<48;i++)
            {
                if(mode == 0)
                {
                    XORarray[i] = XOR(rightExpansion[i], SUBKEYS[round, i]);
                }
                else if(mode == 1)
                {
                    XORarray[i] = XOR(rightExpansion[i], SUBKEYS[15-round, i]);
                }
            }

            //Czas na sboxowanie itp;
            SBOX();
            //kolejna permutacja
            permutationBox();

            //xorowanie prawej z lewą
            for(int i =0;i<32;i++)
            {
                RIGHT[round + 1, i] = XOR(LEFT[round, i], PostSbox[i]);
            }
        }

        private int XOR(int x, int y)
        {
            return (x ^ y);
        }
        private void SBOX()
        {
            int it = 0;
            for(int i=0;i<8;i++)
            {
                for(int j=0;j<6;j++)
                {
                    XorToSBox[i, j] = XORarray[it++];
                }
            }

            int bitsvalue;
            List<int> listofbytes = new List<int>();
            for(int i =0;i<8;i++)
            {
                bitsvalue = AssignValuesToSbox(i);

                string array = Convert.ToString(bitsvalue, 2).PadLeft(4, '0').ToString();

                foreach (var a in array) listofbytes.Add((int)(a - '0'));
            }

            for(int i=0;i<32;i++)
            {
                PostSbox[i] = listofbytes[i];
            }
        }
        //Funckcja z permutacją po Sboxowaniu
        private void permutationBox()
        {
            int[] permutation = new int[] { 16,  7, 20, 21, 29, 12, 28, 17,1, 15, 23, 26,5, 18, 31, 10,2,  8, 24, 14,32, 27,  3,  9,19, 13, 30,  6,22, 11,  4, 25};
            int[] copy = new int[32];
            Array.Copy(PostSbox, copy, PostSbox.Length);
            for(int i=0;i<32;i++)
            {
                PostSbox[i] = copy[permutation[i]-1];
            }
        }

        //funkcja która dla naszego 6 bitów z XOR prawej strony z kluczem przydzieli wartość z SBOXów
        private int AssignValuesToSbox(int j)
        {
            int row, column;
            int[] bytes = new int[6];
            //przypisujemy jeden wiersz z XorToSboź żeby znaleźć numer wiersza i kolumny dla Sboxa
            for(int i=0;i<6;i++)
            {
                bytes[i] = XorToSBox[j, i];
            }

            //wiersz w Sboxie
            row = bytes[0] * 2 + bytes[5];
            //kolumna w Sboxie
            column = 8 * bytes[1] + 4 * bytes[2] + 2 * bytes[3] + bytes[4];

            switch(j)
            {
                case 0:
                    return S1BOX[row, column];
                case 1:
                    return S2BOX[row, column];
                case 2:
                    return S3BOX[row, column];
                case 3:
                    return S4BOX[row, column];
                case 4:
                    return S5BOX[row, column];
                case 5:
                    return S6BOX[row, column];
                case 6:
                    return S7BOX[row, column];
                case 7:
                    return S8BOX[row, column];
                default:
                    throw new IndexOutOfRangeException();
            }

        }

        private void InitialPermutation()
        {
            //pierwszy krok działania na tekscie do zaszyfrowania
            //tablica przetasowania naszego tekstu czyli na 1 pozycji ma być 58, na 2 ma być 50 i tak dalej
            int[] permutation = new int[] { 58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7 };

            for (int i = 0; i < 64; i++)
            {
                initialpermutation[i] = inputArray[permutation[i]-1];
            }
            //zastanowić się czy zwracamy czy zamieniamy zmienną w klasie, jak na razie zmieniamy
        }

        //funckja będzie wywoływana w tak zwanej pierwszej rundzie
        //zaraz gdy podzielimy nasz tekst na dwie 32-bitowe części
        //Funckja będzie rozszerzać naszą 32-bitową prawą część do 48 bitów według tabelki
        //w pdf z Aragorna to jest tabellka E BIT-SELECTION TABLE, schemat FIGURE 2. Calculation of f(R,K)
        private void ExpansionPermutation(int rounds)
        {
            //tabelka rozszerzania prawej części tekstu doszyfrowania (czyli gdzieś trzeba zostawić oryginalny?)
            int[] expansionPermutation = new int[] { 32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1 };

            int[] rightCopy = new int[32];
            for(int z = 0; z< 32;z++)
            {
                rightCopy[z] = RIGHT[round, z];
            }
            
            //wypełniamy nowo utworzoną tablice według tabelki
            for (int i = 0; i < 48; i++)
            {
                rightExpansion[i] = rightCopy[expansionPermutation[i] - 1];
            }
        }

        private void InverseInitialPermutation()
        {
            //ostatni krok algorytmu czyli znowu przetasowujemy nasz tekst na wzór InitialPermutation()
            int[] inversePermutation = new int[] { 40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25 };
            int[] copy = new int[64];
            Array.Copy(inputArray, copy, inputArray.Length);
            for (int i = 0; i < 64; i++)
            {
                inputArray[i] = copy[inversePermutation[i]-1];
            }
        }

        private void KeyPermutedChoice1()
        {
            //tablica zmiany naszego klucza z 64 bit do 56 bit
            int[] permutedChoice1 = new int[] { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };
            //Console.WriteLine("Key pre1");
            for (int i = 0; i < 56; i++)
            {
                KEY56[i] = keyArray[permutedChoice1[i]-1];
               // Console.Write(keyArray[permutedChoice1[i] - 1]);
            }
            //zwrócić ten 56 bitowy klucz
        }

        //funckja przesuwająca wszystkie bity w naszym kluczu w lewo
        //Funkcja jest wykonywana po PermutedChoice1
        //Po wykonaniu PermutedChoice1 otrzymujemy 56 bitowy klucz
        //56 bitowy klucz dzielimy na dwie części po 28 bitów
        //Na każdej z tych części wykonujemy LeftShift
        //W zależności od aktualnej rundy( w całym algorytmie mamy 16 rund) liczba bitów o które przeswuamy klucz się zmienia
        //Ogólnie według schematu: rundy 1,2,9,16 - przesuwamy o 1 bit w lewo, w pozostałych rundach przesuwamy o 2 bity w lewo
        private void LeftShift()
        {
            int[] C, D;
            C = D = new int[28];

            //podzielenie klucza na dwie części 28 bitowe
            for (int li = 0; li<56;li++)
            {
                if (li < 28) C[li] = KEY56[li];
                else
                {
                    D[li - 28] = KEY56[li];
                }
            }

            //przesunięcie w lewo w zależności od rundy
            for (int i = 0; i < 16; i++)
            {
                //wartość o którą przesuwamy
                int shift = roundShift[i];
                //zapamiętujemy częśći klucza
                int[] backup_C = new int[32];
                int[] backup_D = new int[32];
                Array.Copy(C, backup_C, C.Length);
                Array.Copy(D, backup_D, D.Length);

                //przesuwamy części klucza o wartość
                for(int j=0;j<28;j++)
                {
                    int shiftValue = j - shift;
                    if (shiftValue < 0) shiftValue = shiftValue + 28;

                    C[j] = backup_C[shiftValue];
                   // Console.WriteLine("LeftShift C" + C[j]);
                    D[j] = backup_D[shiftValue];
                  //  Console.WriteLine("LeftShift D" + D[j]);
                }
                //tworzymy zmienną do zapamiętania nowego klucza
                int[] CD = new int[56];

                //tworzymy nowy klucz z przesuniętych części
                for (int z = 0; z < 56;z++)
                {
                    if (z < 28) CD[z] = C[z];
                    else CD[z] = D[z - 28];
                }

                //wywołujemy permuted choice 2 dla danej rundy
                //dla każdej iteracji dostaniemy 1 subkey
                KeyPermutedChoice2(i, CD);
            }

        }
        //Ostatni krok zabawy na kluczu w pojedyńczej rundzie dostajemy 48 bitowy klucz na wyjście
        private void KeyPermutedChoice2(int x,int[] CD)
        {
            //tablica zmiany naszego klucza z 56bit do 48 bit
            int[] permutedChoice2 = new int[] { 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };

            //jak narazie nie ma innych części algorytmu to przypisuje po prostu klucz
            //tutaj powinien być przypisany ten 56 bitowy
            //więc funkcja jak na razie nie będzie działać ale taki jest plan - stare komentarze

            //tworzymy subkey dla konkretnej rundy od konkrentnych CD keyów z funkcji LEFT SHIFT
            //Console.WriteLine("Key" + x);
            for (int i = 0; i < 48; i++)
            {
                SUBKEYS[x, i] = CD[permutedChoice2[i]-1];

               // Console.Write(CD[permutedChoice2[i] - 1]);
            }
           // Console.WriteLine();
        }

        public void DES_PrepareDecipher()
        {
            //pierwsze przetasowanie
            InitialPermutation();

            //podzielenie tekstu na dwie części
            for (int i = 0; i < 64; i++)
            {
                if (i < 32)
                {
                    LEFT[0, i] = initialpermutation[i];
                }
                else
                {
                    RIGHT[0, i - 32] = initialpermutation[i];
                }
            }

            //początek przygotowywania kluczy, robimy klucz 56 bitowy
            KeyPermutedChoice1();

            //zainicjowanie stworzenia 16 SUBKEYÓW potrzebnych do każdej z rund kodowania
            LeftShift();

            //rozpoczęcie kodowania tekstu
            for (int j = 0; j < 16; j++)
            {
                round = j;
                Cipher(j, 1);

                for (int i = 0; i < 32; i++)
                {
                    LEFT[round + 1, i] = RIGHT[round, i];
                }
            }

            //zamienienie stronami 
            for (int i = 0; i < 64; i++)
            {
                if (i < 32) inputArray[i] = RIGHT[16, i];
                else inputArray[i] = LEFT[16, i - 32];
            }

            InverseInitialPermutation();
        }

        int[,] S1BOX = new int[4, 16]
        {
            {14,  4, 13,  1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9,  0,  7 },
            { 0, 15,  7,  4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5,  3,  8},
            { 4,  1, 14,  8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10,  5,  0},
            { 15, 12,  8,  2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0,  6, 13}
        };
        int[,] S2BOX = new int[4, 16]
        {
            { 15,  1,  8, 14,  6, 11,  3,  4,  9,  7,  2, 13, 12,  0,  5, 10},
            { 3, 13,  4,  7, 15,  2,  8, 14, 12,  0,  1, 10,  6,  9, 11,  5},
            { 0, 14,  7, 11, 10,  4, 13,  1,  5,  8, 12,  6,  9,  3,  2, 15},
            { 13,  8, 10,  1,  3, 15,  4,  2, 11,  6,  7, 12,  0,  5, 14,  9 }
        };
        int[,] S3BOX = new int[4, 16]
       {
            { 10,  0,  9, 14,  6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8},
            { 13,  7,  0,  9,  3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1},
            { 13,  6,  4,  9,  8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7},
            { 1, 10, 13,  0,  6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12}
       };
        int[,] S4BOX = new int[4, 16]
       {
            {7, 13, 14,  3,  0,  6,  9, 10,  1,  2,  8,  5, 11, 12,  4, 15 },
            { 13,  8, 11,  5,  6, 15,  0,  3,  4,  7,  2, 12,  1, 10, 14,  9},
            { 10,  6,  9,  0, 12, 11,  7, 13, 15,  1,  3, 14,  5,  2,  8,  4},
            { 3, 15,  0,  6, 10,  1, 13,  8,  9,  4,  5, 11, 12,  7,  2, 14}
       };
        int[,] S5BOX = new int[4, 16]
       {
            {2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13,  0, 14,  9 },
            {14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3,  9,  8,  6 },
            {4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6,  3,  0, 14 },
            {11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10,  4,  5,  3}
       };
        int[,] S6BOX = new int[4, 16]
       {
            { 12,  1, 10, 15,  9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11},
            { 10, 15,  4,  2,  7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8},
            { 9, 14, 15,  5,  2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6},
            { 4,  3,  2, 12,  9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13}
       };
        int[,] S7BOX = new int[4, 16]
       {
            { 4, 11,  2, 14, 15,  0,  8, 13,  3, 12,  9,  7,  5, 10,  6,  1},
            { 13,  0, 11,  7,  4,  9,  1, 10, 14,  3,  5, 12,  2, 15,  8,  6},
            {  1,  4, 11, 13, 12,  3,  7, 14, 10, 15,  6,  8,  0,  5,  9,  2},
            { 6, 11, 13,  8,  1,  4, 10,  7,  9,  5,  0, 15, 14,  2,  3, 12}
       };
        int[,] S8BOX = new int[4, 16]
       {
            { 13,  2,  8,  4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7},
            {  1, 15, 13,  8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14,  9,  2},
            {  7, 11,  4,  1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8},
            {  2,  1, 14,  7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11}
       };

    }
}
