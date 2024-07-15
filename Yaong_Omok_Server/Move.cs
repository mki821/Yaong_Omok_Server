namespace Yaong_Omok_Server {
    public enum TeamColor : short {
        White = -1,
        Black = 1
    }

    public enum MoveType : short {
        Put = 0,
        Union,
        Push,
        Kill
    }

    public enum Direction : short {
        Left = 0,
        Right,
        Up,
        Down,
    }

    public class Move {
        public MoveType type;
        public Coord coord;
        public TeamColor team;
        public Direction direction;

        public static readonly Coord[] DirectionPos = [
            new(-1, 0),
            new(1, 0),
            new(0, 1),
            new(0, -1)
        ];

        public Move(MoveType type, Coord coord, TeamColor team) {
            this.type = type;
            this.coord = coord;
            this.team = team;
        }

        public Move(MoveType type, Coord coord, Direction direction, TeamColor team) {
            this.type = type;
            this.coord = coord;
            this.direction = direction;
            this.team = team;
        }
    }
}
