using System.Collections.Generic;

namespace Domain {
    public interface IMatchFinderStrategy {
        List<BoardPos> FindMatches(BoardModel boardModel);
    }
}