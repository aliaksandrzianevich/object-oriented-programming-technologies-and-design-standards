using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Lab02;

internal class SortingAlgorithms
{
    private readonly int[] _array;
    private readonly int[] _sortedArray;
    public int Comparisons;
    public int Swaps;
    public bool IsPaused;
    public bool IsAborted;

    public SortingAlgorithms(int[] arr)
    {
        _array = new int[arr.Length];
        Array.Copy(arr, _array, arr.Length);
        _sortedArray = new int[_array.Length];
    }

    public int[] GetSortedArray() => _sortedArray;

    private void ResetCounters()
    {
        Comparisons = 0;
        Swaps = 0;
    }

    // Алгоритм сортировки слиянием
    public void MergeSort()
    {
        Array.Copy(_array, _sortedArray, _array.Length);
        ResetCounters();
        MergeSortHelper(_sortedArray, 0, _sortedArray.Length - 1);
    }

    private void MergeSortHelper(int[] arr, int left, int right)
    {
        if (IsAborted) return;

        while (IsPaused && !IsAborted)
        {
            Thread.Sleep(100);
        }

        if (left < right)
        {
            var mid = (left + right) / 2;
            MergeSortHelper(arr, left, mid);
            MergeSortHelper(arr, mid + 1, right);
            Merge(arr, left, mid, right);
        }
    }

    private void Merge(int[] arr, int left, int mid, int right)
    {
        var temp = new int[right - left + 1];
        int i = left, j = mid + 1, k = 0;

        while (i <= mid && j <= right)
        {
            Comparisons++;
            if (arr[i] <= arr[j])
            {
                temp[k++] = arr[i++];
            }
            else
            {
                temp[k++] = arr[j++];
                Swaps++;
            }
        }

        while (i <= mid)
            temp[k++] = arr[i++];

        while (j <= right)
            temp[k++] = arr[j++];

        for (i = left, k = 0; i <= right; i++, k++)
            arr[i] = temp[k];
    }

    // Алгоритм сортировки по разрядам
    public void RadixSort()
    {
        Array.Copy(_array, _sortedArray, _array.Length);
        ResetCounters();

        var maxNum = GetMax(_sortedArray);
        for (var exp = 1; maxNum / exp > 0; exp *= 10)
        {
            if (IsAborted) return;

            while (IsPaused && !IsAborted)
                Thread.Sleep(100);

            CountingSortByDigit(_sortedArray, exp);
        }
    }

    private int GetMax(int[] arr)
    {
        var max = arr[0];
        for (var i = 1; i < arr.Length; i++)
        {
            Comparisons++;
            if (arr[i] > max)
                max = arr[i];
        }
        return max;
    }

    private void CountingSortByDigit(int[] arr, int exp)
    {
        var n = arr.Length;
        var output = new int[n];
        var count = new int[10];

        for (var i = 0; i < n; i++)
        {
            Comparisons++;
            count[(arr[i] / exp) % 10]++;
        }

        for (var i = 1; i < 10; i++)
            count[i] += count[i - 1];

        for (var i = n - 1; i >= 0; i--)
        {
            Swaps++;
            output[count[(arr[i] / exp) % 10] - 1] = arr[i];
            count[(arr[i] / exp) % 10]--;
        }

        for (var i = 0; i < n; i++)
            arr[i] = output[i];
    }
}

internal static class Program
{
    private static int[] _originalArray;
    private static SortingAlgorithms _mergeSorter;
    private static SortingAlgorithms _radixSorter;
    private static Thread _mergeThread;
    private static Thread _radixThread;
    private static Stopwatch _mergeStopwatch;
    private static Stopwatch _radixStopwatch;
    private static bool _isRunning = true;

    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Генерация случайного массива из 50000 чисел
        Console.WriteLine("Генерация массива из 50000 элементов...");
        var random = new Random();
        _originalArray = new int[50000];
        for (var i = 0; i < _originalArray.Length; i++)
            _originalArray[i] = random.Next(1, 100000);
        Console.WriteLine("Массив сгенерирован.\n");

