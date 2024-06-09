using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _09._06._2024
{
    public partial class MainWindow : Window
    {
        private Mutex mutex = new Mutex();
        private int[] array = new int[10];
        private Random random = new Random();
        private int maxValue;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Clear();
            InitializeArray();
            Task.Run(() => FirstThread());
        }

        private void InitializeArray()
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(1, 100);
            }

            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText("Initial array: " + string.Join(", ", array) + "\n");
            });
        }

        private void FirstThread()
        {
            mutex.WaitOne();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] += random.Next(1, 10);
                Thread.Sleep(100); // Затримка для демонстрації
            }
            mutex.ReleaseMutex();
            Task.Run(() => SecondThread());
        }

        private void SecondThread()
        {
            mutex.WaitOne();
            maxValue = array.Max();
            mutex.ReleaseMutex();
            Dispatcher.Invoke(DisplayResults);
        }

        private void DisplayResults()
        {
            OutputTextBox.AppendText("Modified array: " + string.Join(", ", array) + "\n");
            OutputTextBox.AppendText("Max value: " + maxValue + "\n");
        }
    }
}