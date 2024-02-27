using System.Collections.Generic;
using UnityEngine;

public interface ITileGenerator {
    /// <summary>
    /// Creates a new tile in a given position so there will be no matches
    /// </summary>
    /// <param name="currentGrid">Current state of the board</param>
    /// <param name="x">Row Index</param>
    /// <param name="y">Column Index</param>
    /// <returns></returns>
    Tile CreateTile(Tile[,] currentGrid, int x, int y);
}

public class TileGenerator : ITileGenerator {
    private readonly int[] _tileTypes;

    public TileGenerator(int[] tileTypes) {
        _tileTypes = tileTypes;
    }

    public Tile CreateTile(Tile[,] currentGrid, int x, int y) {
        int tileType;
        do {
            tileType = _tileTypes[Random.Range(0, _tileTypes.Length)];
        } while ((x > 1 && (currentGrid[x - 1, y] != null && currentGrid[x - 1, y].TileType == tileType) &&
                  currentGrid[x - 2, y] != null && currentGrid[x - 2, y].TileType == tileType) ||
                 (y > 1 && (currentGrid[x, y - 1] != null && currentGrid[x, y - 1].TileType == tileType) &&
                  currentGrid[x, y - 2] != null && currentGrid[x, y - 2].TileType == tileType));

        return Tile.Create(tileType);
    }
}