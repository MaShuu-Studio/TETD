using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class TileController : MonoBehaviour
{
    public static TileController Instance { get { return instance; } }
    private static TileController instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private GameObject selectedTile;
    private Grid grid;

    // 임시 변수
    private Camera cam;
    private void Start()
    {
        grid = GetComponent<Grid>();
        cam = FindObjectOfType<Camera>();
    }

    void Update()
    {
        SelectTile();
    }

    public void SelectTile()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        /* 타일은 grid.cellSize만큼 나누어져 있음
         * 따라서 worldPos에서 내림을 통해 사각형의 왼쪽 아래 꼭지점을 얻음.
         * 이후 cellSize의 절반만큼 위치를 조정해주면 타일의 위치가 됨.
         */
        float x = (int)Mathf.Floor(worldPos.x) + grid.cellSize.x / 2;
        float y = (int)Mathf.Floor(worldPos.y) + grid.cellSize.y / 2;
        float z = transform.position.z;

        // 선택된 타일의 위치를 localPosition으로 조정하지 않으면 grid에서 위치가 어긋남.
        selectedTile.transform.localPosition = new Vector3(x, y, z);
    }
}
