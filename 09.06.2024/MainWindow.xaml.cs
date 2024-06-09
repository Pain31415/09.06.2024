using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _09._06._2024
{
    public partial class MainWindow : Window
    {
        private Mutex mutex = new Mutex();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Clear();
            Task.Run(() => FirstThread());
        }

        private void FirstThread()
        {
            mutex.WaitOne();
            for (int i = 0; i <= 20; i++)
            {
                Dispatcher.Invoke(() => OutputTextBox.AppendText(i + "\n"));
                Thread.Sleep(2000);
            }
            mutex.ReleaseMutex();
            Task.Run(() => SecondThread());
        }

        private void SecondThread()
        {
            mutex.WaitOne();
            for (int i = 10; i >= 0; i--)
            {
                Dispatcher.Invoke(() => OutputTextBox.AppendText(i + "\n"));
                Thread.Sleep(2000);
            }
            mutex.ReleaseMutex();
        }
    }
}