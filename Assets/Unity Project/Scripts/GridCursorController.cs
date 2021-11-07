using System;
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

        [SerializeField]
        private bool m_IsInKeyControlMode = false;

        public Grid Grid;
        private Camera m_MainCamera;
        private SpriteRenderer m_SpriteRenderer;

        private GridHelperScript m_GridHelper;
        public TileSelectionManager TileSelectionManager;

        private void Start()
        {
            m_MainCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Confined;

            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_GridHelper = Grid.GetComponent<GridHelperScript>();
            
            var transform1 = transform;
            transform1.position = Grid.GetCellCenterWorld(Vector3Int.zero);
            transform1.localScale = Grid.cellSize;
        }

        private void Update()
        {
            // Determine if we're in mouse or keyboard mode
            
            // Is there any mouse movement?
            if ((Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f) && m_IsInKeyControlMode)
            {
                m_IsInKeyControlMode = false;
            }
            else
            {
                // If not, check to see if mouse buttons are pressed.
                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && m_IsInKeyControlMode)
                {
                    m_IsInKeyControlMode = false;
                }
                else
                {
                    m_IsInKeyControlMode = true;
                }
            }
        }

        private void LateUpdate()
        {
            // If there's mouse movement,
            if (!m_IsInKeyControlMode)
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
                    TileSelectionManager.HandleInput(Vector3Int.FloorToInt(mousePositionWorld));
                }
            }
            else // If key controls are preferred,
            {
                // Get KeyCode for KeyInput
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    TileSelectionManager.HandleKeyCodeInput(KeyCode.LeftArrow);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    TileSelectionManager.HandleKeyCodeInput(KeyCode.RightArrow);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    TileSelectionManager.HandleKeyCodeInput(KeyCode.UpArrow);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    TileSelectionManager.HandleKeyCodeInput(KeyCode.DownArrow);
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    TileSelectionManager.HandleKeyCodeInput(KeyCode.Return);
                }
            }
        }

        // + + + + | Functions | + + + +

        public void Translate(Vector2Int translation)
        {
            transform.position += new Vector3(translation.x, translation.y, 0f);
        }
    }
}