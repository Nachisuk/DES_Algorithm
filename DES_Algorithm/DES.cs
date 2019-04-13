using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DES_Algorithm
{
    class DES
    {
        //Kącik teoretyczny aby wszystko było łatwiejsze do przemyślenia
        /*
         *Ogólnie możemy podzielić to na dwie części zaczniemy od kluacza
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
         * 18. Wykonaliśmy poprawnie szyfrowanko.
         */
        BitArray inputArray = new BitArray(64);
        BitArray keyArray = new BitArray(64);

        public DES(BitArray array)
        {
            this.inputArray = array;
        }

        //główna funckja algorytmu będzie wykonywała po kolei kroki szyfrowania
        public void DES_Cipher()
        {
            InitialPermutation();
        }

        private void InitialPermutation()
        {
            //pierwszy krok działania na tekscie do zaszyfrowania
            //tablica przetasowania naszego tekstu czyli na 1 pozycji ma być 58, na 2 ma być 50 i tak dalej
            int[] permutation = new int[] { 58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7 };
            BitArray copy = inputArray;

            for (int i = 0; i < 64; i++)
            {
                inputArray[i] = copy[permutation[i]];
            }
            //zastanowić się czy zwracamy czy zamieniamy zmienną w klasie, jak na razie zmieniamy
        }

        //funckja będzie wywoływana w tak zwanej pierwszej rundzie
        //zaraz gdy podzielimy nasz tekst na dwie 32-bitowe części
        //Funckja będzie rozszerzać naszą 32-bitową prawą część do 48 bitów według tabelki
        //w pdf z Aragorna to jest tabellka E BIT-SELECTION TABLE, schemat FIGURE 2. Calculation of f(R,K)
        private void ExpansionPermutation()
        {
            //tabelka rozszerzania prawej części tekstu doszyfrowania (czyli gdzieś trzeba zostawić oryginalny?)
            int[] expansionPermutation = new int[] { 32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1 };

            //testowe robienie bo jak na razie nie mam zmiennych
            //kopiujemy prawą część
            //BitArray copy = rightSideArray;

            //tworzymy tablice do przechowania rozszerzonej tablicy
            //BitArray expandedRightSide = new BitArray(48);

            //wypełniamy nowo utworzoną tablice według tabelki
            /*
            for (int i = 0; i < 48; i++)
            {
                expandedRightSide[i] = copy[expansionPermutation[i]];
            }
            */

            //zwracamy rozszerzoną tablice;
        }

        private void InverseInitialPermutation()
        {
            //ostatni krok algorytmu czyli znowu przetasowujemy nasz tekst na wzór InitialPermutation()
            int[] inversePermutation = new int[] { 40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25 };
            BitArray copy = inputArray;
            for (int i = 0; i < 64; i++)
            {
                inputArray[i] = copy[inversePermutation[i]];
            }
        }

        private void KeyPermutedChoice1()
        {
            //tablica zmiany naszego klucza z 64 bit do 56 bit
            int[] permutedChoice1 = new int[] { 57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4 };
            BitArray copy = keyArray;
            for (int i = 0; i < 56; i++)
            {
                copy[i] = keyArray[permutedChoice1[i]];
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

        }
        //Ostatni krok zabawy na kluczu w pojedyńczej rundzie dostajemy 48 bitowy klucz na wyjście
        private void KeyPermutedChoice2()
        {
            //tablica zmiany naszego klucza z 56bit do 48 bit
            int[] permutedChoice2 = new int[] { 14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32 };

            //jak narazie nie ma innych części algorytmu to przypisuje po prostu klucz
            //tutaj powinien być przypisany ten 56 bitowy
            //więc funkcja jak na razie nie będzie działać ale taki jest plan
            BitArray copy = keyArray;
            for (int i = 0; i < 56; i++)
            {
                copy[i] = keyArray[permutedChoice2[i]];
            }
            //zwrócić ten 48 bitowy klucz
        }

    }
}
