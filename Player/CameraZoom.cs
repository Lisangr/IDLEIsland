using UnityEngine;
using YG;

public class CameraZoom : MonoBehaviour
{
    public Camera camera;
    public float zoomSpeed = 10f;
    public float minFOV = 15f;
    public float maxFOV = 90f;
    private void OnEnable()
    {
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        MapForComputers();

        if (YandexGame.EnvironmentData.isMobile || YandexGame.EnvironmentData.isTablet)
        {
            MapForComputers();
            MapForMobile();
        }
        else
        {
            MapForComputers();
        }
    }

    private void MapForComputers()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            ZoomCamera(scrollInput);
        }
    }

    private void ZoomCamera(float increment)
    {
        // Используем поле зрения стандартной камеры
        float currentFOV = camera.fieldOfView;
        float newFOV = currentFOV - increment * zoomSpeed;
        camera.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
    }

    private void MapForMobile()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            ZoomCamera(difference * zoomSpeed * 0.1f);
        }
    }
}
