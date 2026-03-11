using System;
using System.Diagnostics; //merjenje časa izvajanja programa
using System.Threading.Tasks; //knižnica za vzporedno (parallel) izvajanje kode

namespace SortingSeminar
{
    class Program
    {
        /*
           prag za uporabo InsertionSort
           če je segment tabele manjši od 16 elementov,
           je InsertionSort hitrejši kot QuickSort
        */
        const int INSERTION_THRESHOLD = 16;

        static void Main(string[] args)
        {
            //napis za začetek rograma
            Console.WriteLine("Program se je začel izvajati.");
            //določimo velikost testnega niza
            int size = 200000;

            Console.WriteLine("Ustvarjanje naključnih števil...");

            //ustvarimo tabelo naključnih števil
            int[] data = GenerateRandomArray(size);

            //pričetek testiranja
            Console.WriteLine("Začetek testiranja algoritmov...");

            //za vsak algoritem izvedemo test hitrosti
            TestAlgorithm("Bubble Sort", data, BubbleSort);
            TestAlgorithm("Quick Sort", data, QuickSort);
            TestAlgorithm("Intro Sort (Optimiziran)", data, IntroSort);
            TestAlgorithm("Parallel Intro Sort (Optimiziran)", data, ParallelIntroSort);

            //konec
            Console.WriteLine("\nProgram je končal izvajanje, pritisni neko tiško za izhod.\nHVALA.");

            //počakamo na pritisk tipke, da se konzola ne zapre takoj
            Console.ReadKey();
        }

        //
        //fUNKCIJA ZA TESTIRANJE ALGORITMA
        //

        //ta funkcija izmeri čas izvajanja določenega algoritma
        static void TestAlgorithm(string name, int[] original, Action<int[]> algorithm)
        {
            //naredimo kopijo originalnega niza da vsi algoritmi dobijo enake podatke
            int[] arr = (int[])original.Clone();

            //stopwatch meri čas izvajanja kode
            Stopwatch sw = Stopwatch.StartNew();

            //izvedemo algoritem razvrščanja
            algorithm(arr);

            //ustavimo merjenje časa
            sw.Stop();

            //izpišemo čas izvajanja in preverimo ali je tabela pravilno razvrščena
            Console.WriteLine($"{name} : {sw.Elapsed.TotalSeconds} s | Sorted: {IsSorted(arr)}");
        }

        //
        //GENERATOR NAKLJUČNIH PODATKOV
        //

        //funkcija ustvari tabelo naključnih števil
        static int[] GenerateRandomArray(int size)
        {
            Random rand = new Random();

            //ustvarimo tabelo želene velikosti
            int[] arr = new int[size];

            // Napolnimo tabelo z naključnimi števili
            for (int i = 0; i < size; i++)
            {
                arr[i] = rand.Next();
            }

            return arr;
        }

        //
        //PREVERJANJE ALI JE NIZ SORTIRAN
        //

        //funkcija preveri ali je tabela pravilno razvrščena to naredimo tako, da preverimo vsak sosednji element
        static bool IsSorted(int[] arr)
        {
            for (int i = 1; i < arr.Length; i++)
            {
                //če je trenutni element manjši od prejšnjega potem tabela ni pravilno razvrščena
                if (arr[i] < arr[i - 1])
                    return false;
            }

            return true;
        }

        //
        //BUBBLE SORT (klasičen algoritem)
        //

        //bubblesort je zelo preprost algoritem primerja sosednje elemente in jih po potrebi zamenja
        static void BubbleSort(int[] arr)
        {
            int n = arr.Length;

            //večkrat prehodimo tabelo
            for (int i = 0; i < n - 1; i++)
            {
                bool swapped = false;

                for (int j = 0; j < n - i - 1; j++)
                {
                    //če je levi element večji od desnega ju zamenjamo
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(arr, j, j + 1);
                        swapped = true;
                    }
                }

                //če ni bilo nobene zamenjave pomeni da je tabela že urejena
                if (!swapped)
                    break;
            }
        }

        //
        //QUICK SORT (klasičen algoritem)
        //

        //quicksort je zelo hiter algoritem uporablja princip "divide and conquer"
        static void QuickSort(int[] arr)
        {
            QuickSortRec(arr, 0, arr.Length - 1);
        }

        //rekurzivna funkcija za QuickSort
        static void QuickSortRec(int[] arr, int low, int high)
        {
            //če segment nima več elementov
            if (low >= high)
                return;

            //razdelimo tabelo glede na pivot element
            int pivot = Partition(arr, low, high);

            //rekurzivno sortiramo levi del
            QuickSortRec(arr, low, pivot - 1);

            //rekurzivno sortiramo desni del
            QuickSortRec(arr, pivot + 1, high);
        }

