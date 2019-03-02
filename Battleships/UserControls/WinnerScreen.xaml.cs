using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for WinnerScreen.xaml
    /// </summary>
    public partial class WinnerScreen : UserControl
    {
        public WinnerScreen(Player player)
        {
            InitializeComponent();
            DataContext = new PlayerViewModel(player);
        }
    }
}
