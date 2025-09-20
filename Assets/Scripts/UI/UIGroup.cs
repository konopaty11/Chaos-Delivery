using UnityEngine;

[System.Serializable]
public class UIGroup
{
    [Header("Fade Settings")]
    public string groupName;
    public CanvasGroup[] elements;
    public float duration;
    [HideInInspector] public bool isVisible;

    [Header("Move Settings")]
    public Vector2 finishPosition;
    public Vector2 startPosition;
    public float moveSpeed;
}
