using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileView : MonoBehaviour {
    [SerializeField] private TextMeshPro _text;
    private SpriteRenderer _spriteRenderer;

    public Transform Transform { get; private set; }

    public void SetColor(Color color) {
        _spriteRenderer.color = color;
    }

    public void SetText(string text) {
        _text.text = text;
    }
    
    private void Awake() {
        Transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}