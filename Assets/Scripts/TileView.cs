using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class TileView : MonoBehaviour {
    [SerializeField] private TextMeshPro _text;
    private SpriteRenderer _spriteRenderer;

    public Transform Transform { get; private set; }
    public float Alpha => _spriteRenderer.color.a;

    public void SetColor(Color color) {
        _spriteRenderer.color = color;
    }

    public void SetText(string text) {
        _text.text = text;
    }

    public void SetAlpha(float alpha) {
        var color = _spriteRenderer.color;
        color.a = alpha;
        _spriteRenderer.color = color;
    }

    public void SetSortingOrder(int sortingOrder) => _spriteRenderer.sortingOrder = sortingOrder;

    private void Awake() {
        Transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}