using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraMgr : SerializedMonoBehaviour
{
    [SerializeField] private float maxSize;
    [SerializeField] private float minSize;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float moveSpeed;

    private const float TOP     = 35;
    private const float BOTTOM  = -55;
    private const float RIGHT   = 65;
    private const float LEFT    = -65;

    private Camera camera;
    private bool InitFlag = false;
    public static CameraMgr Instance { get; private set; }
    private void Init()
    {
        if (InitFlag)
            return;
        InitFlag = true;

        camera ??= Camera.main;

        Instance = this;
    }

    private void MouseScroll()
    {
        Init();

        if (GameMgr.Instance.loadEndFlag == false)
            return;

        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (scroll == 0)
            return;

        float newSize = camera.orthographicSize + scroll;
        newSize = Mathf.Clamp(newSize, minSize, maxSize);

        camera.orthographicSize = newSize;
    }

    private void CameraMove()
    {
        Init();

        if (GameMgr.Instance.loadEndFlag == false)
            return;

        float speed = moveSpeed / camera.orthographicSize;
        speed = Mathf.Max(speed, 0.2f);

        float moveHorizontal = Input.GetAxis("Horizontal") * speed;
        float moveVertical = Input.GetAxis("Vertical") * speed;
        camera.transform.position += new Vector3(moveHorizontal, moveVertical, 0);

        float yScreenHalfSize = camera.orthographicSize;
        float xScreenHalfSize = yScreenHalfSize * camera.aspect;

        if(camera.transform.position.y - yScreenHalfSize < BOTTOM)
            camera.transform.position = new Vector3(camera.transform.position.x, BOTTOM + yScreenHalfSize, camera.transform.position.z);
        if (camera.transform.position.y + yScreenHalfSize > TOP)
            camera.transform.position = new Vector3(camera.transform.position.x, TOP - yScreenHalfSize, camera.transform.position.z);
        if (camera.transform.position.x + xScreenHalfSize > RIGHT)
            camera.transform.position = new Vector3(RIGHT - xScreenHalfSize, camera.transform.position.y, camera.transform.position.z);
        if (camera.transform.position.x - xScreenHalfSize < LEFT)
            camera.transform.position = new Vector3(LEFT + xScreenHalfSize, camera.transform.position.y, camera.transform.position.z);
    }

    public void SetPos(Vector2 pos)
    {
        Init();
        camera.transform.position = new Vector3(pos.x, pos.y, camera.transform.position.z);
    }
    private void Awake()
    {
        Init();
    }
    private void Update()
    {
        MouseScroll();
        CameraMove();
    }
}
