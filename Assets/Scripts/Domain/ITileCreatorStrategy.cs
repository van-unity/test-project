namespace Domain {
    public interface ITileCreatorStrategy {
        Tile CreateTile(BoardModel boardModel, int x, int y);
    }
}