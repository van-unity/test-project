public class TileData {
    public Tile Tile { get; }
    public BoardPos Pos { get; }

    public TileData(Tile tile, BoardPos pos) {
        Tile = tile;
        Pos = pos;
    }
}