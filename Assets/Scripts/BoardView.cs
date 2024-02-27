using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoardView : MonoBehaviour {
    [Serializable]
    private class TileAnimationParameters {
        [Tooltip("Delay before resolving the board and refilling")]
        public float _resolveDelay;

        public float _swapDuration = .25f;
        public Ease _swapEase = Ease.OutQuad;
        public float _fallDuration = .3f;
        public Ease _fallEase = Ease.InOutQuad;
        public Vector3 _collectedTileWobbleScale = new Vector3(0.1f, 0.1f, 0);
        public float _collectedTilesWobbleDuration = .25f;
        public Ease _collectedTilesWobbleEase = Ease.InOutBack;
        public int _collectedTilesWobbleVibrato = 5;
    }

    [SerializeField] private TileView _tileViewPrefab;
    [SerializeField] private TileConfiguration _tileConfiguration;
    [SerializeField] private float _minSwipeDelta;
    [SerializeField] private TileAnimationParameters _tileAnimationParameters;

    private BoardModel _boardModel;
    private Dictionary<BoardPos, TileView> _tileViewByPos;
    private Sequence _swapSequence;

    public void Initialize(BoardModel boardModel) {
        _boardModel = boardModel;
        _tileViewByPos = new Dictionary<BoardPos, TileView>();
        CenterCamera();
        UpdateGrid();
    }

    private void CenterCamera() {
        Camera.main.transform.position = new Vector3((_boardModel.Width - 1) * 0.5f, (_boardModel.Height - 1) * 0.5f);
    }

    private void UpdateGrid(ResolveResult resolveResult = null) {
        foreach (var tileData in _boardModel.IterateTiles()) {
            if (tileData == null) {
                continue;
            }

            var tileView = Instantiate(_tileViewPrefab, transform);
            if (_tileConfiguration.TryGetTileColorByType(tileData.Tile.TileType, out var tileColor)) {
                tileView.SetColor(tileColor);
                tileView.SetText($"[{tileData.Pos.x}, {tileData.Pos.y}]");
                _tileViewByPos[tileData.Pos] = tileView;
            }

            if (resolveResult != null) {
                if (resolveResult.TileChangeByID.TryGetValue(tileData.Tile.ID, out var changeInfo)) {
                    // var delay = 0;
                    // if (changeInfo.WasCreated) {
                    //     delay = 1;
                    // }
                    var distance = Mathf.Abs(changeInfo.ToPos.y - changeInfo.FromPos.y);
                    var duration = _tileAnimationParameters._fallDuration * distance;
                    tileView.Transform.localPosition = new Vector3(changeInfo.FromPos.x, changeInfo.FromPos.y);
                    tileView.Transform
                        .DOLocalMove(new Vector3(changeInfo.ToPos.x, changeInfo.ToPos.y),
                            duration).SetEase(_tileAnimationParameters._fallEase);
                    continue;
                }
            }

            tileView.Transform.localPosition = new Vector3(tileData.Pos.x, tileData.Pos.y, 0);
        }
    }

    private void ClearGrid() {
        foreach (var tileView in GetComponentsInChildren<TileView>()) {
            Destroy(tileView.gameObject);
        }

        _tileViewByPos.Clear();
    }

    private BoardPos ScreenPosToLogicPos(float x, float y) {
        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(x, y, Camera.main.farClipPlane));
        var boardSpace = transform.InverseTransformPoint(worldPos);

        return new BoardPos() {
            x = Mathf.RoundToInt(boardSpace.x),
            y = Mathf.RoundToInt(boardSpace.y)
        };
    }

    private Vector3 _mouseDownPosition;
    private BoardPos _swipePos;
    private bool _canSwipe;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _mouseDownPosition = Input.mousePosition;
            var pos = ScreenPosToLogicPos(_mouseDownPosition.x, _mouseDownPosition.y);

            if (_boardModel.IsWithinBounds(pos.x, pos.y)) {
                _swipePos = pos;
                _canSwipe = true;
            } else {
                _canSwipe = false;
            }
        }

        if (_canSwipe && Input.GetMouseButtonUp(0)) {
            _canSwipe = false;
            var delta = (Input.mousePosition - _mouseDownPosition);
            if (delta.sqrMagnitude < _minSwipeDelta) {
                return;
            }

            delta = delta.normalized;

            BoardPos toPos;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
                toPos = delta.x > 0
                    ? new BoardPos(_swipePos.x + 1, _swipePos.y)
                    : new BoardPos(_swipePos.x - 1, _swipePos.y);
            } else {
                toPos = delta.y > 0
                    ? new BoardPos(_swipePos.x, _swipePos.y + 1)
                    : new BoardPos(_swipePos.x, _swipePos.y - 1);
            }

            if (!_boardModel.IsWithinBounds(toPos.x, toPos.y)) {
                return;
            }

            (_tileViewByPos[_swipePos], _tileViewByPos[toPos]) = (_tileViewByPos[toPos], _tileViewByPos[_swipePos]);

            _swapSequence = DOTween.Sequence();

            _swapSequence.Join(_tileViewByPos[_swipePos].Transform
                .DOLocalMove(new Vector3(_swipePos.x, _swipePos.y), _tileAnimationParameters._swapDuration)
                .SetEase(_tileAnimationParameters._swapEase));

            _swapSequence.Join(_tileViewByPos[toPos].Transform
                .DOLocalMove(new Vector3(toPos.x, toPos.y), _tileAnimationParameters._swapDuration)
                .SetEase(_tileAnimationParameters._swapEase));

            _swapSequence.AppendInterval(_tileAnimationParameters._resolveDelay);

            _swapSequence.AppendCallback(() => {
                var resolveResult = _boardModel.Resolve(_swipePos, toPos);

                if (resolveResult != null) {
                    var wobbleSequence = DOTween.Sequence();

                    foreach (var tile in resolveResult.CollectedTiles) {
                        wobbleSequence.Join(_tileViewByPos[tile].Transform
                            .DOPunchScale(_tileAnimationParameters._collectedTileWobbleScale,
                                _tileAnimationParameters._collectedTilesWobbleDuration,
                                _tileAnimationParameters._collectedTilesWobbleVibrato)
                            .SetEase(_tileAnimationParameters._collectedTilesWobbleEase));
                    }

                    wobbleSequence.AppendCallback(() => {
                        ClearGrid();
                        UpdateGrid(resolveResult);
                    });
                }
            });
        }
    }
}