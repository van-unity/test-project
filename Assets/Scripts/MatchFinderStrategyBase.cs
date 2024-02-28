using System.Collections.Generic;
using Domain;
using Extensions;
using UnityEngine;

public class MatchFinderStrategyBase : IMatchFinderStrategy {
    public List<BoardPos> FindMatches(BoardModel boardModel) {
        var matches = new List<BoardPos>();

        foreach (var tileData in boardModel.IterateTiles()) {
            if (tileData == null) {
                continue;
            }

            var leftPos = new BoardPos(tileData.Pos.x - 1, tileData.Pos.y);
            var rightPos = new BoardPos(tileData.Pos.x + 1, tileData.Pos.y);
            var topPos = new BoardPos(tileData.Pos.x, tileData.Pos.y + 1);
            var bottomPos = new BoardPos(tileData.Pos.x, tileData.Pos.y - 1);

            var rightMatches =
                boardModel.FindMatchesDirection(rightPos, tileData.Tile.TileType, Vector2Int.right); // right
            var leftMatches = boardModel.FindMatchesDirection(leftPos, tileData.Tile.TileType, Vector2Int.left); // left
            var hasHorizontalMatch = false;
            if (rightMatches.Count + leftMatches.Count >= 2) {
                hasHorizontalMatch = true;
                leftMatches.ForEach(pos => boardModel.SetAt(pos.x, pos.y, null));
                rightMatches.ForEach(pos => boardModel.SetAt(pos.x, pos.y, null));
                matches.AddRange(rightMatches);
                matches.AddRange(leftMatches);
            }

            var topMatches = boardModel.FindMatchesDirection(topPos, tileData.Tile.TileType, Vector2Int.up); // top
            var bottomMatches =
                boardModel.FindMatchesDirection(bottomPos, tileData.Tile.TileType, Vector2Int.down); // bottom
            var hasVerticalMatch = false;
            if (topMatches.Count + bottomMatches.Count >= 2) {
                hasVerticalMatch = true;
                topMatches.ForEach(pos => boardModel.SetAt(pos.x, pos.y, null));
                bottomMatches.ForEach(pos => boardModel.SetAt(pos.x, pos.y, null));
                matches.AddRange(topMatches);
                matches.AddRange(bottomMatches);
            }

            //if there is a match in any direction we safely add current tile 
            if (hasVerticalMatch || hasHorizontalMatch) {
                boardModel.SetAt(tileData.Pos.x, tileData.Pos.y, null);
                matches.Add(tileData.Pos);
            }
        }

        if (matches.Count < 3) {
            matches.Clear();
            return null;
        }

        return matches;
    }
}