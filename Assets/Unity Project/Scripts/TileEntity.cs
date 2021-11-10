using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileEntity : MonoBehaviour
{
    public Vector3Int TilePosition;
    
    protected Grid m_Grid; // TODO: Do we even need a reference to the Grid?

    protected void Awake()
    {
        m_Grid = transform.parent.parent.GetComponent<Grid>();
    }
}
