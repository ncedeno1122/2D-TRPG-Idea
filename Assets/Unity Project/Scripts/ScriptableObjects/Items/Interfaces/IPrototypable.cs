using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPrototypable
{
    public IPrototypable Prototype { get; set; }
}