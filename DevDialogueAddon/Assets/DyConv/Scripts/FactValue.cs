using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FactValueType
{
    Int,
    Float,
    Boolen,
}

[Serializable]
public class FactValue 
{
    public float value;
    public FactValueType type;
}
