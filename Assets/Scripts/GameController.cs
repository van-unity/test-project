using UnityEngine;

public class GameController : MonoBehaviour {
    [SerializeField] private BoardView _boardView;
    [SerializeField] private TileConfiguration _tileConfiguration;
    [SerializeField] private int _width = 5;
    [SerializeField] private int _height = 5;

    private void Start() {
        var boardDefinition = GenerateGrid(_tileConfiguration.GetAllTileTypes(), _width, _height);

        var boardModel = new BoardModel(boardDefinition);
        _boardView.Initialize(boardModel);
    }

    private int[,] GenerateGrid(int[] tileTypes, int width, int height) {
        var grid = new int[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int tileType;
                do {
                    tileType = tileTypes[UnityEngine.Random.Range(0, tileTypes.Length)];
                } while ((x > 1 && grid[x - 1, y] == tileType && grid[x - 2, y] == tileType) ||
                         (y > 1 && grid[x, y - 1] == tileType && grid[x, y - 2] == tileType));

                grid[x, y] = tileType;
            }
        }

        return grid;
    }
}