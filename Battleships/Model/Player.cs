namespace Battleships
{
    public class Player
    {
        public string Nickname { get; set; }
        public int Number { get; set; }
        public bool IsComputer { get; set; }
        public int Damage { get; set; }
        public ShipPlacement Placement { get; set; }
        public PlayerBoard Board { get; set; }

        public Player(int number)
        {
            Number = number;
            IsComputer = false;
            Damage = 0;
            Placement = new ShipPlacement(this);
            Board = new PlayerBoard(this);
        }
    }
}
