using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;


[System.Serializable]
public class Effect
{
    public string name;
    public int level;
    public float value;
    public float duration;
    [XmlArray("tags"), XmlArrayItem("tag")]
    public List<Tag> tags;
    public Effect(string _name,int _level, float _value,float _duration,List<Tag> _tags)
    {   
        name = _name;
        level = _level;
        value = _value;
        duration = _duration;
        tags = _tags;
    }
    public Effect()
    {
        name = "";
        level = 0;
        value = 0;
        duration = 0;
        tags = new List<Tag>();
    }
}
