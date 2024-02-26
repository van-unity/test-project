using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileConfiguration : MonoBehaviour {
    [Serializable]
    private class TileColorByType {
        public int tileType;
        public Color tileColor;
    }

    [SerializeField] private TileColorByType[] _tileColors;

    private Dictionary<int, Color> _tileColorLookup;

    public bool TryGetTileColorByType(int tileType, out Color tileColor) {
        if (_tileColorLookup.TryGetValue(tileType, out var color)) {
            tileColor = color;

            return true;
        }

        tileColor = Color.clear;

        return false;
    }

    public int[] GetAllTileTypes() {
        return _tileColorLookup.Select(tileTypeByColor => tileTypeByColor.Key).ToArray();
    }

    private void Awake() {
        _tileColorLookup = new Dictionary<int, Color>();

        foreach (var tileColorByType in _tileColors) {
            _tileColorLookup[tileColorByType.tileType] = tileColorByType.tileColor;
        }

        _tileColors = null;
    }
}