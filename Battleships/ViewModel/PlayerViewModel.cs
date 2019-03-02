using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Battleships
{
    public class PlayerViewModel : INotifyPropertyChanged
    {
        private Player player;

        public PlayerViewModel(Player player)
        {
            this.player = player;
        }

        public int Number
        {
            get => player.Number;
            set => player.Number = value;
        }

        public string Nickname
        {
            get => player.Nickname;
            set
            {
                player.Nickname = value;
                OnPropertyChanged("Nickname");
            }
        }

        public int Damage
        {
            get => player.Damage;
            set
            {
                player.Damage = value;
                OnPropertyChanged("Damage");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
