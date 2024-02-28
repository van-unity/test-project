using System.Collections.Generic;
using Domain;
using UnityEngine;

namespace Extensions {
    public static class BoardModelExtensions {
        public static List<BoardPos> FindMatchesDirection(
            this BoardModel boardModel,
            BoardPos current,
            int matchValue,
            Vector2Int direction
        ) {
            var matches = new List<BoardPos>();

            while (true) {
                if (!boardModel.IsWithinBounds(current.x, current.y) ||
                    boardModel.GetAt(current.x, current.y) == null ||
                    boardModel.GetAt(current.x, current.y).TileType != matchValue) {
                    break;
                }

                matches.Add(current);

                current.x += direction.x;
                current.y += direction.y;
            }

            return matches;
        }
    }
}