namespace Domain {
    public struct BoardPos {
        public int x;
        public int y;

        public BoardPos(int x, int y) {
            this.x = x;
            this.y = y;
        }
    
        public override string ToString() => $"[{x}, {y}]";
    }
}