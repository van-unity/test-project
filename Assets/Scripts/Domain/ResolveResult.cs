using System.Collections.Generic;

namespace Domain {
    public class ResolveResult {
        public List<BoardPos> CollectedTiles { get; }
        public Dictionary<string, TileChangeInfo> TileChangeByID { get; }

        public ResolveResult() {
            CollectedTiles = new List<BoardPos>();
            TileChangeByID = new Dictionary<string, TileChangeInfo>();
        }
    }
}