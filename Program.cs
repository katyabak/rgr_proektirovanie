using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

// алгоритм муравья для решения проблемы коммивояжера
static class Defined
{
    public const int nodeAmount = 6; // Количество узлов
    public const int numAnts = 3; // Количество муравьев
    public const int time = 1000; // Время выполнения
    public const float increase = 2.0f; // Фактор увеличения феромонов
    public const float evaporation = 0.01f; // Фактор испарения феромонов
}
public class AntColony
{
    private static Random random = new Random(0); // Инициализация генератора случайных чисел       
    private static int alpha = 3; // Влияние феромонов на направление     
    private static int beta = 1; // Влияние расстояния до соседнего узла
    private static double evaporation = Defined.evaporation;
    private static double increase = Defined.increase;
    public static void Main(string[] args)
    {
        Console.WriteLine();
        Console.WriteLine("=======================================");
        Console.WriteLine("\tАлгоритм муравья");
        Console.WriteLine("=======================================");
        int numNodes = Defined.nodeAmount; // Количество узлов
        int numAnts = Defined.numAnts; // Количество муравьев
        int maxTime = Defined.time; // Макс. время выполнения

        Console.WriteLine("Количество узлов = " + numNodes);
        Console.WriteLine("Количество муравьев = " + numAnts);
        Console.WriteLine("Время = " + maxTime);

        Console.WriteLine("Испарение феромонов = " + evaporation.ToString("F2"));
        Console.WriteLine("Увеличение феромонов = " + increase.ToString("F2"));
        Console.WriteLine("\nВлияние феромонов = " + alpha);
        Console.WriteLine("Влияние расстояния до соседнего узла  = " + beta);

        Console.WriteLine();
        int[][] distance = InitializeGraph(); // Инициализация графа расстояний               
        int[][] ants = InitAnts(numAnts, numNodes); // Инициализация муравьев

        ShowAnts(ants, distance); // Вывод информации о муравьях

        int[] bestTrail = BestTrail(ants, distance); // Определение лучшего пути
        double bestLength = DistanceOfTrail(bestTrail, distance); // Длина лучшего пути

        Console.Write("\nЛучшая длина начального пути: " + bestLength.ToString("F1") + "\n");
        Show(bestTrail);

        Console.WriteLine("\nИнициализация феромонов на тропах");
        double[][] pheromones = InitPheromones(numNodes); // Инициализация феромонов
        int time = 0;

        while (time < maxTime)
        {
            UpdateAnts(ants, pheromones, distance); // Обновление информации о муравьях
            UpdatePheromones(pheromones, ants, distance); // Обновление феромонов на тропах

            int[] currBestTrail = BestTrail(ants, distance); // Определение текущего лучшего пути
            double currBestLength = DistanceOfTrail(currBestTrail, distance); // Длина текущего лучшего пути
            if (currBestLength < bestLength)
            {
                bestLength = currBestLength;
                bestTrail = currBestTrail;
                Console.WriteLine("Найден новый лучший путь длиной " + bestLength.ToString("F1") + " в момент времени " + time);
            }
            else if (time == maxTime - 1)
            {
                Console.WriteLine("Путь лучше не найден");
            }
            time += 1;
        }
        Console.WriteLine("\nЛучший найденный путь: " + bestLength.ToString("F1"));
        Show(bestTrail);
        Console.ReadLine();
    }
    // Инициализация массива муравьев с случайными путями
    public static int[][] InitAnts(int numAnts, int numNode)
    {
        int[][] ants = new int[numAnts][];
        for (int k = 0; k <= numAnts - 1; k++)
        {
            // Выбор случайного стартового узла
            int start = random.Next(0, numNode);
            // Получение случайного пути начиная с выбранного узла
            ants[k] = TakeRandomTrail(start, numNode);
        }
        return ants;
    }
    // Создание случайного пути начиная с указанного узла
    public static int[] TakeRandomTrail(int start, int numNode)
    {
        int[] trail = new int[numNode];
        for (int i = 0; i <= numNode - 1; i++) // Заполнение пути узлами 
        {
            trail[i] = i;
        }
        for (int i = 0; i <= numNode - 1; i++) // Перетасовка Фишера-Йейтса
        {
            int x = random.Next(i, numNode);
            int tmp = trail[x];
            trail[x] = trail[i];
            trail[i] = tmp;
        }
        int idx = IndexHelper(trail, start);
        int temp = trail[0];
        trail[0] = trail[idx];
        trail[idx] = temp;

        return trail;
    }
    private static int IndexHelper(int[] trail, int target)
    {
        // вспомогательный метод для TakeRandomTrail
        for (int i = 0; i <= trail.Length - 1; i++)
        {
            if (trail[i] == target)
            {
                return i;
            }
        }
        throw new Exception();
    }
    // Вычисление длины маршрута муравья
    public static double DistanceOfTrail(int[] trail, int[][] dists)
    {
        double result = 0.0;
        for (int i = 0; i <= trail.Length - 2; i++)
        {
            // Суммирование расстояний между узлами маршрута
            result += Distance(trail[i], trail[i + 1], dists);
        }
        // Добавление расстояния от последнего узла до начального
        result += Distance(trail[trail.Length - 1], trail[0], dists);
        return result;
    }


