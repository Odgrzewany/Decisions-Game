using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private int _openW;
    public GameObject Parent;
    public GameObject DeckAvaible;
    public GameObject Window;

    private void Play(Deck deck)
    {
        XMLSerializerScript.Ins.DeckChoosen = deck;
        SceneManager.LoadScene(2);
    }

    void Start()
    {
        if(PlayerPrefs.GetInt("WasCreated") == 1) return;

        PlayerPrefs.SetInt("WasCreated", 1);
        Directory.CreateDirectory(Application.persistentDataPath + "/XML Saves");
        Directory.CreateDirectory(Application.persistentDataPath + "/Cards Images");
    }

    public void OpenWidnow()
    {
        if (_openW == 0)
        {
            Window.SetActive(true);
            LoadDeckTabel();
            _openW++;
        }
        else
        {
            Window.SetActive(false);
            _openW = 0;
        }
    }

    private void LoadDeckTabel()
    {
        XMLSerializerScript.Ins.LoadData(true);

        foreach (Transform child in Parent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var deck in XMLSerializerScript.Ins.DecksToPlay.PlayerDecks)
        {
            GameObject go = Instantiate(DeckAvaible, Parent.transform);
            go.GetComponentInChildren<TextMeshProUGUI>().text = deck.DeckName;
            go.GetComponentInChildren<Button>().onClick.AddListener(delegate { Play(deck); });
        }
    }

    public void DeckEditor()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
