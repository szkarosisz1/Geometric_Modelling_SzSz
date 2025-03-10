using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Binomialis_egyutthatok_WPF
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, long> memo = new Dictionary<string, long>();

        public MainWindow()
        {
            InitializeComponent();
        }

        // Faktoriális számítás
        private long Factorial(int n)
        {
            if (n == 0 || n == 1)
                return 1;
            return n * Factorial(n - 1);
        }

        // Binomiális együttható számítása faktoriálisokkal
        private long BinomialCoefficientFactorial(int n, int k)
        {
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        // Binomiális együttható számítása rekurzívan (memoizálással)
        private long BinomialCoefficientRecursive(int n, int k)
        {
            if (k == 0 || k == n)
                return 1;

            string key = $"{n}-{k}";
            if (memo.ContainsKey(key))
                return memo[key];

            long result = BinomialCoefficientRecursive(n - 1, k - 1) + BinomialCoefficientRecursive(n - 1, k);
            memo[key] = result;
            return result;
        }

        // Bernstein polinom kiszámítása és ábrázolása
        private List<double> CalculateBernsteinPolynomial(int n, double t)
        {
            List<double> values = new List<double>();

            for (int k = 0; k <= n; k++)
            {
                double binomial = BinomialCoefficientFactorial(n, k);
                double term = binomial * Math.Pow(t, k) * Math.Pow(1 - t, n - k);
                values.Add(term);
            }

            return values;
        }

        // Gomb megnyomására történő akció: a binomiális együtthatók számítása
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            int n = int.Parse(nTextBox.Text); // N értéke a TextBox-ból
            int k = int.Parse(kTextBox.Text); // K értéke a TextBox-ból

            // Binomiális együttható számítása faktoriális módszerrel
            long binomialFactorial = BinomialCoefficientFactorial(n, k);
            factorialResultTextBlock.Text = $"Binomiális együttható (faktoriálissal): {binomialFactorial}";

            // Binomiális együttható számítása rekurzívan
            long binomialRecursive = BinomialCoefficientRecursive(n, k);
            recursiveResultTextBlock.Text = $"Binomiális együttható (rekurzívan): {binomialRecursive}";

            // Bernstein polinomok kiszámítása és megjelenítése
            double t = 0.5; // t értékét rögzíthetjük, vagy egy másik TextBox-ból olvashatjuk
            List<double> bernsteinValues = CalculateBernsteinPolynomial(n, t);

            // Eredmények kiírása
            string bernsteinOutput = "Bernstein polinom értékek: ";
            foreach (var value in bernsteinValues)
            {
                bernsteinOutput += $"{value:F4} ";
            }
            bernsteinResultTextBlock.Text = bernsteinOutput;
        }
    }
}
