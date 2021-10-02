using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCursorController : MonoBehaviour
{
    public Grid Grid;
        private Camera m_mainCamera;
        private SpriteRenderer m_sr;
    
        // Start is called before the first frame update
        private void Start()
        {
            transform.localScale = Grid.cellSize;
            m_mainCamera = Camera.main;
            Cursor.lockState = CursorLockMode.Confined;
    
            m_sr = GetComponent<SpriteRenderer>();
        }
    
        private void LateUpdate()
        {
            Ray ray = m_mainCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 mousePositionWorld = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePositionWorld.z = 0;
    
            transform.position = Grid.GetCellCenterWorld(Grid.WorldToCell(mousePositionWorld));
    
            if (Input.GetMouseButtonDown(0))
            {
                //
            }
        }
    
        // + + + + | Functions | + + + +
}
