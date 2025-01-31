using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        private int Rows = 4;
        private int Cols = 4;
        private List<Card> cards;
        private Card firstFlippedCard;
        private Card secondFlippedCard;
        private DispatcherTimer timer;
        private int elapsedTime;
        private bool isPaused = false;
        private int moveCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeGame();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            elapsedTime = 0;
            TimerText.Text = "Time: 0s";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            TimerText.Text = $"Time: {elapsedTime}s";
        }

        private void InitializeGame()
        {
            cards = new List<Card>();
            firstFlippedCard = null;
            secondFlippedCard = null;
            moveCount = 0;
            MoveCounterText.Text = "Moves: 0";

            // Generiši parove slika
            List<string> imagePaths = GenerateImagePaths(Rows * Cols / 2);
            imagePaths.AddRange(imagePaths);
            imagePaths = imagePaths.OrderBy(x => Guid.NewGuid()).ToList();

            CardGrid.Children.Clear();
            CardGrid.RowDefinitions.Clear();
            CardGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < Rows; i++)
                CardGrid.RowDefinitions.Add(new RowDefinition());
            for (int j = 0; j < Cols; j++)
                CardGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Button cardButton = new Button
                    {
                        Content = new Image
                        {
                            Source = new BitmapImage(new Uri("Images/card_back.png", UriKind.Relative)),
                            Stretch = Stretch.Fill
                        },
                        Background = Brushes.LightGray,
                        Tag = imagePaths[i * Cols + j]
                    };
                    cardButton.Click += CardButton_Click;

                    Grid.SetRow(cardButton, i);
                    Grid.SetColumn(cardButton, j);
                    CardGrid.Children.Add(cardButton);

                    cards.Add(new Card { Button = cardButton, ImagePath = imagePaths[i * Cols + j], IsFlipped = false });
                }
            }

            timer.Start();
        }

        private List<string> GenerateImagePaths(int count)
        {
            List<string> paths = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                paths.Add($"Images/image{i}.jpg");
            }
            return paths;
        }

        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            Card clickedCard = cards.First(c => c.Button == clickedButton);

            if (clickedCard.IsFlipped || secondFlippedCard != null)
                return;

            clickedCard.IsFlipped = true;
            clickedButton.Content = new Image
            {
                Source = new BitmapImage(new Uri(clickedCard.ImagePath, UriKind.Relative)),
                Stretch = Stretch.Fill
            };

            if (firstFlippedCard == null)
            {
                firstFlippedCard = clickedCard;
            }
            else
            {
                secondFlippedCard = clickedCard;
                moveCount++;
                MoveCounterText.Text = $"Moves: {moveCount}";
                CheckForMatch();
            }
        }

        private async void CheckForMatch()
        {
            if (firstFlippedCard.ImagePath == secondFlippedCard.ImagePath)
            {
                firstFlippedCard.Button.IsEnabled = false;
                secondFlippedCard.Button.IsEnabled = false;

                if (cards.All(c => !c.Button.IsEnabled))
                {
                    timer.Stop();
                    MessageBox.Show($"Čestitamo! Pobedili ste za {elapsedTime} sekundi sa {moveCount} poteza!", "Kraj igre");
                }
            }
            else
            {
                await Task.Delay(1000);

                firstFlippedCard.Button.Content = new Image
                {
                    Source = new BitmapImage(new Uri("Images/card_back.png", UriKind.Relative)),
                    Stretch = Stretch.Fill
                };

                secondFlippedCard.Button.Content = new Image
                {
                    Source = new BitmapImage(new Uri("Images/card_back.png", UriKind.Relative)),
                    Stretch = Stretch.Fill
                };

                firstFlippedCard.IsFlipped = false;
                secondFlippedCard.IsFlipped = false;
            }

            firstFlippedCard = null;
            secondFlippedCard = null;
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Želite li da restartujete igru?", "Restart", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                timer.Stop();
                elapsedTime = 0;
                TimerText.Text = "Time: 0s";
                InitializeGame();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPaused)
            {
                timer.Start();
                PauseButton.Content = "Pause";
                EnableCards(true);
            }
            else
            {
                timer.Stop();
                PauseButton.Content = "Resume";
                EnableCards(false);
            }

            isPaused = !isPaused;
        }

        private void EnableCards(bool isEnabled)
        {
            foreach (var card in cards)
            {
                if (!card.IsFlipped)
                    card.Button.IsEnabled = isEnabled;
            }
        }

        private void DifficultyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DifficultyComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int size = int.Parse(selectedItem.Tag.ToString());
                Rows = size;
                Cols = size;
                RestartButton_Click(null, null);
            }
        }
    }

    public class Card
    {
        public Button Button { get; set; }
        public string ImagePath { get; set; }
        public bool IsFlipped { get; set; }
    }
}
