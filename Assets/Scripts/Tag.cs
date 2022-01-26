using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class Tag
{
    public string name;
    public float value;
    public Tag(string _name,float _value)
    {
        name = _name;
        value = _value;
    }
    public Tag()
    {
        name = "";
        value = 0;
    }
}
