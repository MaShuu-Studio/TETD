using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerObject : MonoBehaviour
{
    private SpriteRenderer renderer;
    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    public void Arrange(Vector3 pos)
    {
        int sorting = Mathf.FloorToInt(pos.y) * -1;
        renderer.sortingOrder = sorting;
    }
}
