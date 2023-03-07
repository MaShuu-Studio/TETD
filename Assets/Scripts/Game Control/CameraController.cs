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

        // 경계 강제 설정
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

        // 변화가 없다면 스킵
        if (horizontal == 0 && vertical == 0 && scroll == 0) return;
        Vector3 movement = new Vector3(horizontal, vertical) * 10 * Time.deltaTime;
        cam.transform.position += movement;

        /* 카메라의 크기
         * 높이: orthographicSize * 2
         * 너비: aspect * height
         * y: height / 2 = orthographicSize
         * x: width / 2 = aspect * orthographicSize
         * 해당 사이즈를 기준으로 Boundary에서 빠져나가지 못하도록
         * 각 포인트가 Rect 밖으로 나가면 빠져나간 것.
         */

        /* 전체 이동을 완료한 후 cam의 Rect를 잼
         * 사이즈 조정은 Boundary가 존재하기 때문에 진행해도 상관없음.
         * PPU가 60일 때 orthographicSize: 9
         * 강제적으로 세팅한 Boundary: -16,9,32,18
         * 그려지는 Rect: 중심점으로부터 절반만큼 좌하단으로 이동
         */

        float x = cam.aspect * cam.orthographicSize;
        float y = cam.orthographicSize;
        Rect rect = new Rect(-x + cam.transform.position.x, -y + cam.transform.position.y, x * 2, y * 2);

        /* 밖으로 벗어났다면 안으로 이동시켜줘야함.
         * Rect의 xmin, max, ymin, max를 각각 확인한 뒤 벗어난 만큼 이동
         */

        if (rect.xMin < boundary.xMin) cam.transform.position -= new Vector3(rect.xMin - boundary.xMin, 0);
        else if (rect.xMax > boundary.xMax) cam.transform.position -= new Vector3(rect.xMax - boundary.xMax, 0);

        if (rect.yMin < boundary.yMin) cam.transform.position -= new Vector3(0, rect.yMin - boundary.yMin);
        else if (rect.yMax > boundary.yMax) cam.transform.position -= new Vector3(0, rect.yMax - boundary.yMax);
    }
}
