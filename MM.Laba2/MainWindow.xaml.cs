using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Input;
using System.CodeDom;

namespace MM.Laba2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, int> factories = new Dictionary<int, int>();
        private Dictionary<int, int> shops = new Dictionary<int, int>();
        private Dictionary<int, (int, bool)> costs = new Dictionary<int, (int, bool)>();
        private Dictionary<int, int> potentials = new Dictionary<int, int>();
        private Dictionary<int, int> V = new Dictionary<int, int>();
        private Dictionary<int, int> U = new Dictionary<int, int>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommitCost(Dictionary<int, (int, bool)> dic, Panel table)
        {
            foreach (var stackPanelChild in table.Children)
            {
                if (stackPanelChild is StackPanel innerStackPanel)
                {
                    foreach (var innerChild in innerStackPanel.Children)
                    {
                        if (innerChild is TextBox textBox && textBox.Name.StartsWith("tbC"))
                        {
                            string textBoxValue = textBox.Text;
                            int key = Convert.ToInt32(textBox.Name.Substring(3, 2));
                            int number = Convert.ToInt32(textBoxValue);
                            dic.Add(key, (number, true));
                        }
                    }
                }
            }
        }

        private void CommitCompany(Dictionary<int,int> dic, Panel table, string name)
        {
            foreach (var stackPanelChild in table.Children)
            {
                if (stackPanelChild is StackPanel innerStackPanel)
                {
                    foreach (var innerChild in innerStackPanel.Children)
                    {
                        if (innerChild is TextBox textBox && textBox.Name.StartsWith(name))
                        {
                            string textBoxValue = textBox.Text;
                            int number = Convert.ToInt32(textBoxValue);
                            dic.Add(dic.Count + 1, number);
                        }
                    }
                }
            }
        }

        private void CompareAB(Dictionary<int, int> A, Dictionary<int, int> B)
        {
            var sumA = 0;
            var sumB = 0;

            foreach (var a in A)
                sumA += a.Value;

            foreach (var b in B)
                sumB += b.Value;

            var delta = Math.Abs(sumA - sumB);

            if (delta > 0)
            {
                if (sumA > sumB)
                {
                    B.Add(B.Count + 1, delta);
                    for (int i = 14; i < 54; i += 10)
                        costs.Add(i, (0, false));
                }
                else
                {
                    A.Add(A.Count + 1, delta);
                    for (int i = 41; i < 43; i++)
                        costs.Add(i, (0, false));
                }
            }
        }

        private void btnCommitFactory_Click(object sender, RoutedEventArgs e)
        {
            if (tableFactory.IsEnabled == true)
            {
                CommitCompany(factories, tableFactory, "tbF");
                tableFactory.IsEnabled = false;
                (sender as Button)!.Content = "Изменить заводы";
                return;
            }

            tableFactory.IsEnabled = true;
            (sender as Button)!.Content = "Утвердить заводы";
        }

        private void btnCommitShop_Click(object sender, RoutedEventArgs e)
        {
            if (tableShop.IsEnabled == true)
            {
                CommitCompany(shops, tableShop, "tbS");
                tableShop.IsEnabled = false;
                (sender as Button)!.Content = "Изменить магазины";
                return;
            }

            tableShop.IsEnabled = true;
            (sender as Button)!.Content = "Утвердить магазины";
        }

        private void btnCommitCost_Click(object sender, RoutedEventArgs e)
        {
            if (tableTransportationCost.IsEnabled == true)
            {
                CommitCost(costs, tableTransportationCost);
                tableTransportationCost.IsEnabled = false;
                (sender as Button)!.Content = "Изменить затраты";
                return;
            }

            tableFactory.IsEnabled = true;
            (sender as Button)!.Content = "Утвердить затраты";
        }

        private void btnGetResult_Click(object sender, RoutedEventArgs e)
        {
            CompareAB(factories, shops);
            int lastMinKey = 0;
            bool flag = true;

            while (factories.Values.Any(v => v != 0) || shops.Values.Any(v => v != 0))
            {
                var minKey = costs
                .Where(x => x.Value.Item2)
                .OrderBy(x => x.Value.Item1)
                .Select(x => x.Key)
                .FirstOrDefault();

                var cordA = minKey / 10;
                var cordB = minKey % 10;

                if (costs.All(pair => !pair.Value.Item2) && flag)
                {
                    minKey = lastMinKey;
                    if (shops.Count == 4)
                    {
                        cordA = minKey % 10;
                        cordB = 4;
                        costs[cordA * 10 + cordB] = (0, true);
                    }
                    else if (factories.Count == 5)
                    {
                        cordA = 5;
                        cordB = minKey / 10;
                        costs[cordA * 10 + cordB] = (0, true);
                    }
                    flag = false;

                    minKey = cordA * 10 + cordB;
                }
                else
                {
                    lastMinKey = minKey;
                }

                var A = factories[cordA];
                var B = shops[cordB];

                if (A > B)
                {
                    factories[cordA] -= B;
                    shops[cordB] -= B;
                    potentials[minKey] = B;
                }
                else
                {
                    factories[cordA] -= A;
                    shops[cordB] -= A;
                    potentials[minKey] = A;
                }


                if (factories[cordA] == 0)
                {
                    for (int i = minKey / 10 * 10 + 1; i <= shops.Count + minKey / 10 * 10; i++)
                    {
                        costs[i] = (costs[i].Item1, false);
                    }
                }
                if (shops[cordB] == 0)
                {
                    for (int i = minKey % 10 + 10; i <= shops.Count * 10 + minKey % 10; i += 10)
                    {
                        costs[i] = (costs[i].Item1, false);
                    }
                }
            }

            foreach (var potential in potentials)
            {
                string textBoxName = $"tbP{potential.Key}";

                TextBox textBox = FindName(textBoxName) as TextBox;

                if (textBox != null)
                {
                    textBox.Text = potential.Value.ToString();
                }
            }

            bool toCollaepsedB = true;
            for (int i = 51; i <= 54; i++)
            {
                string textBoxName = $"tbP{i}";
                TextBox textBox = FindName(textBoxName) as TextBox;
                if (textBox != null)
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                        toCollaepsedB = false;
                }
            }
            if (toCollaepsedB)
            {
                for (int i = 51; i <= 54; i++)
                {
                    string textBoxName = $"tbP{i}";
                    TextBox textBox = FindName(textBoxName) as TextBox;
                    textBox.Visibility = Visibility.Collapsed;
                    dopRow.Visibility = Visibility.Collapsed;
                }
            }

            bool toCollaepsedA = true;
            for (int i = 14; i <= 54; i += 10)
            {
                string textBoxName = $"tbP{i}";
                TextBox textBox = FindName(textBoxName) as TextBox;
                if (textBox != null)
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                        toCollaepsedA = false;
                }
            }
            if (toCollaepsedA)
            {
                for (int i = 51; i <= 54; i++)
                {
                    dopCol.Visibility = Visibility.Collapsed;
                }
            }

            U.Add(1, 0);

            foreach (var potential in potentials)
            {
                if (U.ContainsKey(potential.Key / 10) && !V.ContainsKey(potential.Key % 10))
                    V.Add(potential.Key % 10, U[potential.Key / 10] + costs[potential.Key].Item1);

                if (V.ContainsKey(potential.Key % 10) && !U.ContainsKey(potential.Key / 10))
                    U.Add(potential.Key / 10, V[potential.Key % 10] - costs[potential.Key].Item1);
            }

            foreach (var u in U)
            {

            }


            foreach (var v in V)
            {
                string textBoxName = $"tbV{v.Key}";
                TextBox textBox = FindName(textBoxName) as TextBox;
                if (textBox != null)
                    textBox.Text = v.Value.ToString();
            }

            foreach (var u in U)
            {
                string textBoxName = $"tbU{u.Key}";
                TextBox textBox = FindName(textBoxName) as TextBox;
                if (textBox != null)
                    textBox.Text = u.Value.ToString();
            }
        }
    }
}
