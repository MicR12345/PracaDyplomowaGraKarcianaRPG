using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class Event
{
    public string name;
    public string description;
    public string type;
    public List<ChoiceOption> choices;
    public Sprite eventBackground;

    public Event(string _name, string _description, string _type, List<ChoiceOption> _choices, Sprite _eventBackground)
    {
        name = _name;
        description = _description;
        type = _type;
        choices = _choices;
        eventBackground = _eventBackground;
    }
}
public class ChoiceOption
{
    public string text;
    [XmlArray("addCards"),XmlArrayItem("card")]
    public List<string> addCards;
    [XmlArray("tags"), XmlArrayItem("tag")]
    public List<Tag> tags;

    public ChoiceOption(string _text, List<string> _addCards, List<Tag> _tags)
    {
        text = _text;
        addCards = _addCards;
        tags = _tags;
    }
    public ChoiceOption()
    {
        text = "";
        addCards = new List<string>();
        tags = new List<Tag>();
    }
    public Tag FindTag(string name)
    {
        foreach (Tag item in tags)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
}