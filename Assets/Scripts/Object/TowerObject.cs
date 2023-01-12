using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    [SerializeField] private GameObject range;
    private SpriteRenderer spriteRenderer;

    private Tower data;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "Character";

        range.SetActive(false);
    }
    public void Init(Tower data, Vector3 pos)
    {
        this.data = data;

        int sorting = Mathf.FloorToInt(pos.y) * -1;
        spriteRenderer.sortingOrder = sorting;

        range.transform.localPosition = Vector3.zero;
        range.transform.localScale = Vector3.one * (1 + data.range * 2);
    }

    public void SelectTower(bool b)
    {
        range.SetActive(b);
    }
}
