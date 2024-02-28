using System.Collections.Generic;

namespace Domain {
    public class BoardModel {
        private readonly Tile[,] _state;
        private readonly ITileCreatorStrategy _tileCreatorStrategy;
        private readonly IMatchFinderStrategy _matchFinderStrategy;

        public int Width { get; }
        public int Height { get; }

        public BoardModel(BoardSettings settings, ITileCreatorStrategy tileCreatorStrategy,
            IMatchFinderStrategy matchFinderStrategy) {
            _tileCreatorStrategy = tileCreatorStrategy;
            _matchFinderStrategy = matchFinderStrategy;

            Width = settings.Width;
            Height = settings.Height;

            _state = new Tile[Width, Height];

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    SetAt(x, y, tileCreatorStrategy.CreateTile(this, x, y));
                }
            }
        }

        public Tile GetAt(int x, int y) {
            if (IsWithinBounds(x, y)) {
                return _state[x, y];
            }

            return null;
        }

        public void SetAt(int x, int y, Tile value) {
            if (IsWithinBounds(x, y)) {
                _state[x, y] = value;
                return;
            }

            //handle the case
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

            var matches = _matchFinderStrategy.FindMatches(this);
            var result = new ResolveResult(matches);
            bool moreToResolve;
            var resolveStep = 0;

            do {
                moreToResolve = TryMoveTilesDown(result);
                moreToResolve |= CreateTilesAtTop(result, resolveStep++);
            } while (moreToResolve);

            return result;
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

        private bool CreateTilesAtTop(ResolveResult resolveResult, int resolveStep) {
            int rowIndex = -1;

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    if (GetAt(x, y) != null) {
                        continue;
                    }

                    rowIndex = y;
                    break;
                }
            }
        
            if (rowIndex == -1) {
                return false;
            }
        
            var createdAnyPieces = false;
            for (int x = 0; x < Width; x++) {
                if (GetAt(x, rowIndex) == null) {
                    var pos = new BoardPos(x, rowIndex);
                    var tile = _tileCreatorStrategy.CreateTile(this, x, rowIndex);
                    SetAt(x, rowIndex, tile);
                    createdAnyPieces = true;

                    resolveResult.TileChangeByID.Add(tile.ID,
                        new TileChangeInfo {
                            CreationTime = resolveStep,
                            WasCreated = true,
                            ToPos = pos,
                            FromPos = new BoardPos(x, Height + 1)
                        });
                }
            }

            return createdAnyPieces;
        }
    }
}