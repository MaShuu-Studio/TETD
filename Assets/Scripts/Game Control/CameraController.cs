using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get { return instance; } }
    private static CameraController instance;
    public static int ReferencePPU { get; private set; } = 72;
    private Vector2Int baseResolution = new Vector2Int(1920, 1080);
    public Vector2Int RefResolution { get { return refResolution; } }
    private Vector2Int refResolution;

    private int curPPU;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // ��� ���� ����
        // 1 = 24pixel
        // ��ü������ = 640 * 360
        // 26.67 * 15
        float x = 640 / 24f;
        float y = 360 / 24f;
        boundary = new Rect(-x / 2, -y / 2, x, y);

        pcam = cam.GetComponent<PixelPerfectCamera>();

        pcam.assetsPPU = ReferencePPU;
        pcam.refResolutionX = baseResolution.x;
        pcam.refResolutionY = baseResolution.y;
    }

    public Camera Cam { get { return cam; } }
    [SerializeField] private Camera cam;
    private PixelPerfectCamera pcam;

    public void ChangeResolution(int width, int height)
    {
        refResolution = new Vector2Int(width, height);

        pcam.assetsPPU = (int)(ReferencePPU * ((float)width / baseResolution.x));
        pcam.refResolutionX = refResolution.x;
        pcam.refResolutionY = refResolution.y;

        curPPU = pcam.assetsPPU;
    }

    public void ResetCameraSetting()
    {
        pcam.assetsPPU = curPPU;
        pcam.refResolutionX = refResolution.x;
        pcam.refResolutionY = refResolution.y;
    }

    private Rect boundary;
    /*
    public void FindCamera()
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            //cam.transform.localScale = Vector3.one;
            shakeCoroutine = null;
        }

        cam = Camera.main;
        if (cam == null) return;

        pcam = cam.GetComponent<PixelPerfectCamera>();

        float ratio = (float)1920 / Screen.width;

        pcam.assetsPPU = 72;// Mathf.CeilToInt(80 / ratio);
        pcam.refResolutionX = Screen.width;
        pcam.refResolutionY = Screen.height;
    }
    */
    float scroll;

    // Update is called once per frame
    //    void Update()
    //    {
    //        if (cam == null) return;
    //
    //        scroll = Input.GetAxis("ScrollWheel");
    //
    //        if (scroll > 0 && pcam.assetsPPU < 120) scroll = 8;
    //        else if (scroll < 0 && pcam.assetsPPU > 72) scroll = -8;
    //        else scroll = 0;
    //
    //        pcam.assetsPPU += (int)scroll;
    //
    //        float horizontal = Input.GetAxis("Horizontal");
    //        float vertical = Input.GetAxis("Vertical");
    //        // ��ȭ�� ���ٸ� ��ŵ
    //        //if (horizontal == 0 && vertical == 0 && scroll == 0) return;
    //
    //        Vector3 movement = new Vector3(horizontal, vertical) * 10 * Time.deltaTime;
    //        cam.transform.position += movement;
    //
    //        /* ī�޶��� ũ��
    //         * ����: orthographicSize * 2
    //         * �ʺ�: aspect * height
    //         * y: height / 2 = orthographicSize
    //         * x: width / 2 = aspect * orthographicSize
    //         * �ش� ����� �������� Boundary���� ���������� ���ϵ���
    //         * �� ����Ʈ�� Rect ������ ������ �������� ��.
    //         */
    //
    //        /* ��ü �̵��� �Ϸ��� �� cam�� Rect�� ��
    //         * ������ ������ Boundary�� �����ϱ� ������ �����ص� �������.
    //         * PPU�� 60�� �� orthographicSize: 9
    //         * ���������� ������ Boundary: -16,9,32,18
    //         * �׷����� Rect: �߽������κ��� ���ݸ�ŭ ���ϴ����� �̵�
    //         */
    //
    //        float x = cam.aspect * cam.orthographicSize;
    //        float y = cam.orthographicSize;
    //        Rect rect = new Rect(-x + cam.transform.position.x, -y + cam.transform.position.y, x * 2, y * 2);
    //
    //        /* ������ ����ٸ� ������ �̵����������.
    //         * Rect�� xmin, max, ymin, max�� ���� Ȯ���� �� ��� ��ŭ �̵�
    //         */
    //
    //        if (rect.xMin < boundary.xMin) cam.transform.position -= new Vector3(rect.xMin - boundary.xMin, 0);
    //        else if (rect.xMax > boundary.xMax) cam.transform.position -= new Vector3(rect.xMax - boundary.xMax, 0);
    //
    //        if (rect.yMin < boundary.yMin) cam.transform.position -= new Vector3(0, rect.yMin - boundary.yMin);
    //        else if (rect.yMax > boundary.yMax) cam.transform.position -= new Vector3(0, rect.yMax - boundary.yMax);
    //    }

    //private Vector3 shakePos;
    private float shaking;
    private IEnumerator shakeCoroutine;

    public void ShakeCamera(float time)
    {
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
            //cam.transform.localScale = Vector3.one;
            shakeCoroutine = null;
        }
        shakeCoroutine = Shake(time);
        StartCoroutine(shakeCoroutine);
    }

    public IEnumerator Shake(float max)
    {
        float time = 0;
        //shakePos = Vector3.zero;
        shaking = .98f;
        while (time < max)
        {
            /*
            int compare = (int)(time * 20);
            // 0.05�� �������� ����ũ
            if (compare == count)
            {
                cam.transform.position -= shakePos;
                float x = Random.Range(-0.064f, 0.064f);
                float y = Random.Range(-0.036f, 0.036f);
                shakePos = new Vector3(x, 0);
                cam.transform.position += shakePos;
                count++;
            }*/

            shaking = .98f + (0.02f * (time / max));
            cam.transform.localScale = Vector3.one * shaking;
            time += Time.deltaTime;
            yield return null;
        }
        cam.transform.localScale = Vector3.one;
    }
}
