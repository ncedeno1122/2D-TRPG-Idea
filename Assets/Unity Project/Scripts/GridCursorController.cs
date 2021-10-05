using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCursorController : MonoBehaviour
{
    private float m_CameraZoomMin = 1f;
    private float m_CameraZoomMax = 10f;
    private int m_ScreenWidth = Screen.width;
    private int m_ScreenHeight = Screen.height;
    private int m_CameraMoveDeadzone = 40;
    private float m_CameraMoveSpeed;

    public Grid Grid;
    private Camera m_mainCamera;
    private SpriteRenderer m_sr;

    private GridHelperScript m_gridHelper;

    private void Start()
    {
        transform.localScale = Grid.cellSize;
        m_mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Confined;

        m_sr = GetComponent<SpriteRenderer>();
        m_gridHelper = Grid.GetComponent<GridHelperScript>();
    }

    private void LateUpdate()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mousePositionWorld = m_mainCamera.ScreenToWorldPoint(mousePosition);
        mousePositionWorld.z = 0;
        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);

        // Move Cursor to Mouse Position
        transform.position = Grid.GetCellCenterWorld(Grid.WorldToCell(mousePositionWorld));

        // Move Camera if at edge of screen
        m_CameraMoveSpeed = (Input.GetKeyDown(KeyCode.LeftShift)) ? 5f : 10f;

        if (mousePosition.x < m_CameraMoveDeadzone)
        {
            m_mainCamera.transform.Translate(Vector3.left * m_CameraMoveSpeed * Time.deltaTime);
        }
        else if (mousePosition.x > m_ScreenWidth - m_CameraMoveDeadzone)
        {
            m_mainCamera.transform.Translate(Vector3.right * m_CameraMoveSpeed * Time.deltaTime);
        }

        if (mousePosition.y < m_CameraMoveDeadzone)
        {
            m_mainCamera.transform.Translate(Vector3.down * m_CameraMoveSpeed * Time.deltaTime);
        }
        else if (mousePosition.y > m_ScreenHeight - m_CameraMoveDeadzone)
        {
            m_mainCamera.transform.Translate(Vector3.up * m_CameraMoveSpeed * Time.deltaTime);
        }

        // Zoom Logic
        var mouseScrollDelta = Input.mouseScrollDelta;
        if (mouseScrollDelta != Vector2.zero)
        {
            m_mainCamera.orthographicSize = Mathf.Clamp(m_mainCamera.orthographicSize - mouseScrollDelta.y, m_CameraZoomMin, m_CameraZoomMax);
        }

        // If Mouse1 Pressed
        if (Input.GetMouseButtonDown(0))
        {
            //m_gridHelper.PaintInteractionRange(3, 1, Vector3Int.FloorToInt(mousePositionWorld));
            m_gridHelper.SelectTile(Vector3Int.FloorToInt(mousePositionWorld));
        }
    }

    // + + + + | Functions | + + + +
}