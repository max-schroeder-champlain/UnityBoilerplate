using UnityEngine;

public class CameraScript : MonoBehaviour
{
    GameObject gameManager;

    public Camera camObj;
    Vector3 cameraPos;

    [Header("Camera Settings")]
    public float speed;
    public float zoom;
    public float zoomChange;

    void Start()
    {
        gameManager = GameObject.Find("GameManager");
        cameraPos = transform.position;
    }

    void Update()
    {
        UpdateCameraPos();
        UpdateCameraZoom();
    }

    void UpdateCameraPos()
    {
        //Switches movement direction based on activePlayer
        if (transform.eulerAngles.z == 0)
        {
            if (Input.GetKey(KeyCode.W) && cameraPos.y < 24)
            {
                cameraPos.y += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S) && cameraPos.y > 0)
            {
                cameraPos.y -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A) && cameraPos.x > 0)
            {
                cameraPos.x -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) && cameraPos.x < 14)
            {
                cameraPos.x += speed * Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.S) && cameraPos.y < 24)
            {
                cameraPos.y += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.W) && cameraPos.y > 0)
            {
                cameraPos.y -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) && cameraPos.x > 0)
            {
                cameraPos.x -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A) && cameraPos.x < 14)
            {
                cameraPos.x += speed * Time.deltaTime;
            }
        }

            this.transform.position = cameraPos;
    }

    void UpdateCameraZoom()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            zoom -= zoomChange * Time.deltaTime * 10f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoom += zoomChange * Time.deltaTime * 10f;
        }

        zoom = Mathf.Clamp(zoom, 1.5f, 5f);

        camObj.orthographicSize = zoom;
    }
}