using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMode
{
    Menu, Game, HowTo
}

public class CameraController : MonoBehaviour
{
    public Transform ObjectToFollow;
    public float maxHeightOff = 5;

    Vector3 initialGameCameraPos;
    Vector3 offset;
    Vector3 lastMousePos;

    [Header("Cameras")]
    public Camera MenuCamera;
    public Camera GameCamera;
    public Camera HowToCamera;

    [Header("Animation")]
    public float transitionTime;
    public AnimationCurve AnimationCurve;

    Camera activeCamera;

    Vector3 startPosition;
    Quaternion startRotation;

    Vector3 targetPosition;
    Quaternion targetRotation;

    bool animating = false;
    public bool Animationg { get { return animating; } }

    CameraMode currentMode;
    public CameraMode CurrentMode
    {
        get { return currentMode; }
        set
        {
            TransitionTo(value);
        }
    }

    public void GotoHowTo()
    {
        CurrentMode = CameraMode.HowTo;
    }

    public void GotoMenu()
    {
        CurrentMode = CameraMode.Menu;
    }

    private void Start()
    {
        initialGameCameraPos = GameCamera.transform.position;
        GameManager.Instance.CameraManager = this;

        CurrentMode = CameraMode.Menu;
        SetCamerasEnabled();

        offset = transform.position - ObjectToFollow.position;
        lastMousePos = Input.mousePosition;
    }

    void TransitionTo(CameraMode to)
    {
        if (currentMode == to)
            return;

        if(to == CameraMode.Game)
        {
            GameCamera.transform.position = initialGameCameraPos;
            offset = GameCamera.transform.position - ObjectToFollow.position;
            currentMode = to;
            SetCamerasEnabled();
            return;
        }

        Debug.Log("CameraManager: Transition to " + to);

        var currentCam = GetCameraForMode(currentMode);
        var nextCam = GetCameraForMode(to);

        startPosition = currentCam.transform.position;
        startRotation = currentCam.transform.rotation;

        targetPosition = nextCam.transform.position;
        targetRotation = nextCam.transform.rotation;

        currentMode = to;
        activeCamera = nextCam;

        StartCoroutine(AnimateTransition());
    }

    IEnumerator AnimateTransition()
    {
        animating = true;

        activeCamera.transform.position = startPosition;
        activeCamera.transform.rotation = startRotation;

        SetCamerasEnabled();

        float timer = 0;
        while(timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;
            float at = AnimationCurve.Evaluate(t);

            activeCamera.transform.position = Vector3.Slerp(startPosition, targetPosition, at);
            activeCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, at);

            yield return new WaitForEndOfFrame();
        }

        activeCamera.transform.position = targetPosition;
        activeCamera.transform.rotation = targetRotation;

        animating = false;

        
    }

    Camera GetCameraForMode(CameraMode mode)
    {
        switch (mode)
        {
            case CameraMode.Menu: return MenuCamera;
            case CameraMode.Game: return GameCamera;
            case CameraMode.HowTo: return HowToCamera;
        }
        return null;
    }

    void SetCamerasEnabled()
    {
        MenuCamera.gameObject.SetActive(false);
        GameCamera.gameObject.SetActive(false);
        HowToCamera.gameObject.SetActive(false);
        GetCameraForMode(currentMode).gameObject.SetActive(true);
    }

    private void Update()
    {
        if(CurrentMode == CameraMode.Game)
        {
            if (ObjectToFollow != null)
            {
                GameCamera.transform.position = ObjectToFollow.position + offset;
                GameCamera.transform.LookAt(ObjectToFollow);
            }

            Vector3 currentMousePos = Input.mousePosition;

            if (GameManager.Instance.GameRunning == true && Input.GetMouseButton(1))
            {
                Vector3 delta = lastMousePos - currentMousePos;
                delta *= 8;
                GameCamera.transform.RotateAround(ObjectToFollow.position, Vector3.up, -delta.x * Time.deltaTime);
                GameCamera.transform.RotateAround(ObjectToFollow.position, Vector3.right, -delta.y * Time.deltaTime);

                var camPos = GameCamera.transform.position;
                camPos.y = Mathf.Clamp(camPos.y, ObjectToFollow.transform.position.y - maxHeightOff, ObjectToFollow.transform.position.y + maxHeightOff);
                GameCamera.transform.position = camPos;
            }
           
            offset = GameCamera.transform.position - ObjectToFollow.position;

            

            lastMousePos = currentMousePos;
        }
    }

}
