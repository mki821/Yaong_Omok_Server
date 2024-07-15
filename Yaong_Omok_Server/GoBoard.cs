namespace Yaong_Omok_Server {
    public enum BoardInfo : short {
        WFirst = -1,
        WSecond = -2,
        WThird = -3,
        WDeny = -4,

        None = 0,

        BFirst = 1,
        BSecond = 2,
        BThird = 3,
        BDeny = 4
    }

    public class GoBoard {
        private static readonly int BoardSize = 15;

        private BoardInfo[,] _board = new BoardInfo[BoardSize, BoardSize];

        public BoardInfo[,] Board { get => _board; }

        public BoardInfo GetBoardInfo(int x, int y) {
            if(x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
                return BoardInfo.None;

            return _board[y, x];
        }

        public BoardInfo GetBoardInfo(Coord coord) {
            int x = coord.x, y = coord.y;

            if(x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
                return BoardInfo.None;

            return _board[y, x];
        }

        public void SetBoardInfo(int x, int y, BoardInfo info) {
            if(x < 0 || x > 14 || y < 0 || y > 14) 
                return;

            _board[y, x] = info;
        }

        public void SetBoardInfo(Coord coord, BoardInfo info) {
            int x = coord.x, y = coord.y;

            if(x < 0 || x > 14 || y < 0 || y > 14)
                return;

            _board[y, x] = info;
        }

        public void Move(Move move) {
            switch(move.type) {
                case MoveType.Put:
                    Put(move);
                    break;
                case MoveType.Union:
                    Union(move);
                    break;
                case MoveType.Push:
                    Push(move);
                    break;
                case MoveType.Kill:
                    Kill(move);
                    break;
            }
        }

        public void Put(Move move) {
            SetBoardInfo(move.coord.x, move.coord.y, (BoardInfo)move.team);
        }

        public void Push(Move move) {
            SetBoardInfo(move.coord, BoardInfo.None);

            Coord direction = Yaong_Omok_Server.Move.DirectionPos[(int)move.direction];

            int count = 0;
            BoardInfo next = GetBoardInfo(move.coord + direction);
            while(next != BoardInfo.None) {
                next = GetBoardInfo(move.coord + direction * ++count);
            }

            for(int i = count; i > 0; --i) {
                BoardInfo before = GetBoardInfo(move.coord + direction * (i - 1));
                SetBoardInfo(move.coord + direction * i, before);
            }
        }

        public void Union(Move move) {
            BoardInfo upgrade = (BoardInfo)(Math.Abs((int)GetBoardInfo(move.coord)) + 1);

            SetBoardInfo(move.coord, BoardInfo.None);

            Coord direction = Yaong_Omok_Server.Move.DirectionPos[(int)move.direction];
            SetBoardInfo(move.coord + direction, upgrade);
        }

        public void Kill(Move move) {
            SetBoardInfo(move.coord, BoardInfo.None);

            Coord direction = Yaong_Omok_Server.Move.DirectionPos[(int)move.direction];
            SetBoardInfo(move.coord + direction, (BoardInfo)(3 * (int)move.team));
        }
    }
}
