using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TileView : MonoBehaviour, ISpawnable {
    [SerializeField] private TextMeshPro _text;
    private SpriteRenderer _spriteRenderer;

    public GameObject GameObject { get; private set; }
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
        
    }

    public void Initialize() {
        GameObject = gameObject;
        Transform = transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        GameObject.SetActive(false);
    }

    public void OnSpawned() {
        GameObject.SetActive(true);
    }

    public void OnDeSpawned() {
        SetAlpha(1);
        Transform.localScale = Vector3.one;
        GameObject.SetActive(false);
    }
}