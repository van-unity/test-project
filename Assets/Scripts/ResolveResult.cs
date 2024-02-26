using System.Collections.Generic;

public class ResolveResult {
    public List<BoardPos> CollectedTiles { get; }
    public Dictionary<string, TileChangeInfo> TileChangeByID { get; }

    public ResolveResult(List<BoardPos> collectedTiles) {
        CollectedTiles = collectedTiles;
        TileChangeByID = new Dictionary<string, TileChangeInfo>();
    }
}