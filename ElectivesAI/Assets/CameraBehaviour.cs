using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public enum CameraScreenPosition { TopLeft, TopRight, BottomLeft, BottomRight };
    public CameraScreenPosition PositionOfCamera;
    private Camera _camera;

    private void Start()
    {
        _camera = GetComponent<Camera>();

        switch (PositionOfCamera)
        {
            case CameraScreenPosition.TopLeft:
                _camera.rect = new Rect(0, 0, 0.5f, 0.5f);
                break;
            case CameraScreenPosition.TopRight:
                _camera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                break;
            case CameraScreenPosition.BottomLeft:
                _camera.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                break;
            case CameraScreenPosition.BottomRight:
                _camera.rect = new Rect(0.5f, 0.5f, 0.50f, 0.5f);
                break;
        }
    }
}
