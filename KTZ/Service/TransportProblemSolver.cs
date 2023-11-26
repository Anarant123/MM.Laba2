using KTZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTZ.Service
{
    public class TransportProblemSolver
    {
        public static TransportProblemResult SolveTransportProblem(int[] suppliers, int[] consumers, int[,] costs)
        {
            int m = suppliers.Length;
            int n = consumers.Length;

            int[,] basis = new int[m, n];  // Массив базисов
            int[] u = new int[m];          // Массив потенциалов U
            int[] v = new int[n];          // Массив потенциалов V

            // Шаг 1: Находим начальное базисное решение
            while (true)
            {
                // Ищем невыделенную ячейку с максимальной отрицательной стоимостью
                int maxNegativeCost = int.MinValue;
                int selectedRow = -1, selectedCol = -1;

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (basis[i, j] == 0 && costs[i, j] < maxNegativeCost)
                        {
                            maxNegativeCost = costs[i, j];
                            selectedRow = i;
                            selectedCol = j;
                        }
                    }
                }

                // Если не нашли отрицательных стоимостей, базисное решение найдено
                if (maxNegativeCost >= 0)
                    break;

                // Построение цикла
                int[] markedRows = new int[m];
                int[] markedCols = new int[n];
                MarkCycle(basis, selectedRow, selectedCol, markedRows, markedCols);

                // Шаг 2: Улучшение базисного решения
                ImproveBasis(basis, markedRows, markedCols);
            }

            // Шаг 3: Расчет потенциалов
            CalculatePotentials(basis, costs, u, v);

            // Возвращение результата
            return new TransportProblemResult
            {
                U = u,
                V = v,
                Basis = basis
            };
        }

        // Метод для обозначения цикла в базисе
        private static void MarkCycle(int[,] basis, int row, int col, int[] markedRows, int[] markedCols)
        {
            int m = basis.GetLength(0);
            int n = basis.GetLength(1);

            markedRows[row] = 1;

            for (int j = 0; j < n; j++)
            {
                if (basis[row, j] == 1 && markedCols[j] == 0)
                {
                    markedCols[j] = 1;

                    for (int i = 0; i < m; i++)
                    {
                        if (basis[i, j] == -1 && markedRows[i] == 0)
                        {
                            MarkCycle(basis, i, j, markedRows, markedCols);
                            return;
                        }
                    }
                }
            }
        }

        // Метод для улучшения базиса в соответствии с обозначенным циклом
        private static void ImproveBasis(int[,] basis, int[] markedRows, int[] markedCols)
        {
            int m = basis.GetLength(0);
            int n = basis.GetLength(1);

            int theta = int.MaxValue;

            // Находим минимальное значение в невыделенных ячейках цикла
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (markedRows[i] == 0 && markedCols[j] == 1)
                    {
                        theta = Math.Min(theta, basis[i, j]);
                    }
                }
            }

            // Изменяем базис в соответствии с найденным минимальным значением
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (markedRows[i] == 1 && markedCols[j] == 0)
                    {
                        basis[i, j] += theta;
                    }
                    else if (markedRows[i] == 0 && markedCols[j] == 1)
                    {
                        basis[i, j] -= theta;
                    }
                }
            }
        }

        // Метод для расчета потенциалов
        private static void CalculatePotentials(int[,] basis, int[,] costs, int[] u, int[] v)
        {
            int m = basis.GetLength(0);
            int n = basis.GetLength(1);

            u[0] = 0; // Задаем начальное значение для потенциала U

            // Расчет потенциалов U
            for (int i = 1; i < m; i++)
            {
                u[i] = costs[i, basis[i, 0]] - v[basis[i, 0]];
            }

            v[0] = costs[basis[0, 0], 0]; // Задаем начальное значение для потенциала V

            // Расчет потенциалов V
            for (int j = 1; j < n; j++)
            {
                v[j] = costs[basis[0, j], j] - u[basis[0, j]];
            }
        }
    }
}
