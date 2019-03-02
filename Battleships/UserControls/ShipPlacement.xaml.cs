using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace Battleships
{
    /// <summary>
    /// Interaction logic for ShipPlacement.xaml
    /// </summary>
    public partial class ShipPlacement : UserControl
    {
        private static Random random = new Random();
        
        private Rectangle selectedShip;
        public ObservableCollection<int> ShipsCount { get; set; }

        public ShipPlacement(Player player)
        {
            InitializeComponent();
            DataContext = this;
            txtPlayer.Text = player.Number.ToString();
            field.IsTileSelectable = false;
            ShipsCount = new ObservableCollection<int>() { 4, 3, 2, 1 };
            foreach (Rectangle tile in field.grid.Children)
                tile.MouseLeftButtonDown += FieldTile_MouseLeftButtonDown;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedShip != null)
            {
                selectedShip.Opacity = 1;
                selectedShip = null;
            }
            if (selectedShip != (Rectangle)sender && ShipsCount[int.Parse((sender as Rectangle).Tag.ToString().Substring(0, 1)) - 1] > 0)
            {
                selectedShip = sender as Rectangle;
                selectedShip.Opacity = 0.5;
            }
        }

        private bool TileAvailable(int index)
        {
            try
            {
                for (int i = 0; i < selectedShip.Width / 30; i++)
                    if (field[index + i].Fill != Brushes.Transparent)
                        return false;
                for (int i = 0; i < selectedShip.Height / 3; i += 10)
                    if (field[index + i].Fill != Brushes.Transparent)
                        return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void PlaceShip(int index)
        {
            if (TileAvailable(index) && ShipsCount[int.Parse(selectedShip.Tag.ToString().Substring(0, 1)) - 1] > 0)
                if (selectedShip.Width > 30)
                {
                    if (index % 10 <= 10 - selectedShip.Width / 30)
                    {
                        for (int i = 0; i < selectedShip.Width / 30; i++)
                        {
                            field[index + i].Fill = selectedShip.Fill;
                            field[index + i].Stroke = selectedShip.Stroke;
                            field[index + i].Tag = $"{i + 1}{selectedShip.Tag}";
                        }
                        for (int i = (index % 10 == 0 ? 0 : -1); i < selectedShip.Width / 30 + (index % 10 >= 10 - selectedShip.Width / 30 ? 0 : 1); i++)
                            for (int j = -10; j < 20; j += 10)
                                if (index + i + j >= 0 && index + i + j < 100)
                                    if (field[index + i + j].Fill != selectedShip.Fill)
                                        field[index + i + j].Fill = (SolidColorBrush)Application.Current.FindResource("brushWater");
                        selectedShip.Opacity = 1;
                        --ShipsCount[int.Parse(selectedShip.Tag.ToString().Substring(0, 1)) - 1];
                        selectedShip = null;
                    }
                }
                else
                {
                    for (int i = 0; i < selectedShip.Height / 3; i += 10)
                    {
                        field[index + i].Fill = selectedShip.Fill;
                        field[index + i].Stroke = selectedShip.Stroke;
                        field[index + i].Tag = $"{i / 10 + 1}{selectedShip.Tag}";
                    }
                    for (int i = (index % 10 == 0 ? 0 : -1); i < (index % 10 == 9 ? 1 : 2); i++)
                        for (int j = -10; j < (selectedShip.Height / 30 + 1) * 10; j += 10)
                            if (index + i + j >= 0 && index + i + j < 100)
                                if (field[index + i + j].Fill != selectedShip.Fill)
                                    field[index + i + j].Fill = (SolidColorBrush)Application.Current.FindResource("brushWater");
                    selectedShip.Opacity = 1;
                    --ShipsCount[int.Parse(selectedShip.Tag.ToString().Substring(0, 1)) - 1];
                    selectedShip = null;
                }
        }

        private void FieldTile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedShip != null)
                PlaceShip(field.grid.Children.IndexOf(sender as Rectangle));
        }

        private void RandomizeShip(Rectangle shipVertical, Rectangle shipHorizontal)
        {
            while (ShipsCount[int.Parse(shipVertical.Tag.ToString().Substring(0, 1)) - 1] > 0)
            {
                selectedShip = random.Next(1, 3) == 1 ? shipVertical : shipHorizontal;
                while (selectedShip != null)
                    PlaceShip(random.Next(0, 100));
            }
        }

        public void RandomizeField()
        {
            RandomizeShip(ship4V, ship4H);
            RandomizeShip(ship3V, ship3H);
            RandomizeShip(ship2V, ship2H);
            RandomizeShip(ship1, ship1);
        }

        private void ClearField()
        {
            for (int i = 0; i < 4; i++)
                ShipsCount[i] = 4 - i;
            foreach (Rectangle tile in field.grid.Children)
            {
                tile.Fill = Brushes.Transparent;
                tile.Stroke = Brushes.LightBlue;
                tile.Tag = null;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearField();
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            ClearField();
            RandomizeField();
        }
    }
}