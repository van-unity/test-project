using Domain;
using Extensions;
using UnityEngine;

public interface ITileCreatorStrategy {
    Tile CreateTile(BoardModel boardModel, int x, int y);
}

public class TileCreatorStrategyBase : ITileCreatorStrategy {
    private readonly int[] _tileTypes;

    public TileCreatorStrategyBase(int[] tileTypes) {
        _tileTypes = tileTypes;
    }

    public Tile CreateTile(BoardModel boardModel, int x, int y) {
        int tileType;

        do {
            tileType = _tileTypes[Random.Range(0, _tileTypes.Length)];
        } while (IsMatch(boardModel, new BoardPos(x, y), tileType));

        return Tile.Create(tileType);
    }

    private bool IsMatch(BoardModel boardModel, BoardPos currentPos, int matchValue) {
        var leftPos = new BoardPos(currentPos.x - 1, currentPos.y);
        var leftMatches = boardModel.FindMatchesDirection(leftPos, matchValue, Vector2Int.left);
        if (leftMatches.Count >= 2) {
            return true;
        }

        var rightPos = new BoardPos(currentPos.x - 1, currentPos.y);
        var rightMatches = boardModel.FindMatchesDirection(rightPos, matchValue, Vector2Int.right);
        if (rightMatches.Count >= 2) {
            return true;
        }

        if (rightMatches.Count + leftMatches.Count >= 2) {
            return true;
        }

        var topPos = new BoardPos(currentPos.x, currentPos.y + 1);
        var topMatches = boardModel.FindMatchesDirection(topPos, matchValue, Vector2Int.up);
        if (topMatches.Count >= 2) {
            return true;
        }

        var bottomPos = new BoardPos(currentPos.x, currentPos.y - 1);
        var bottomMatches = boardModel.FindMatchesDirection(bottomPos, matchValue, Vector2Int.down);
        if (bottomMatches.Count >= 2) {
            return true;
        }

        if (topMatches.Count + bottomMatches.Count >= 2) {
            return true;
        }

        return false;
    }
}