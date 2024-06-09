using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _09._06._2024
{
    public partial class MainWindow : Window
    {
        static SemaphoreSlim semaphore = new SemaphoreSlim(3);
        static Random random = new Random();
        static StringBuilder outputBuilder = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartThreads_Click(object sender, RoutedEventArgs e)
        {
            Task[] tasks = new Task[10];

            for (int i = 0; i < 10; i++)
            {
                int threadIndex = i;
                tasks[threadIndex] = Task.Run(() => ThreadWork(threadIndex, this));
            }

            await Task.WhenAll(tasks);

            OutputTextBox.Text += "Усі потоки завершили роботу.\n";
        }

        static void ThreadWork(int threadIndex, MainWindow window)
        {
            semaphore.Wait();

            try
            {
                lock (outputBuilder)
                {
                    outputBuilder.AppendLine($"Потік {threadIndex} починає роботу");
                }

                for (int i = 0; i < 5; i++)
                {
                    int randomNumber = random.Next(1, 100);
                    lock (outputBuilder)
                    {
                        outputBuilder.AppendLine($"Потік {threadIndex}: {randomNumber}");
                    }
                    Thread.Sleep(100);
                }

                lock (outputBuilder)
                {
                    outputBuilder.AppendLine($"Потік {threadIndex} завершує роботу");
                }
            }
            finally
            {
                semaphore.Release();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    window.OutputTextBox.Text = outputBuilder.ToString();
                });
            }
        }
    }
}
