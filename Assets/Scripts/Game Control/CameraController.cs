using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get { return instance; } }
    private static CameraController instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // ��� ���� ����
        boundary = new Rect(-16, -9, 32, 18);
    }

    public Camera Cam { get { return cam; } }
    private Camera cam;
    private PixelPerfectCamera pcam;
    
    private Rect boundary;

    public void FindCamera()
    {
        cam = Camera.main;
        if (cam == null) return;

        pcam = cam.GetComponent<PixelPerfectCamera>();

        float ratio = (float)1920 / Screen.width;

        pcam.assetsPPU = Mathf.CeilToInt(80 / ratio);
        pcam.refResolutionX = Screen.width;
        pcam.refResolutionY = Screen.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float scroll = Input.GetAxis("ScrollWheel");

        if (scroll > 0 && pcam.assetsPPU < 120) scroll = 2;
        else if (scroll < 0 && pcam.assetsPPU > 60) scroll = -2;
        else scroll = 0;

        pcam.assetsPPU += (int)scroll;

        // ��ȭ�� ���ٸ� ��ŵ
        if (horizontal == 0 && vertical == 0 && scroll == 0) return;
        Vector3 movement = new Vector3(horizontal, vertical) * 10 * Time.deltaTime;
        cam.transform.position += movement;

        /* ī�޶��� ũ��
         * ����: orthographicSize * 2
         * �ʺ�: aspect * height
         * y: height / 2 = orthographicSize
         * x: width / 2 = aspect * orthographicSize
         * �ش� ����� �������� Boundary���� ���������� ���ϵ���
         * �� ����Ʈ�� Rect ������ ������ �������� ��.
         */

        /* ��ü �̵��� �Ϸ��� �� cam�� Rect�� ��
         * ������ ������ Boundary�� �����ϱ� ������ �����ص� �������.
         * PPU�� 60�� �� orthographicSize: 9
         * ���������� ������ Boundary: -16,9,32,18
         * �׷����� Rect: �߽������κ��� ���ݸ�ŭ ���ϴ����� �̵�
         */

        float x = cam.aspect * cam.orthographicSize;
        float y = cam.orthographicSize;
        Rect rect = new Rect(-x + cam.transform.position.x, -y + cam.transform.position.y, x * 2, y * 2);

        /* ������ ����ٸ� ������ �̵����������.
         * Rect�� xmin, max, ymin, max�� ���� Ȯ���� �� ��� ��ŭ �̵�
         */

        if (rect.xMin < boundary.xMin) cam.transform.position -= new Vector3(rect.xMin - boundary.xMin, 0);
        else if (rect.xMax > boundary.xMax) cam.transform.position -= new Vector3(rect.xMax - boundary.xMax, 0);

        if (rect.yMin < boundary.yMin) cam.transform.position -= new Vector3(0, rect.yMin - boundary.yMin);
        else if (rect.yMax > boundary.yMax) cam.transform.position -= new Vector3(0, rect.yMax - boundary.yMax);
    }
}
