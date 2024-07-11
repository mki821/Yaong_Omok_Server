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

        public BoardInfo GetBoardInfo(int x, int y) {
            if(x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
                return BoardInfo.None;

            return _board[y, x];
        }

        public void SetBoardInfo(int x, int y, BoardInfo info) {
            if(x < 0 || x > 14 || y < 0 || y > 14) 
                return;

            _board[y, x] = info;
        }
    }
}
