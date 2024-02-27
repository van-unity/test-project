using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private BoardView _boardView;
    [SerializeField] private TileConfiguration _tileConfiguration;
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;

    private void Start() {
        var tileGenerator = new TileGenerator(_tileConfiguration.GetAllTileTypes());
        var boardModel = new BoardModel(_width, _height, tileGenerator);
        _boardView.Initialize(boardModel);

        Application.targetFrameRate = 60;
    }
}