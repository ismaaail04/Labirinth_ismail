using System;
using System.Collections.Generic;

class Program
{
    static char[,] lab; //Двумерный массив для представления лабиринта
    static int playerX, playerY; //Координаты игрока по оси X
    static int exitX, exitY; //Координаты выхода по оси X
    static Random random = new Random(); //Объект для генерации случайных чисел

    static void Main()
    {
        int size = 21; //Размер лабиринта
        Initializelab(size);
        Generatelab();
        WherePlayerExit();
        Printlab();

        //Цикл игры
        while (true)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.I)
            {
                ShowShortestPath();
            }
            else
            {
                MovePlayer(keyInfo.Key);
            }

            //Проверка на выход игрока
            if (playerX == exitX && playerY == exitY)
            {
                Console.SetCursorPosition(0, size);
                Console.WriteLine("Поздравляем! Вы прошли лабиринт!");
                break;
            }
        }
    }

    //Инициализация лабиринта
    static void Initializelab(int size)
    {
        lab = new char[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                lab[i, j] = '#';
            }
        }
    }
    //Генерация лабиринта
    static void Generatelab()
    {
        int startX = 1, startY = 1;
        lab[startX, startY] = ' ';
        RandomStep(startX, startY);
    }

    static void RandomStep(int x, int y)
    {
        int[] dx = { 2, -2, 0, 0 };
        int[] dy = { 0, 0, 2, -2 };

        //Рандом направлений
        for (int i = 0; i < 4; i++)
        {
            int randIndex = random.Next(4);
            int temp = dx[i];
            dx[i] = dx[randIndex];
            dx[randIndex] = temp;

            temp = dy[i];
            dy[i] = dy[randIndex];
            dy[randIndex] = temp;
        }

        for (int i = 0; i < 4; i++)
        {
            int newX = x + dx[i];
            int newY = y + dy[i];

            if (IsInBounds(newX, newY) && lab[newX, newY] == '#')
            {
                lab[newX, newY] = ' ';
                lab[x + dx[i] / 2, y + dy[i] / 2] = ' ';
                RandomStep(newX, newY);
            }
        }
    }

    //Проверка позиции в пределах лабиринта
    static bool IsInBounds(int x, int y)
    {
        return x > 0 && x < lab.GetLength(0) && y > 0 && y < lab.GetLength(1);
    }

    //Нахождение игрока и выхода
    static void WherePlayerExit()
    {
        playerX = 1;
        playerY = 1;
        lab[playerX, playerY] = 'P';

        exitX = lab.GetLength(0) - 2;
        exitY = lab.GetLength(1) - 2;
        lab[exitX, exitY] = 'E';
    }

    //Вывод лабиринта
    static void Printlab()
    {
        Console.Clear();
        for (int i = 0; i < lab.GetLength(0); i++)
        {
            for (int j = 0; j < lab.GetLength(1); j++)
            {
                Console.Write(lab[i, j]);
            }
            Console.WriteLine();
        }
    }

    //Перемещение игрока
    static void MovePlayer(ConsoleKey key)
    {
        int oldX = playerX;
        int oldY = playerY;

        if (key == ConsoleKey.W && IsMoveValid(playerX - 1, playerY))
        {
            playerX--;
        }
        else if (key == ConsoleKey.S && IsMoveValid(playerX + 1, playerY))
        {
            playerX++;
        }
        else if (key == ConsoleKey.A && IsMoveValid(playerX, playerY - 1))
        {
            playerY--;
        }
        else if (key == ConsoleKey.D && IsMoveValid(playerX, playerY + 1))
        {
            playerY++;
        }

        //Новая позиция игрока
        Console.SetCursorPosition(oldY, oldX);
        Console.Write(' ');

        Console.SetCursorPosition(playerY, playerX);
        Console.Write('P');
    }

    //Поиск кратчайшего пути
    static void ShowShortestPath()
    {
        Queue<(int x, int y)> queue = new Queue<(int, int)>();
        bool[,] visited = new bool[lab.GetLength(0), lab.GetLength(1)];
        (int x, int y)[,] prev = new (int x, int y)[lab.GetLength(0), lab.GetLength(1)];

        queue.Enqueue((playerX, playerY));
        visited[playerX, playerY] = true;

        bool found = false;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            //Проверка на выход
            if (x == exitX && y == exitY)
            {
                found = true;
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (IsInBounds(newX, newY) && !visited[newX, newY] && (lab[newX, newY] == ' ' || lab[newX, newY] == 'E'))
                {
                    queue.Enqueue((newX, newY));
                    visited[newX, newY] = true;
                    prev[newX, newY] = (x, y);
                }
            }
        }

        //Вывод кратчайшего пути на экран
        if (found)
        {
            var path = new List<(int x, int y)>();
            int cx = exitX, cy = exitY;

            while (cx != playerX || cy != playerY)
            {
                path.Add((cx, cy));
                (cx, cy) = prev[cx, cy];
            }

            foreach (var (px, py) in path)
            {
                Console.SetCursorPosition(px, py);
                Console.Write('!');
            }
        }
    }

    static bool IsMoveValid(int x, int y)
    {
        return x >= 0 && x < lab.GetLength(0) && y >= 0 && y < lab.GetLength(1) && (lab[x, y] == ' ' || lab[x, y] == 'E');
    }

  
}
