using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class SaveCamTexture : MonoBehaviour
{
    int index = 0;
    int _index = 0;
    private string[] ss = {"3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "小", "大" };
    private int[] pos = { 0, -70, -140, -210 };

    public Camera cam;
    public RenderTexture rt;

    public Transform image;
    public Text value;

    public void Start()
    {
        if (cam == null)
        {
            cam = this.GetComponent<Camera>();
        }
    }
    private void Update()
    {
        if (cam == null)
        { return; }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            CaptureAndSaveScreenshot();
            //_SaveCamTexture();
        }
    }
    private void _SaveCamTexture()
    {
        rt = cam.targetTexture;
        if (rt != null)
        {
            _SaveRenderTexture(rt);
            rt = null;
        }
        else
        {
            GameObject camGo = new GameObject("camGO");
            Camera tmpCam = camGo.AddComponent<Camera>();
            tmpCam.CopyFrom(cam);
            rt = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            tmpCam.targetTexture = rt;
            tmpCam.Render();
            _SaveRenderTexture(rt);
            Destroy(camGo);
            rt.Release();
            Destroy(rt);
        }

    }
    private void _SaveRenderTexture(RenderTexture rt)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        png.Apply();
        RenderTexture.active = active;
        byte[] bytes = png.EncodeToPNG();
        string path = string.Format("Assets/rt_{0}_{1}_{2}.png", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        FileStream fs = File.Open(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(fs);
        writer.Write(bytes);
        writer.Flush();
        writer.Close();
        fs.Close();
        Destroy(png);
        png = null;
        Debug.Log("保存成功！" + path);
    }

    public void CaptureAndSaveScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine());
    }

    private IEnumerator CaptureScreenshotCoroutine()
    {
        Capture();

        yield return new WaitForEndOfFrame(); // 等待当前帧渲染结束

        // 截取整个屏幕
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        // 保存截图到本地文件
        byte[] bytes = screenshot.EncodeToPNG();
        string path = "Assets/" + index + "" + _index + ".png";
        System.IO.File.WriteAllBytes(path, bytes);

        // 释放截图资源
        Destroy(screenshot);

        Debug.Log("Screenshot saved to: " + path);
    }

    public void Capture()
    {
        if (index >= ss.Length)
        {
            _index++;
            index = 0;
        }
        string _value = ss[index];

        if (_index > pos.Length - 1) return;
        int _pos = pos[_index];

        Debug.Log(index + "" + _index);

        Vector2 currentPosition = image.GetComponent<RectTransform>().anchoredPosition;
        currentPosition.x = _pos;
        image.GetComponent<RectTransform>().anchoredPosition = currentPosition;

        value.text = _value;
        index++;
    }
}