        // Создание сортировщиков
        _mergeSorter = new SortingAlgorithms(_originalArray);
        _radixSorter = new SortingAlgorithms(_originalArray);

        // Создание потоков
        _mergeStopwatch = new Stopwatch();
        _radixStopwatch = new Stopwatch();

        _mergeThread = new Thread(RunMergeSort);
        _radixThread = new Thread(RunRadixSort);

        _mergeThread.Name = "Merge Sort Thread";
        _radixThread.Name = "Radix Sort Thread";

        // Запуск потоков
        Console.WriteLine("Запуск сортировки в двух потоках...\n");
        _mergeThread.Start();
        _radixThread.Start();

        // Обработка команд пользователя
        UserInteractionLoop();

        // Ожидание завершения потоков
        _mergeThread.Join();
        _radixThread.Join();

        // Вывод результатов
        DisplayResults();

        // Сохранение результатов в файл
        SaveResultsToFile();
    }

    private static void RunMergeSort()
    {
        Console.WriteLine("[Merge Sort] Начало сортировки слиянием...");
        _mergeStopwatch.Start();
        _mergeSorter.MergeSort();
        _mergeStopwatch.Stop();

        Console.WriteLine(!_mergeSorter.IsAborted
            ? "[Merge Sort] Сортировка завершена!"
            : "[Merge Sort] Сортировка прервана!");
    }

    private static void RunRadixSort()
    {
        Console.WriteLine("[Radix Sort] Начало поразрядной сортировки...");
        _radixStopwatch.Start();
        _radixSorter.RadixSort();
        _radixStopwatch.Stop();

        Console.WriteLine(!_radixSorter.IsAborted
            ? "[Radix Sort] Сортировка завершена!"
            : "[Radix Sort] Сортировка прервана!");
    }

    private static void UserInteractionLoop()
    {
        Console.WriteLine("\n=== Управление ===");
        Console.WriteLine("P - Пауза | R - Возобновить | S - Стоп | Q - Выход");
        Console.WriteLine("M - Статус Merge Sort | X - Статус Radix Sort\n");

        while (_isRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = char.ToUpper(Console.ReadKey(true).KeyChar);

                switch (key)
                {
                    case 'P':
                        _mergeSorter.IsPaused = true;
                        _radixSorter.IsPaused = true;
                        Console.WriteLine(">>> Сортировка ПРИОСТАНОВЛЕНА");
                        break;

                    case 'R':
                        _mergeSorter.IsPaused = false;
                        _radixSorter.IsPaused = false;
                        Console.WriteLine(">>> Сортировка ВОЗОБНОВЛЕНА");
                        break;

                    case 'S':
                        _mergeSorter.IsAborted = true;
                        _radixSorter.IsAborted = true;
                        Console.WriteLine(">>> Сортировка ОСТАНОВЛЕНА");
                        _isRunning = false;
                        break;

                    case 'Q':
                        _mergeSorter.IsAborted = true;
                        _radixSorter.IsAborted = true;
                        _isRunning = false;
                        break;

                    case 'M':
                        Console.WriteLine($">>> Merge Sort: Comparisons={_mergeSorter.Comparisons}, Swaps={_mergeSorter.Swaps}");
                        break;

                    case 'X':
                        Console.WriteLine($">>> Radix Sort: Comparisons={_radixSorter.Comparisons}, Swaps={_radixSorter.Swaps}");
                        break;
                }
            }

            Thread.Sleep(100);

            // Проверка завершения обоих потоков
            if (!_mergeThread.IsAlive && !_radixThread.IsAlive)
                _isRunning = false;
        }
    }

    private static void DisplayResults()
    {
        Console.WriteLine("\n=== РЕЗУЛЬТАТЫ СОРТИРОВКИ ===\n");

        Console.WriteLine("--- СОРТИРОВКА СЛИЯНИЕМ ---");
        Console.WriteLine($"Время выполнения: {_mergeStopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine($"Количество сравнений: {_mergeSorter.Comparisons}");
        Console.WriteLine($"Количество обменов: {_mergeSorter.Swaps}");
        Console.WriteLine($"Статус: {(_mergeSorter.IsAborted ? "Прервана" : "Завершена")}\n");

        Console.WriteLine("--- ПОРАЗРЯДНАЯ СОРТИРОВКА ---");
        Console.WriteLine($"Время выполнения: {_radixStopwatch.ElapsedMilliseconds} мс");
        Console.WriteLine($"Количество сравнений: {_radixSorter.Comparisons}");
        Console.WriteLine($"Количество обменов: {_radixSorter.Swaps}");
        Console.WriteLine($"Статус: {(_radixSorter.IsAborted ? "Прервана" : "Завершена")}\n");

        // Сравнение
        if (!_mergeSorter.IsAborted && !_radixSorter.IsAborted)
        {
            Console.WriteLine("--- СРАВНЕНИЕ ---");
            var timeDiff = _radixStopwatch.ElapsedMilliseconds - _mergeStopwatch.ElapsedMilliseconds;
            switch (timeDiff)
            {
                case > 0:
                    Console.WriteLine($"Сортировка слиянием быстрее на {timeDiff} мс");
                    break;
                case < 0:
                    Console.WriteLine($"Поразрядная сортировка быстрее на {Math.Abs(timeDiff)} мс");
                    break;
                default:
                    Console.WriteLine("Время выполнения одинаковое!");
                    break;
            }

            // Проверка корректности сортировки
            Console.WriteLine("\nПроверка корректности:");
            var mergeSorted = IsSorted(_mergeSorter.GetSortedArray());
            var radixSorted = IsSorted(_radixSorter.GetSortedArray());

            Console.WriteLine($"Merge Sort отсортирован корректно: {(mergeSorted ? "ДА" : "НЕТ")}");
            Console.WriteLine($"Radix Sort отсортирован корректно: {(radixSorted ? "ДА" : "НЕТ")}");
        }
    }

    private static bool IsSorted(int[] arr)
    {
        for (var i = 1; i < arr.Length; i++)
        {
            if (arr[i] < arr[i - 1])
                return false;
        }
        return true;
    }

    private static void SaveResultsToFile()
    {
        try
        {
            const string filePath = "sort_results.txt";
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("=== РЕЗУЛЬТАТЫ СОРТИРОВКИ ===");
                writer.WriteLine($"Размер массива: {_originalArray.Length}\n");

                writer.WriteLine("--- СОРТИРОВКА СЛИЯНИЕМ ---");
                writer.WriteLine($"Время выполнения: {_mergeStopwatch.ElapsedMilliseconds} мс");
                writer.WriteLine($"Количество сравнений: {_mergeSorter.Comparisons}");
                writer.WriteLine($"Количество обменов: {_mergeSorter.Swaps}");
                writer.WriteLine($"Статус: {(_mergeSorter.IsAborted ? "Прервана" : "Завершена")}\n");

                writer.WriteLine("--- ПОРАЗРЯДНАЯ СОРТИРОВКА ---");
                writer.WriteLine($"Время выполнения: {_radixStopwatch.ElapsedMilliseconds} мс");
                writer.WriteLine($"Количество сравнений: {_radixSorter.Comparisons}");
                writer.WriteLine($"Количество обменов: {_radixSorter.Swaps}");
                writer.WriteLine($"Статус: {(_radixSorter.IsAborted ? "Прервана" : "Завершена")}\n");

                writer.WriteLine("--- ОТСОРТИРОВАННЫЙ МАССИВ (первые 100 элементов) ---");
                if (!_mergeSorter.IsAborted)
                {
                    var sortedArray = _mergeSorter.GetSortedArray();
                    for (var i = 0; i < Math.Min(100, sortedArray.Length); i++)
                    {
                        writer.Write(sortedArray[i] + " ");
                        if ((i + 1) % 10 == 0)
                            writer.WriteLine();
                    }
                }

                writer.WriteLine("\n--- ПОЛНАЯ СТАТИСТИКА ---");
                writer.WriteLine("Сортировка завершена успешно!");
            }

            Console.WriteLine($"\nРезультаты сохранены в файл: {Path.GetFullPath(filePath)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }
}