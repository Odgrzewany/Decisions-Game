using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using System.IO;

public class XMLSerializerScript : MonoBehaviour
{
    public static XMLSerializerScript Ins;

    public Decks DecksToPlay;
    public Deck DeckChoosen;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (!Ins) Ins = this;
        else Destroy(this);
    }

    public Sprite LoadNewSprite(string filePath, float pixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Texture2D spriteTexture = LoadTexture(filePath);
        if (spriteTexture == null) return null;
        var newSprite = Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit);

        return newSprite;
    }

    public Texture2D LoadTexture(string filePath)
    {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        if (File.Exists(filePath))
        {
            var fileData = File.ReadAllBytes(filePath);
            var tex2D = new Texture2D(2, 2);
            if (tex2D.LoadImage(fileData))           // Load the imagedata into the texture (size is set automatically)
                return tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    public void SaveData(Deck deck)
    {
        FileStream stream = new FileStream(Application.persistentDataPath + "/XML Saves/" + deck.DeckName + ".xml", FileMode.Create);
        XmlSerializer serializer = new XmlSerializer(typeof(Deck));
        serializer.Serialize(stream, deck);
        stream.Close();
    }

    public void LoadData(bool decks)
    {
        string[] files = Directory.GetFiles(Application.persistentDataPath + "/XML Saves", "*.xml", SearchOption.AllDirectories);

        DecksToPlay.PlayerDecks = new List<Deck>();

        foreach (var t in files)
        {
            FileStream stream = new FileStream(t, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Deck));

            Deck serializedDeck = serializer.Deserialize(stream) as Deck;

            DecksToPlay.PlayerDecks.Add(serializedDeck);
            stream.Close();
        }
    }
}

[System.Serializable]
public class Decks
{
    public List<Deck> PlayerDecks = new List<Deck>();
}

[System.Serializable]
public class Deck
{
    public string DeckName;
    [Header("Event Cards Deck")]
    public List<EventCard> EventCards = new List<EventCard>();

//    [Header("Experimental Card Generation")]
//    public List<ProceduralCard> ProceduralCards = new List<ProceduralCard>();
//    public List<string> ProceduralCardImagesPaths = new List<string>();
  }

[System.Serializable]
public class ProceduralCard
{
    [TextArea] public string Description;
    public CardImpact CardFunction;
}

public enum CardImpact { Neutral, Positive, Negative }
