using System;
using System.Collections.Generic;
using DG.Tweening;
using Domain;
using UnityEngine;

public class BoardView : MonoBehaviour {
    [Serializable]
    private class TileAnimationParameters {
        public float _swapDuration = .25f;
        public Ease _swapEase = Ease.OutQuad;
        public float _fallDuration = .3f;
        public Ease _fallEase = Ease.InOutQuad;
        public Vector3 _collectedTileWobbleScale = new Vector3(0.1f, 0.1f, 0);
        public float _collectedTilesWobbleDuration = .25f;
        public Ease _collectedTilesWobbleEase = Ease.InOutBack;
    }

    [SerializeField] private TileViewPool _tileViewPool;
    [SerializeField] private TileConfiguration _tileConfiguration;
    [SerializeField] private float _minSwipeDelta;
    [SerializeField] private TileAnimationParameters _tileAnimationParameters;

    private BoardModel _boardModel;
    private Dictionary<BoardPos, TileView> _tileViewByPos;
    private Sequence _swapSequence;
    private Vector3 _mouseDownPosition;
    private BoardPos _swipePos;
    private bool _canSwipe;

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
        var createdTilesSequence = DOTween.Sequence();
        var existingTilesSequence = DOTween.Sequence();
        var wholeSequence = DOTween.Sequence();
        
        foreach (var tileData in _boardModel.IterateTiles()) {
            if (tileData == null) {
                continue;
            }

            var tileView = _tileViewPool.Get();
            tileView.Transform.SetParent(transform);
            if (_tileConfiguration.TryGetTileColorByType(tileData.Tile.TileType, out var tileColor)) {
                tileView.SetColor(tileColor);
                tileView.SetText($"[{tileData.Pos.x}, {tileData.Pos.y}]\n{tileData.Tile.TileType}");
                _tileViewByPos[tileData.Pos] = tileView;
            }

            if (resolveResult != null) {
                if (resolveResult.TileChangeByID.TryGetValue(tileData.Tile.ID, out var changeInfo)) {
                    tileView.SetSortingOrder(changeInfo.WasCreated
                        ? _boardModel.Height - changeInfo.CreationTime
                        : 100);
                    tileView.Transform.localPosition = new Vector3(changeInfo.FromPos.x, changeInfo.FromPos.y);
                    var moveTween = tileView.Transform
                        .DOLocalMove(new Vector3(changeInfo.ToPos.x, changeInfo.ToPos.y),
                            _tileAnimationParameters._fallDuration).SetEase(_tileAnimationParameters._fallEase);

                    if (changeInfo.WasCreated) {
                        createdTilesSequence.Join(moveTween);
                    } else {
                        existingTilesSequence.Join(moveTween);
                    }

                    continue;
                }
            }

            tileView.Transform.localPosition = new Vector3(tileData.Pos.x, tileData.Pos.y, 0);
        }

        wholeSequence.Join(createdTilesSequence);
        wholeSequence.Join(existingTilesSequence);
        wholeSequence.AppendCallback(ResolveBoard);
    }

    private void ClearGrid() {
        foreach (var tileView in GetComponentsInChildren<TileView>()) {
            _tileViewPool.Return(tileView);
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
            var swipeDirection = (Input.mousePosition - _mouseDownPosition);
            if (swipeDirection.sqrMagnitude < _minSwipeDelta) {
                return;
            }

            swipeDirection = swipeDirection.normalized;

            HandleSwipe(swipeDirection);
        }
    }

    private void HandleSwipe(Vector3 normalizedSwipeDirection) {
        BoardPos toPos;
        if (Mathf.Abs(normalizedSwipeDirection.x) > Mathf.Abs(normalizedSwipeDirection.y)) {
            toPos = normalizedSwipeDirection.x > 0
                ? new BoardPos(_swipePos.x + 1, _swipePos.y)
                : new BoardPos(_swipePos.x - 1, _swipePos.y);
        } else {
            toPos = normalizedSwipeDirection.y > 0
                ? new BoardPos(_swipePos.x, _swipePos.y + 1)
                : new BoardPos(_swipePos.x, _swipePos.y - 1);
        }

        if (!_boardModel.IsWithinBounds(toPos.x, toPos.y)) {
            return;
        }

        _boardModel.Swap(_swipePos, toPos);

        (_tileViewByPos[_swipePos], _tileViewByPos[toPos]) = (_tileViewByPos[toPos], _tileViewByPos[_swipePos]);

        _swapSequence = DOTween.Sequence();

        _swapSequence.Join(_tileViewByPos[_swipePos].Transform
            .DOLocalMove(new Vector3(_swipePos.x, _swipePos.y), _tileAnimationParameters._swapDuration)
            .SetEase(_tileAnimationParameters._swapEase));

        _swapSequence.Join(_tileViewByPos[toPos].Transform
            .DOLocalMove(new Vector3(toPos.x, toPos.y), _tileAnimationParameters._swapDuration)
            .SetEase(_tileAnimationParameters._swapEase));

        _swapSequence.AppendCallback(ResolveBoard);
    }

    private void ResolveBoard() {
        var resolveResult = _boardModel.Resolve();

        if (resolveResult is { CollectedTiles: { Count: > 0 } }) {
            var wobbleSequence = DOTween.Sequence();
            resolveResult.CollectedTiles.ForEach(tile => {
                var tileWobbleSequence = WobbleCollectedTile(_tileViewByPos[tile]);
                wobbleSequence.Join(tileWobbleSequence);
            });

            wobbleSequence.AppendCallback(() => {
                ClearGrid();
                UpdateGrid(resolveResult);
            });
        }
    }

    private Sequence WobbleCollectedTile(TileView tileView) {
        var sequence = DOTween.Sequence();
        sequence.Join(tileView.Transform
            .DOScale(_tileAnimationParameters._collectedTileWobbleScale,
                _tileAnimationParameters._collectedTilesWobbleDuration)
            .SetEase(_tileAnimationParameters._collectedTilesWobbleEase));
        sequence.Join(DOTween.To(() => tileView.Alpha, tileView.SetAlpha, 0,
            _tileAnimationParameters._collectedTilesWobbleDuration));

        return sequence;
    }
}