    // Получение расстояния между двумя узлами
    private static double Distance(int X, int Y, int[][] dists)
    {
        return dists[X][Y];
    }
    // Нахождение лучшего маршрута среди муравьев
    public static int[] BestTrail(int[][] ants, int[][] dists)
    {

        double bestLength = DistanceOfTrail(ants[0], dists);
        int idxBestLength = 0;
        for (int k = 1; k <= ants.Length - 1; k++)
        {
            double len = DistanceOfTrail(ants[k], dists);
            if (len < bestLength)
            {
                bestLength = len;
                idxBestLength = k;
            }
        }
        int numNodes = ants[0].Length;
        int[] bestTrail_Renamed = new int[numNodes];
        ants[idxBestLength].CopyTo(bestTrail_Renamed, 0);
        return bestTrail_Renamed;
    }
    private static double[][] InitPheromones(int numNode) // инициализация феромонов
    {   // информация о феромонах
        double[][] pheromones = new double[numNode][];

        for (int i = 0; i <= numNode - 1; i++)
        {
            pheromones[i] = new double[numNode];
        }
        for (int i = 0; i <= pheromones.Length - 1; i++)
        {
            for (int j = 0; j <= pheromones[i].Length - 1; j++)
            {
                pheromones[i][j] = 0.01;
            }
        }
        return pheromones;
    }
    // Обновление муравьев: каждый муравей идет по новому пути
    public static void UpdateAnts(int[][] ants, double[][] pheromones, int[][] dists)
    {
        int numNode = pheromones.Length;
        for (int k = 0; k <= ants.Length - 1; k++) // Для каждого муравья
        {
            // Выбор случайного стартового узла
            int start = random.Next(0, numNode);
            // Создание нового пути муравья
            int[] newTrail = MakeTrail(k, start, pheromones, dists);
            ants[k] = newTrail;
        }
    }
    // Создание пути муравья, начиная с указанного узла
    private static int[] MakeTrail(int k, int startNode, double[][] pheromones, int[][] dists)
    {
        int numNodes = pheromones.Length;
        int[] trail = new int[numNodes];
        bool[] visited = new bool[numNodes];
        trail[0] = startNode;
        visited[startNode] = true;
        // Для каждого узла
        for (int i = 0; i <= numNodes - 2; i++)
        {
            int X = trail[i];
            // Выбор след. узла методом рулетки
            int next = NextNode_Roulette(k, X, visited, pheromones, dists);
            trail[i + 1] = next;
            visited[next] = true;
        }
        return trail;
    }
    // Выбор след. узла методом рулетки
    public static int NextNode_Roulette(int k, int X, bool[] visited, double[][] pheromones, int[][] dists)
    {
        double[] probs = MoveProbs(k, X, visited, pheromones, dists);
        double[] sumOf = new double[probs.Length + 1];
        for (int i = 0; i <= probs.Length - 1; i++)
        {
            sumOf[i + 1] = sumOf[i] + probs[i];
        }
        sumOf[sumOf.Length - 1] = 1.00;
        double p = random.NextDouble();
        // Выбор узла на основе вероятностей
        for (int i = 0; i <= sumOf.Length - 2; i++)
        {
            if (p >= sumOf[i] && p < sumOf[i + 1])
            {
                return i;
            }
        }
        return 0;
    }
    // Вычисление вероятностей переходов между узлами
    private static double[] MoveProbs(int k, int X, bool[] visited, double[][] pheromones, int[][] dists)
    {
        int numNodes = pheromones.Length;
        double[] pheromoneFactor = new double[numNodes];
        double sum = 0.0;

        // Вычисление факторов феромонов для всех узлов
        for (int i = 0; i <= pheromoneFactor.Length - 1; i++)
        {
            if (i == X)
            {
                pheromoneFactor[i] = 0.0;
            }
            else if (visited[i] == true)
            {
                pheromoneFactor[i] = 0.0;
            }
            else
            {
                pheromoneFactor[i] = Math.Pow(pheromones[X][i], alpha) * Math.Pow((1.0 / Distance(X, i, dists)), beta);

                if (pheromoneFactor[i] < 0.0001)
                {
                    pheromoneFactor[i] = 0.0001;
                }
                else if (pheromoneFactor[i] > (double.MaxValue / (numNodes * 100)))
                {
                    pheromoneFactor[i] = double.MaxValue / (numNodes * 100);
                }
            }
            sum += pheromoneFactor[i];
        }
        // Вычисление вероятностей переходов
        double[] probs = new double[numNodes];
        for (int i = 0; i <= probs.Length - 1; i++)
        {
            probs[i] = pheromoneFactor[i] / sum;
        }
        return probs;
    }

