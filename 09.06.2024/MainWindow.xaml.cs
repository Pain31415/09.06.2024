using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace _09._06._2024
{
    public partial class MainWindow : Window
    {
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartDay_Click(object sender, RoutedEventArgs e)
        {
            ReportListBox.Items.Clear();

            List<Player> players = new List<Player>();
            int numPlayers = random.Next(20, 101);

            for (int i = 0; i < numPlayers; i++)
            {
                Player player = new Player($"Гравець {i + 1}", random.Next(-1000, 1001));
                Thread playerThread = new Thread(() => PlayGame(player));
                playerThread.Start();
                players.Add(player);
            }

            foreach (Player player in players)
            {
                string result = $"{player.Name} [{player.InitialMoney}] [{player.Money}]";
                ReportListBox.Items.Add(new ResultViewModel(result, GetColor(player)));
            }

            WriteReport(players);
        }

        private void PlayGame(Player player)
        {
            while (player.Status == "Playing")
            {
                int betAmount = random.Next(1, Math.Abs(player.Money) + 1);
                int betNumber = random.Next(1, 37);

                if (!player.PlaceBet(betAmount, betNumber))
                    break;

                Thread.Sleep(100);
            }
        }

        private void WriteReport(List<Player> players)
        {
            using (StreamWriter writer = new StreamWriter("report.txt"))
            {
                foreach (Player player in players)
                {
                    player.FinishDay();
                    string result = $"{player.Name} [{player.InitialMoney}] [{player.Money}]";
                    writer.WriteLine(result);
                }
            }

            MessageBox.Show("Симуляція казино завершена. Подивіться звіт у файлі report.txt.", "Завершено", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private SolidColorBrush GetColor(Player player)
        {
            if (player.Money > 0)
                return Brushes.LimeGreen;
            else if (player.Money == 0)
                return Brushes.Yellow;
            else
                return Brushes.Red;
        }
    }

    public class Player
    {
        private static readonly object locker = new object();
        private Random random = new Random();

        public string Name { get; }
        public int InitialMoney { get; }
        public int Money { get; private set; }
        public string Status { get; private set; }
        public int WinCount { get; private set; }

        public Player(string name, int money)
        {
            Name = name;
            InitialMoney = money;
            Money = money;
            Status = "Playing";
            WinCount = 0;
        }

        public bool PlaceBet(int amount, int number)
        {
            lock (locker)
            {
                if (Money <= 0)
                {
                    Status = "Lost";
                    return false;
                }

                Money -= amount;

                int rouletteNumber = random.Next(1, 37);
                if (rouletteNumber == number)
                {
                    Money += amount * 2;
                    Status = "Won";
                    WinCount++;
                }

                return true;
            }
        }

        public void FinishDay()
        {
            if (Money == 0)
                Status = "Finished";
        }
    }

    public class ResultViewModel
    {
        public string Result { get; }
        public SolidColorBrush Color { get; }

        public ResultViewModel(string result, SolidColorBrush color)
        {
            Result = result;
            Color = color;
        }
    }
}