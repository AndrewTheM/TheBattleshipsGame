using System.Windows.Controls;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for PlayerBoard.xaml
    /// </summary>
    public partial class PlayerBoard : UserControl
    {
        public PlayerBoard(Player player)
        {
            InitializeComponent();
            DataContext = new PlayerViewModel(player);
            fieldShips.IsTileSelectable = false;
        }
    }
}