        //
        //PARTITION (razdeli tabelo)
        //

        //partition funkcija razdeli tabelo na dva dela elementi manjši od pivot-a elementi večji od pivot-a
        static int Partition(int[] arr, int low, int high)
        {
            //pivot izberemo kot zadnji element
            int pivot = arr[high];

            int i = low;

            for (int j = low; j < high; j++)
            {
                //če je element manjši od pivota
                if (arr[j] < pivot)
                {
                    Swap(arr, i, j);
                    i++;
                }
            }

            //pivot postavimo na pravilno mesto
            Swap(arr, i, high);

            return i;
        }

        //
        //INTRO SORT (OPTIMIZIRANA VERZIJA)
        //

        //introsort je hibridni algoritem združuje QuickSort, HeapSort in InsertionSort
        static void IntroSort(int[] arr)
        {
            int depthLimit = 2 * (int)Math.Log2(arr.Length);

            IntroSortRec(arr, 0, arr.Length - 1, depthLimit);
        }

        static void IntroSortRec(int[] arr, int start, int end, int depthLimit)
        {
            int size = end - start + 1;

            //za majhne segmente uporabimo InsertionSort
            if (size <= INSERTION_THRESHOLD)
            {
                InsertionSort(arr, start, end);
                return;
            }

            //če je rekurzija pregloboka uporabimo HeapSort
            if (depthLimit == 0)
            {
                HeapSort(arr, start, end);
                return;
            }

            int pivot = Partition(arr, start, end);

            IntroSortRec(arr, start, pivot - 1, depthLimit - 1);
            IntroSortRec(arr, pivot + 1, end, depthLimit - 1);
        }

        //
        //PARALLEL INTRO SORT
        //

        //paralelna verzija IntroSort algoritma uporablja več procesorskih jeder
        static void ParallelIntroSort(int[] arr)
        {
            int depthLimit = 2 * (int)Math.Log2(arr.Length);

            ParallelIntroSortRec(arr, 0, arr.Length - 1, depthLimit);
        }

        static void ParallelIntroSortRec(int[] arr, int start, int end, int depthLimit)
        {
            if (start >= end)
                return;

            int size = end - start + 1;

            //če je segment majhen uporabimo navaden IntroSort
            if (size < 10000)
            {
                IntroSortRec(arr, start, end, depthLimit);
                return;
            }

            int pivot = Partition(arr, start, end);

            //dva dela sortiramo vzporedno
            Parallel.Invoke(
                () => ParallelIntroSortRec(arr, start, pivot - 1, depthLimit - 1),
                () => ParallelIntroSortRec(arr, pivot + 1, end, depthLimit - 1)
            );
        }

        //
        //INSERTION SORT
        //

        //insertionsort deluje podobno kot razvrščanje kart element vstavi na pravilno mesto v že urejenem delu
        static void InsertionSort(int[] arr, int start, int end)
        {
            for (int i = start + 1; i <= end; i++)
            {
                int key = arr[i];
                int j = i - 1;

                //premikamo večje elemente v desno
                while (j >= start && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }

                //element postavimo na pravilno mesto
                arr[j + 1] = key;
            }
        }

        //
        //HEAP SORT
        //

        //heapsort uporablja podatkovno strukturo heap in ima vedno kompleksnost O(n log n)
        static void HeapSort(int[] arr, int start, int end)
        {
            int n = end - start + 1;

            //zgradimo heap strukturo
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(arr, n, i, start);

            //največji element premaknemo na konec
            for (int i = n - 1; i >= 0; i--)
            {
                Swap(arr, start, start + i);
                Heapify(arr, i, 0, start);
            }
        }

        //funkcija popravi heap strukturo
        static void Heapify(int[] arr, int n, int i, int offset)
        {
            int largest = i;

            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && arr[offset + left] > arr[offset + largest])
                largest = left;

            if (right < n && arr[offset + right] > arr[offset + largest])
                largest = right;

            if (largest != i)
            {
                Swap(arr, offset + i, offset + largest);
                Heapify(arr, n, largest, offset);
            }
        }

        //
        //ZAMENJAVA ELEMENTOV
        //

        //swap funkcija zamenja dva elementa v tabeli
        static void Swap(int[] arr, int i, int j)
        {//začasno shranimo prvi element drugi element damo na prvo mesto prvi element damo na drugo mesto
            int temp = arr[i]; 
            arr[i] = arr[j];   
            arr[j] = temp;     
        }
    }
}