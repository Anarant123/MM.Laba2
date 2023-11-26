using KTZ.Models;
using KTZ.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KTZ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Ma();
        }

        // Пример использования
        static void Ma()
        {
            int[] suppliers = { 150, 110, 100, 90 };
            int[] consumers = { 120, 150, 80 };
            int[,] costs = { { 6, 7, 4 }, { 6, 7, 9 }, { 2, 5, 3 }, { 4, 5, 8 } };

            TransportProblemResult result = TransportProblemSolver.SolveTransportProblem(suppliers, consumers, costs);

            // Вывод результатов
            Console.WriteLine("Potentials U:");
            foreach (var u in result.U)
            {
                Console.Write(u + " ");
            }
            Console.WriteLine();

            Console.WriteLine("Potentials V:");
            foreach (var v in result.V)
            {
                Console.Write(v + " ");
            }
            Console.WriteLine();

            Console.WriteLine("Basis:");
            for (int i = 0; i < result.Basis.GetLength(0); i++)
            {
                for (int j = 0; j < result.Basis.GetLength(1); j++)
                {
                    Console.Write(result.Basis[i, j] + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
