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

        // 경계 강제 설정
        // 1 = 24pixel
        // 전체사이즈 = 640 * 360
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
    //        // 변화가 없다면 스킵
    //        //if (horizontal == 0 && vertical == 0 && scroll == 0) return;
    //
    //        Vector3 movement = new Vector3(horizontal, vertical) * 10 * Time.deltaTime;
    //        cam.transform.position += movement;
    //
    //        /* 카메라의 크기
    //         * 높이: orthographicSize * 2
    //         * 너비: aspect * height
    //         * y: height / 2 = orthographicSize
    //         * x: width / 2 = aspect * orthographicSize
    //         * 해당 사이즈를 기준으로 Boundary에서 빠져나가지 못하도록
    //         * 각 포인트가 Rect 밖으로 나가면 빠져나간 것.
    //         */
    //
    //        /* 전체 이동을 완료한 후 cam의 Rect를 잼
    //         * 사이즈 조정은 Boundary가 존재하기 때문에 진행해도 상관없음.
    //         * PPU가 60일 때 orthographicSize: 9
    //         * 강제적으로 세팅한 Boundary: -16,9,32,18
    //         * 그려지는 Rect: 중심점으로부터 절반만큼 좌하단으로 이동
    //         */
    //
    //        float x = cam.aspect * cam.orthographicSize;
    //        float y = cam.orthographicSize;
    //        Rect rect = new Rect(-x + cam.transform.position.x, -y + cam.transform.position.y, x * 2, y * 2);
    //
    //        /* 밖으로 벗어났다면 안으로 이동시켜줘야함.
    //         * Rect의 xmin, max, ymin, max를 각각 확인한 뒤 벗어난 만큼 이동
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
            // 0.05초 간격으로 쉐이크
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
