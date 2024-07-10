namespace Yaong_Omok_Server {
    public enum BoardInfo {
        WFirst = -1,
        WSecond = -2,
        WThird = -3,

        None = 0,

        BFirst = 1,
        BSecond = 2,
        BThird = 3
    }

    public class GoBoard {
        private BoardInfo[,] _board = new BoardInfo[15, 15];

        public BoardInfo GetBoardInfo(int x, int y) {
            if(x < 0 || x > 14 || y < 0 || y > 14)
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
