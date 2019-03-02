using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for Battlefield.xaml
    /// </summary>
    public partial class Battlefield : UserControl
    {
        public bool IsTileSelectable { get; set; } = true;
        public Rectangle selectedTile;

        public Battlefield()
        {
            InitializeComponent();
            Rectangle tile;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    tile = new Rectangle();
                    Grid.SetRow(tile, i);
                    Grid.SetColumn(tile, j);
                    tile.StrokeThickness = 1;
                    tile.Stroke = Brushes.LightSkyBlue;
                    tile.Fill = Brushes.Transparent;
                    tile.MouseLeftButtonDown += Tile_MouseLeftButtonDown;
                    grid.Children.Add(tile);
                }
        }

        public Rectangle this[int index]
        {
            get => grid.Children[index] as Rectangle;
            set => grid.Children[index] = value;
        }
        
        private void Tile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsTileSelectable && (sender as Rectangle).Fill == Brushes.Transparent)
            {
                if (selectedTile != null)
                {
                    selectedTile.Stroke = Brushes.LightSkyBlue;
                    selectedTile.StrokeThickness = 1;
                }
                if (selectedTile != (Rectangle)sender)
                {
                    selectedTile = sender as Rectangle;
                    selectedTile.Stroke = Brushes.OrangeRed;
                    selectedTile.StrokeThickness = 3;
                }
                else
                    selectedTile = null;
            }
        }
    }
}
