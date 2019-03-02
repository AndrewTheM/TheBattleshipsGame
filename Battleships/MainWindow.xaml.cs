using System;
using System.Windows;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Shapes;
using WPFLocalization;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Player player1;
        private Player player2;

        public MainWindow()
        {
            InitializeComponent();
            // *бібліотека класів для локалізації взята з інтернету
            LocalizationManager.UICulture = new CultureInfo("en-US");
            GoToMenu();
        }

        private void GoToMenu()
        {
            MenuScreen mainMenu = new MenuScreen();
            mainMenu.btnStart.Click += StartButton_Click;
            mainMenu.btnStartTwo.Click += StartTwoButton_Click;
            mainMenu.btnRules.Click += RulesButton_Click;
            mainMenu.btnExit.Click += ExitButton_Click;
            currentScreen.Content = mainMenu;
            btnMenu.Visibility = btnTopRules.Visibility = Visibility.Hidden;
        }

        private void WinMenuButton_Click(object sender, RoutedEventArgs e)
        {
            GoToMenu();
        }

        private void StartGame(bool againstComputer)
        {
            player1 = new Player(1);
            player1.Placement.btnConfirm.Click += PlacementConfirmButton_Click;
            player1.Board.btnConfirm.Click += ConfirmTurnButton_Click;
            player2 = new Player(2);
            player2.IsComputer = againstComputer;
            if (againstComputer)
                player2.Nickname = "Computer";
            else
            {
                player2.Placement.btnConfirm.Click += PlacementConfirmButton_Click;
                player2.Board.btnConfirm.Click += ConfirmTurnButton_Click;
            }
            currentScreen.Content = player1.Placement;
            btnMenu.Visibility = btnTopRules.Visibility = Visibility.Visible;
        }

        private void PlayAgainButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame(player2.IsComputer);
        }

        private bool IsShipSunk(int index, PlayerBoard enemyBoard)
        {
            string tag = enemyBoard.fieldShips[index].Tag.ToString();
            int first = index;
            switch (tag[tag.Length - 1])
            {
                case 'V':
                    {
                        first -= int.Parse(tag.Substring(0, 1)) * 10 - 10;
                        for (int i = 0; i < int.Parse(tag.Substring(1, 1)) * 10; i += 10)
                            if (enemyBoard.fieldShips[first + i].Fill != Brushes.DarkRed)
                                return false;
                        break;
                    }
                case 'H':
                    {
                        first -= int.Parse(tag.Substring(0, 1)) - 1;
                        for (int i = 0; i < int.Parse(tag.Substring(1, 1)); i++)
                            if (enemyBoard.fieldShips[first + i].Fill != Brushes.DarkRed)
                                return false;
                        break;
                    }
            }
            return true;
        }

        private void DestroyShip(Battlefield field, Battlefield enemyField, int index)
        {
            if (index >= 0 && index < 100)
                if (field[index].Fill == Brushes.Transparent)
                    field[index].Fill = enemyField[index].Fill = (ImageBrush)FindResource("missImageBrush");
                else if (field[index].Fill == (ImageBrush)FindResource("crossImageBrush"))
                {
                    field[index].Stroke = Brushes.Red;
                    field[index].StrokeThickness = enemyField[index].StrokeThickness = 4;
                    enemyField[index].Stroke = Brushes.DarkRed;
                }
        }

        private bool AttackSelectedTile(Player player, Player enemy)
        {
            Battlefield field = player.Board.fieldEnemies, enemyField = enemy.Board.fieldShips;
            if (field.selectedTile != null)
            {
                int index = field.grid.Children.IndexOf(field.selectedTile);
                if (enemyField[index].Fill != Brushes.Transparent)
                {
                    field.selectedTile.Fill = (ImageBrush)FindResource("crossImageBrush");
                    enemyField[index].Fill = Brushes.DarkRed;
                    (player.Board.DataContext as PlayerViewModel).Damage++;
                    if (IsShipSunk(index, enemy.Board))
                    {
                        string tag = enemyField[index].Tag.ToString();
                        switch (tag[tag.Length - 1])
                        {
                            case 'V':
                                {
                                    index -= int.Parse(tag.Substring(0, 1)) * 10 - 10;
                                    for (int i = (index % 10 == 0 ? 0 : -1); i < (index % 10 == 9 ? 1 : 2); i++)
                                        for (int j = -10; j < (int.Parse(tag.Substring(1, 1)) + 1) * 10; j += 10)
                                            DestroyShip(field, enemyField, index + i + j);
                                    break;
                                }
                            case 'H':
                                {
                                    index -= int.Parse(tag.Substring(0, 1)) - 1;
                                    for (int i = (index % 10 == 0 ? 0 : -1); i < int.Parse(tag.Substring(1, 1)) + (index % 10 >= 10 - int.Parse(tag.Substring(1, 1)) ? 0 : 1); i++)
                                        for (int j = -10; j < 20; j += 10)
                                            DestroyShip(field, enemyField, index + i + j);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        field.selectedTile.Stroke = Brushes.LightSkyBlue;
                        field.selectedTile.StrokeThickness = 1;
                    }
                    field.selectedTile = null;
                    if (player.Damage == 20)
                    {
                        WinnerScreen winnerScreen = new WinnerScreen(player);
                        winnerScreen.btnAgain.Click += PlayAgainButton_Click;
                        winnerScreen.btnMenu.Click += WinMenuButton_Click;
                        currentScreen.Content = winnerScreen;
                        return true;
                    }
                }
                else
                {
                    field.selectedTile.Fill = (ImageBrush)FindResource("missImageBrush");
                    enemyField[index].Fill = (ImageBrush)FindResource("missImageBrush");
                    field.selectedTile.Stroke = Brushes.LightSkyBlue;
                    field.selectedTile.StrokeThickness = 1;
                    field.selectedTile = null;
                    if (!player.IsComputer && !enemy.IsComputer)
                    {
                        TurnScreen turnScreen = new TurnScreen(enemy);
                        turnScreen.btnReady.Click += ReadyButton_Click;
                        currentScreen.Content = turnScreen;
                    }
                    return true;
                }
            }
            else
                MessageBox.Show(LocalizationManager.ResourceManager.GetString("TileMessage"));
            return false;
        }

        private void ConfirmTurnButton_Click(object sender, RoutedEventArgs e)
        {
            if (!player2.IsComputer)
                if (currentScreen.Content == player1.Board)
                    AttackSelectedTile(player1, player2);
                else
                    AttackSelectedTile(player2, player1);
            else if (AttackSelectedTile(player1, player2) && player1.Damage < 20)
            {
                currentScreen.Content = player1.Board;
                Random random = new Random();
                bool miss = true;
                int index = -1;
                do
                {
                    if (!miss)
                        switch (random.Next(0, 4))
                        {
                            case 0: index++; break;
                            case 1: index--; break;
                            case 2: index += 10; break;
                            case 3: index -= 10; break;
                        }
                    if (miss || index < 0 || index > 99)
                        index = random.Next(0, 100);
                    player2.Board.fieldEnemies.selectedTile = player2.Board.fieldEnemies[index];
                    if (player2.Board.fieldEnemies.selectedTile.Fill == Brushes.Transparent)
                        miss = AttackSelectedTile(player2, player1);
                } while (!miss);
            }
        }

        private bool PlaceShips(ShipPlacement placement, PlayerBoard board)
        {
            foreach (int count in placement.ShipsCount)
                if (count != 0)
                {
                    MessageBox.Show(LocalizationManager.ResourceManager.GetString("ShipsMessage"));
                    return false;
                }
            Rectangle tile;
            board.fieldShips.grid.Children.Clear();
            for (int i = 0; i < 100; i++)
            {
                tile = placement.field[0];
                if (tile.Fill == (SolidColorBrush)Application.Current.FindResource("brushWater"))
                    tile.Fill = Brushes.Transparent;
                placement.field.grid.Children.RemoveAt(0);
                board.fieldShips.grid.Children.Add(tile);
            }
            return true;
        }

        private void PlacementConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentScreen.Content == player1.Placement)
                if (player1.Placement.boxNickname.Text != string.Empty)
                    if (!player1.Placement.boxNickname.Text.Contains(" "))
                    {
                        if (PlaceShips(player1.Placement, player1.Board))
                        {
                            (player1.Board.DataContext as PlayerViewModel).Nickname = player1.Placement.boxNickname.Text;
                            if (!player2.IsComputer)
                                currentScreen.Content = player2.Placement;
                            else
                            {
                                player2.Placement.RandomizeField();
                                PlaceShips(player2.Placement, player2.Board);
                                currentScreen.Content = player1.Board;
                            }
                        }
                    }
                    else
                        MessageBox.Show(LocalizationManager.ResourceManager.GetString("SpacesMessage"));
                else
                    MessageBox.Show(LocalizationManager.ResourceManager.GetString("EmptyMessage"));
            else if (player2.Placement.boxNickname.Text != string.Empty)
                if (!player1.Placement.boxNickname.Text.Contains(" "))
                {
                    if (PlaceShips(player2.Placement, player2.Board))
                    {
                        (player2.Board.DataContext as PlayerViewModel).Nickname = player2.Placement.boxNickname.Text;
                        TurnScreen turnScreen = new TurnScreen(player1);
                        turnScreen.btnReady.Click += ReadyButton_Click;
                        currentScreen.Content = turnScreen;
                    }
                }
                else
                    MessageBox.Show(LocalizationManager.ResourceManager.GetString("SpacesMessage"));
            else
                MessageBox.Show(LocalizationManager.ResourceManager.GetString("EmptyMessage"));
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            currentScreen.Content = (currentScreen.Content as TurnScreen).player.Board;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame(true);
        }

        private void StartTwoButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame(false);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(LocalizationManager.ResourceManager.GetString("MenuMessage"), string.Empty, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                GoToMenu();
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(LocalizationManager.ResourceManager.GetString("RulesMessage"), LocalizationManager.ResourceManager.GetString("RulesTitleText"));
        }

        private void LanguageButton_Click(object sender, RoutedEventArgs e)
        {
            string locale = string.Empty;
            switch (LocalizationManager.UICulture.Name)
            {
                case "en-US": locale = "uk-UA"; break;
                case "uk-UA": locale = "ru-RU"; break;
                case "ru-RU": locale = "en-US"; break;
            }
            LocalizationManager.UICulture = new CultureInfo(locale);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
