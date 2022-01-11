using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event
{
    public string name;
    public string description;
    public string type;
    public List<ChoiceOption> choices;
}
public class ChoiceOption
{
    public string text;
    public List<string> addCards;
    public List<Tag> tags;
}