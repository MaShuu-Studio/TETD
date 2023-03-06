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
    }

    public Camera Cam { get { return cam; } }
    private Camera cam;
    private PixelPerfectCamera pcam;

    public void FindCamera()
    {
        cam = FindObjectOfType<Camera>();
        Debug.Log(cam);
        if (cam == null) return;

        pcam = cam.GetComponent<PixelPerfectCamera>();

        float ratio = (float)1920 / Screen.width;

        pcam.assetsPPU = Mathf.CeilToInt(81 / ratio);
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

        cam.transform.Translate(new Vector3(horizontal, vertical) * 10 * Time.deltaTime);

        if (scroll > 0 && pcam.assetsPPU < 120) scroll = 2;
        else if (scroll < 0 && pcam.assetsPPU > 40) scroll = -2;
        else scroll = 0;

        pcam.assetsPPU += (int)scroll;
    }
}
