using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardModel {
    private readonly Tile[,] _state;

    public int Width { get; }
    public int Height { get; }

    public BoardModel(int[,] definition) {
        Width = definition.GetLength(0);
        Height = definition.GetLength(1);

        _state = new Tile[Width, Height];

        for (int i = 0; i < definition.GetLength(0); i++) {
            for (int j = 0; j < definition.GetLength(1); j++) {
                _state[i, j] = Tile.Create(definition[i, j]);
            }
        }
    }

    public Tile GetAt(int x, int y) {
        if (IsWithinBounds(x, y)) {
            return _state[x, y];
        }

        return null;
    }

    public IEnumerable<TileData> IterateTiles() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (_state[x, y] == null) {
                    yield return null;
                } else {
                    yield return new TileData(GetAt(x, y), new BoardPos(x, y));
                }
            }
        }
    }

    public bool IsWithinBounds(int x, int y) => x < Width && y < Height && x >= 0 && y >= 0;

    public ResolveResult Resolve(BoardPos from, BoardPos to) {
        _state.Swap(from, to);

        var matches = new List<BoardPos>();

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (GetAt(x, y) == null) {
                    continue;
                }

                var matchValue = _state[x, y].TileType;
                var horizontalMatches = new List<BoardPos>();
                var verticalMatches = new List<BoardPos>();

                FindMatchesDirection(new BoardPos(x + 1, y), matchValue, horizontalMatches, 1, 0); //  right
                FindMatchesDirection(new BoardPos(x - 1, y), matchValue, horizontalMatches, -1, 0); // left
                var hasHorizontalMatch = true;
                if (horizontalMatches.Count < 2) {
                    horizontalMatches.Clear();
                    hasHorizontalMatch = false;
                } else {
                    matches.AddRange(horizontalMatches);
                }

                FindMatchesDirection(new BoardPos(x, y + 1), matchValue, verticalMatches, 0, 1); //  up
                FindMatchesDirection(new BoardPos(x, y - 1), matchValue, verticalMatches, 0, -1); // down
                var hasVerticalMatch = true;
                if (verticalMatches.Count < 2) {
                    verticalMatches.Clear();
                    hasVerticalMatch = false;
                } else {
                    matches.AddRange(verticalMatches);
                }

                if (hasVerticalMatch || hasHorizontalMatch) {
                    matches.Add(to);
                }
            }
        }

        if (matches.Count < 3) {
            matches.Clear();
            return null;
        }

        foreach (var pos in matches) {
            _state[pos.x, pos.y] = null;
        }

        var result = new ResolveResult(matches);
        bool moreToResolve;
        do {
            moreToResolve = TryMoveTilesDown(result);
        } while (moreToResolve);


        //add new tiles
        PrintBoard();
        return result;
    }

    private void PrintBoard() {
        var str = "";
        for (int j = Height - 1; j >= 0; j--) {
            for (int i = 0; i < Width; i++) {
                if (GetAt(i, j) == null) {
                    str += " N ";
                } else {
                    str += $" {GetAt(i, j).TileType} ";
                }
            }

            str += "\n";
        }

        Debug.LogError(str);
    }

    private bool TryMoveTilesDown(ResolveResult result) {
        var movedAny = false;
        for (int y = Height - 1; y >= 1; y--) {
            for (int x = 0; x < Width; x++) {
                var tileToMove = GetAt(x, y);
                if (tileToMove == null) {
                    continue;
                }

                var toPos = new BoardPos(x, y - 1);

                var dest = GetAt(toPos.x, toPos.y);
                if (dest != null) {
                    continue;
                }

                var fromPos = new BoardPos(x, y);
                _state.Swap(fromPos, toPos);
                movedAny = true;

                if (result.TileChangeByID.TryGetValue(tileToMove.ID, out var changeInfo)) {
                    changeInfo.ToPos = toPos;
                } else {
                    result.TileChangeByID.Add(tileToMove.ID,
                        new TileChangeInfo()
                            { CreationTime = 0, FromPos = fromPos, ToPos = toPos, WasCreated = false });
                }
            }
        }

        return movedAny;
    }

    public void FindMatchesDirection(BoardPos current, int matchValue, ICollection<BoardPos> matches, int dx, int dy) {
        while (true) {
            // Check bounds
            if (!IsWithinBounds(current.x, current.y)) return;

            // Check for match
            if (_state[current.x, current.y] == null || _state[current.x, current.y].TileType != matchValue) return;

            matches.Add(current);
            current = new BoardPos(current.x + dx, current.y + dy);
        }
    }
}