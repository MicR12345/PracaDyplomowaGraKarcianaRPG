using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class EventLibrary : MonoBehaviour
{
    public List<Event> eventList = new List<Event>();
    public List<ChoiceOption> choiceList = new List<ChoiceOption>();
    public void LoadAllEvents()
    {
        eventList = new List<Event>();
        EventLoader eventLoader = new EventLoader("CoreGame/XmlFiles/events");
        eventList = new List<Event>(eventLoader.events);
    }
    [XmlRoot(ElementName = "EventList")]
    public class Events
    {
        [XmlArray("events"), XmlArrayItem("event")]
        public List<EventData> events = new List<EventData>();
        public Events()
        {
            events = new List<EventData>();
        }
    }
    public class EventData
    {
        public string name;
        public string description;
        public string type;
        [XmlArray("choices"), XmlArrayItem("choice")]
        public List<ChoiceOption> choices;
        public EventData()
        {
            name = "";
            description = "";
            type = "";
            choices = new List<ChoiceOption>();
        }
    }
    class EventLoader
    {
        public List<Event> events;
        List<EventData> loadedEvents;
        
        public EventLoader(string pathToEventList)
        {
            loadedEvents = new List<EventData>();
            events = new List<Event>();
            XmlSerializer reader = new XmlSerializer(typeof(Events));
            TextAsset eventXml = Resources.Load(pathToEventList, typeof(TextAsset)) as TextAsset;
            TextReader eventReader = new StringReader(eventXml.text);
            Events loaded = (Events)reader.Deserialize(eventReader);
            eventReader.Close();

            foreach(EventData i in loaded.events)
            {
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D = Resources.Load("CoreGame/EventGFX/" + i.name, typeof(Texture2D)) as Texture2D;
                Sprite eventSprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 15f);
                Event wydarzenie = new Event(i.name, i.description, i.type, i.choices, eventSprite);
                events.Add(wydarzenie);
            }
        }

    }
}