    // Обновление феромонов на путях муравьев
    public static void UpdatePheromones(double[][] pheromones, int[][] ants, int[][] dists)
    {
        for (int i = 0; i <= pheromones.Length - 1; i++)
        {
            for (int j = i + 1; j <= pheromones[i].Length - 1; j++)
            {
                for (int k = 0; k <= ants.Length - 1; k++)
                {
                    double length = DistanceOfTrail(ants[k], dists);
                    double decrease = (1.0 - evaporation) * pheromones[i][j];
                    double increase = 0.0;
                    if (EdgeInTrail(i, j, ants[k]) == true)
                    {
                        increase = AntColony.increase / length;
                    }

                    pheromones[i][j] = decrease + increase;

                    if (pheromones[i][j] < 0.0001)
                    {
                        pheromones[i][j] = 0.0001;
                    }
                    else if (pheromones[i][j] > 100000.0)
                    {
                        pheromones[i][j] = 100000.0;
                    }

                    pheromones[j][i] = pheromones[i][j];
                }
            }
        }
    }

    // Проверяет, содержится ли ребро в пути муравья
    private static bool EdgeInTrail(int X, int Y, int[] trail) // Узел X и Узел Y
    {
        int lastIndex = trail.Length - 1;
        int idx = IndexHelper(trail, X);

        if (idx == 0 && trail[1] == Y)
        { return true; }
        else if (idx == 0 && trail[lastIndex] == Y)
        { return true; }
        else if (idx == 0)
        { return false; }
        else if (idx == lastIndex && trail[lastIndex - 1] == Y)
        { return true; }
        else if (idx == lastIndex && trail[0] == Y)
        { return true; }
        else if (idx == lastIndex)
        { return false; }
        else if (trail[idx - 1] == Y)
        { return true; }
        else if (trail[idx + 1] == Y)
        { return true; }
        else
        { return false; }
    }
    public static int[][] InitializeGraph() // Инициализирование графа с заданными весами рёбер
    {
        int[][] graph1 = new int[][]
        {
        new int[] { 0, 2, 3, 5},
        new int[] { 2, 0, 6, 1},
        new int[] { 3, 6, 0, 7},
        new int[] { 5, 1, 7, 0},
        };

        int[][] graph2 = new int[][]
        {
        new int[] { 0, 2, 3, 2, 1, 5 },
        new int[] { 2, 0, 6, 2, 5, 1 },
        new int[] { 3, 6, 0, 3, 2, 7 },
        new int[] { 2, 2, 3, 0, 5, 1 },
        new int[] { 1, 5, 2, 5, 0, 9 },
        new int[] { 5, 1, 7, 1, 9, 0 },
        };
        return graph2;
    }

    // Вывод пути муравья
    private static void Show(int[] trail)
    {
        Console.Write("[");
        for (int i = 0; i <= trail.Length - 1; i++)
        {
            Console.Write(trail[i] + " ");
            if (i > 0 && i % 20 == 0)
            { Console.WriteLine(""); }
        }
        Console.WriteLine(trail[0] + "]");
    }

    // Вывод информации о муравьях 
    private static void ShowAnts(int[][] ants, int[][] dists)
    {
        for (int i = 0; i <= ants.Length - 1; i++)
        {
            Console.Write(i + ": [ ");

            for (int j = 0; j <= Defined.nodeAmount - 1; j++)
            {
                Console.Write(ants[i][j] + " ");
                if (j == Defined.nodeAmount - 1)
                {
                    Console.Write(ants[i][j - (Defined.nodeAmount - 1)] + " ");
                }
            }
            Console.Write("] длина = ");
            double len = DistanceOfTrail(ants[i], dists);
            Console.Write(len.ToString("F1"));
            Console.WriteLine("");
        }
    }
    // Вывод информации о феромонах
    private static void Display(double[][] pheromones)
    {
        for (int i = 0; i <= pheromones.Length - 1; i++)
        {
            Console.Write(i + ": ");
            for (int j = 0; j <= pheromones[i].Length - 1; j++)
            {
                Console.Write(pheromones[i][j].ToString("F4").PadLeft(8) + " ");
            }
            Console.WriteLine("");
        }
    }
}


