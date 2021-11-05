using Unity_Project.Scripts.TileSelectionLogic;
using UnityEngine;

namespace Unity_Project.Scripts
{
    public class GridCursorController : MonoBehaviour
    {
        private float m_CameraZoomMin = 1f;
        private float m_CameraZoomMax = 10f;
        private int m_ScreenWidth = Screen.width;
        private int m_ScreenHeight = Screen.height;
        private int m_CameraMoveDeadzone = 40;
        private float m_CameraMoveSpeed;

        public Grid Grid;
        private Camera m_MainCamera;
        private SpriteRenderer m_SpriteRenderer;

        private GridHelperScript m_GridHelper;
        public TileSelectionManager TileSelectionManager;

        private void Start()
        {
            transform.localScale = Grid.cellSize;
            m_MainCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Confined;

            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_GridHelper = Grid.GetComponent<GridHelperScript>();
        }

        private void LateUpdate()
        {
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mousePositionWorld = m_MainCamera.ScreenToWorldPoint(mousePosition);
            mousePositionWorld.z = 0;
            //Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);

            // Move Cursor to Mouse Position
            transform.position = Grid.GetCellCenterWorld(Grid.WorldToCell(mousePositionWorld));

            // Move Camera if at edge of screen
            m_CameraMoveSpeed = (Input.GetKeyDown(KeyCode.LeftShift)) ? 5f : 10f;

            if (mousePosition.x < m_CameraMoveDeadzone)
            {
                m_MainCamera.transform.Translate(Vector3.left * (m_CameraMoveSpeed * Time.deltaTime));
            }
            else if (mousePosition.x > m_ScreenWidth - m_CameraMoveDeadzone)
            {
                m_MainCamera.transform.Translate(Vector3.right * (m_CameraMoveSpeed * Time.deltaTime));
            }

            if (mousePosition.y < m_CameraMoveDeadzone)
            {
                m_MainCamera.transform.Translate(Vector3.down * (m_CameraMoveSpeed * Time.deltaTime));
            }
            else if (mousePosition.y > m_ScreenHeight - m_CameraMoveDeadzone)
            {
                m_MainCamera.transform.Translate(Vector3.up * (m_CameraMoveSpeed * Time.deltaTime));
            }

            // Zoom Logic
            var mouseScrollDelta = Input.mouseScrollDelta;
            if (mouseScrollDelta != Vector2.zero)
            {
                m_MainCamera.orthographicSize = Mathf.Clamp(m_MainCamera.orthographicSize - mouseScrollDelta.y, m_CameraZoomMin, m_CameraZoomMax);
            }

            // If Mouse1 Pressed
            if (Input.GetMouseButtonDown(0))
            {
                //m_GridHelper.SelectTile(Vector3Int.FloorToInt(mousePositionWorld));
                TileSelectionManager.HandleInput(Vector3Int.FloorToInt(mousePositionWorld));
            }
        }

        // + + + + | Functions | + + + +
    }
}