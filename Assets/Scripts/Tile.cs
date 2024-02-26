using System;

public class Tile {
    public string ID { get; }
    public int TileType { get; }

    public static Tile Create(int tileType) {
        return new Tile(tileType);
    }

    private Tile(int tileType) {
        ID = Guid.NewGuid().ToString();
        TileType = tileType;
    }
}