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

    // �ӽ� ����
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

        /* Ÿ���� grid.cellSize��ŭ �������� ����
         * ���� worldPos���� ������ ���� �簢���� ���� �Ʒ� �������� ����.
         * ���� cellSize�� ���ݸ�ŭ ��ġ�� �������ָ� Ÿ���� ��ġ�� ��.
         */
        float x = (int)Mathf.Floor(worldPos.x) + grid.cellSize.x / 2;
        float y = (int)Mathf.Floor(worldPos.y) + grid.cellSize.y / 2;
        float z = transform.position.z;

        // ���õ� Ÿ���� ��ġ�� localPosition���� �������� ������ grid���� ��ġ�� ��߳�.
        selectedTile.transform.localPosition = new Vector3(x, y, z);
    }
}
