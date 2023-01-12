using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";
    }
    public void Arrange(Vector3 pos)
    {
        int sorting = Mathf.FloorToInt(pos.y) * -1;
        spriteRenderer.sortingOrder = sorting;
    }
}
