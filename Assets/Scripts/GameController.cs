using Domain;
using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private BoardView _boardView;
    [SerializeField] private TileConfiguration _tileConfiguration;
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;

    private void Start() {
        var tileGenerator = new TileCreatorStrategyBase(_tileConfiguration.GetAllTileTypes());
        var matchFinderStrategy = new MatchFinderStrategyBase();
        var boardSettings = new BoardSettings { Width = _width, Height = _height };
        var boardModel = new BoardModel(boardSettings, tileGenerator, matchFinderStrategy);
        _boardView.Initialize(boardModel);

        Application.targetFrameRate = 60;
    }
}