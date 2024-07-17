namespace Yaong_Omok_Server {
    public class Coord {
        public short x, y;

        public Coord() { }

        public Coord(short x, short y) {
            this.x = x;
            this.y = y;
        }

        public Coord(int x, int y) {
            this.x = (short)x;
            this.y = (short)y;
        }

        public Coord(Coord coord) {
            x = coord.x;
            y = coord.y;
        }

        public static Coord operator +(Coord coord1, Coord coord2) {
            return new Coord(coord1.x + coord2.x, coord1.y + coord2.y);
        }

        public static Coord operator -(Coord coord1, Coord coord2) {
            return new Coord(coord1.x - coord2.x, coord1.y - coord2.y);
        }

        public static Coord operator *(Coord coord, int multiplier) {
            return new Coord(coord.x * multiplier, coord.y * multiplier);
        }
    }
}
