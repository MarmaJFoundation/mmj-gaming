using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public enum CameraState
{
    Free = 0,
    Focused = 1,
    Dungeon = 2
}
public class CameraController : MonoBehaviour
{
    public Camera thisCam;
    public Camera pixelCam;
    public PixelPerfectCamera pixelPerfectCamera;
    public Vector3 inventoryOffset;
    public float camSpeed;
    private Coroutine zoomCoroutine;
    private Coroutine moveCoroutine;
    [HideInInspector]
    public float lastScreenWidth;
    public float OrthoDiff
    {
        get
        {
            return pixelCam.orthographicSize / thisCam.orthographicSize;
        }
    }
    private void Start()
    {
        lastScreenWidth = Screen.width;
        thisCam.orthographicSize = 6f;
    }
    private void LateUpdate()
    {
        if (Screen.width != lastScreenWidth)
        {
            lastScreenWidth = Screen.width;
            MoveCamera(thisCam.transform.position);
        }
    }
    public void MoveCamera(Vector3 goPos)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveCameraCoroutine(goPos));
    }
    private IEnumerator MoveCameraCoroutine(Vector3 goPos)
    {
        pixelPerfectCamera.pixelSnapping = false;
        Vector3 fromPos = thisCam.transform.position;
        float timer = 0;
        while (timer <= 1)
        {
            thisCam.transform.position = Vector3.Lerp(fromPos, goPos, timer.Evaluate(CurveType.CameraEaseOut));
            pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
            timer += Time.deltaTime * camSpeed;
            yield return null;
        }
        thisCam.transform.position = goPos;
        pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
        pixelPerfectCamera.pixelSnapping = true;
        moveCoroutine = null;
    }
    public void ZoomCamera(float goOrthoSize)
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
        }
        zoomCoroutine = StartCoroutine(ZoomCameraCoroutine(goOrthoSize));
    }
    private IEnumerator ZoomCameraCoroutine(float goOrthoSize)
    {
        pixelPerfectCamera.pixelSnapping = false;
        float fromOrthoSize = thisCam.orthographicSize;
        float timer = 0;
        while (timer <= 1)
        {
            thisCam.orthographicSize = Mathf.Lerp(fromOrthoSize, goOrthoSize, timer.Evaluate(CurveType.CameraEaseOut));
            timer += Time.deltaTime * camSpeed;
            yield return null;
        }
        thisCam.orthographicSize = goOrthoSize;
        pixelPerfectCamera.pixelSnapping = true;
    }
    public void ChestZoom()
    {
        StartCoroutine(ChestZoomCoroutine());
    }
    private IEnumerator ChestZoomCoroutine()
    {
        Vector3 fromPos = thisCam.transform.position;
        float fromSize = thisCam.orthographicSize;
        float timer = 0;
        while (timer <= 1)
        {
            thisCam.transform.position = Vector3.Lerp(fromPos, fromPos + Vector3.right * 2, timer.Evaluate(CurveType.PeakParabol));
            thisCam.orthographicSize = Mathf.Lerp(fromSize, fromSize - 1f, timer.Evaluate(CurveType.PeakParabol));
            pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
            timer += Time.deltaTime * .75f;
            yield return null;
        }
        thisCam.orthographicSize = fromSize;
        thisCam.transform.position = fromPos;
        pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
    }
    public void BossZoom()
    {
        StartCoroutine(BossZoomCoroutine());
    }
    private IEnumerator BossZoomCoroutine()
    {
        Vector3 fromPos = thisCam.transform.position;
        float timer = 0;
        float fromOrtho = thisCam.orthographicSize;
        while (timer <= 1)
        {
            float shakeOffset = Mathf.LerpUnclamped(0, 3, timer.Evaluate(CurveType.BossShakeCurve));
            thisCam.orthographicSize = Mathf.Lerp(fromOrtho, fromOrtho - 3f, timer.Evaluate(CurveType.AttackCurve));
            thisCam.transform.position = Vector3.Lerp(fromPos, fromPos + Vector3.right * (shakeOffset), timer.Evaluate(CurveType.AttackCurve));
            pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
            timer += Time.deltaTime;
            yield return null;
        }
        thisCam.orthographicSize = fromOrtho;
        thisCam.transform.position = fromPos;
        pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
    }
    public void CritZoom(Transform targetTransform, float delay)
    {
        StartCoroutine(CritZoomCoroutine(targetTransform, delay));
    }
    private IEnumerator CritZoomCoroutine(Transform targetTransform, float delay)
    {
        Vector3 fromPos = thisCam.transform.position;
        float timer = 0;
        float offset = targetTransform.position.x > 0 ? 1 : -1;
        while (timer <= .5f + delay)
        {
            float shakeOffset = Mathf.LerpUnclamped(0, 3, timer.Evaluate(CurveType.ShakeCurve2));
            if (timer < delay)
            {
                shakeOffset = 0;
            }
            thisCam.transform.position = Vector3.Lerp(fromPos, fromPos + Vector3.right * (offset + shakeOffset), timer.Evaluate(CurveType.AttackCurve));
            pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
            timer += Time.deltaTime * 2;
            yield return null;
        }
        thisCam.transform.position = fromPos;
        pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
    }
    public void ShakeCamera(float potency)
    {
        if (moveCoroutine != null)
        {
            return;
        }
        moveCoroutine = StartCoroutine(CameraShakeCoroutine(potency));
    }
    public IEnumerator CameraShakeCoroutine(float potency)
    {
        float timer = 0;
        float shakeOffset;
        Vector3 fromPos = thisCam.transform.position;
        while (timer <= 1)
        {
            shakeOffset = Mathf.LerpUnclamped(0, potency, timer.Evaluate(CurveType.ShakeCurve));
            thisCam.transform.position = fromPos + Vector3.right * shakeOffset;
            pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
            timer += Time.deltaTime * 8;
            yield return null;
        }
        thisCam.transform.position = fromPos;
        pixelCam.transform.position = thisCam.transform.position * OrthoDiff;
        moveCoroutine = null;
    }
}
