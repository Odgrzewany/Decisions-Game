using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

public class DeckEditor : MonoBehaviour
{
    private Deck _createdDeck;
    public EventCard _currentCard;

    private int _openD;
    private int _openV;
    private int _openC;
    private int _currentCardIndex;
    private bool _wasSaved;
    private int _openS;

    public Sprite DefaultSprite;

    [Header("Values Window ('s)")]
    public GameObject ValuesWindow;
    public GameObject ValuesWindowLeft;
    public GameObject ValuesWindowRight;
    public GameObject ValuesWindowDelayed;
    public GameObject ValuesWindowDelayedBlockedCards;

    [Header("Menu")]
    public GameObject Menu;
    public GameObject LoadMenu;
    public GameObject DeckAvaible;
    public GameObject Parent;
    public GameObject SaveWindow;

    [Header("Delayed Text Window")]
    public GameObject DelayedTextWindow;

    [Header("Card Image")]
    public Image CardImage;

    [Header("Inputs")]
    [Header("Objects to UPDATE")]
    public TMP_InputField DeckName;
    public TMP_InputField CardName;
    public TMP_InputField CardImagePath;
    public TMP_InputField CardText;
    public TMP_InputField CardLeftText;
    public TMP_InputField CardRightText;
    public TMP_InputField CardDelayText;
    public TMP_InputField CardDelayedCardNames;
    public TMP_InputField CardBlockedCardsNames;
    [Header("Toggle")]
    public Toggle DelayedOnly;
    [Header("Dropdowns")]
    public TMP_Dropdown CardTypeDropDown;
    public TMP_Dropdown CardDelayedTypeDropDown;
    [Header("Sliders")]
    public Slider Slider1;
    public Slider Slider2;
    public Slider Slider3;
    public Slider Slider4;
    public Slider Slider5;
    public Slider Slider6;
    public Slider Slider7;
    public Slider Slider8;
    public Slider Slider9;
    public Slider Slider10;


    public void Exit()
    {
        if (_wasSaved)
            Menu.SetActive(true);
        else
        {
            if (_openS == 0)
            {
                SaveWindow.SetActive(true);
                _openS++;
            }
            else
            {
                SaveWindow.SetActive(false);
                _openS = 0;
            }
        }
    }

    public void DeleteDeck(Deck deck, GameObject obj)
    {
        if (File.Exists(Application.persistentDataPath + "/XML Saves/" + deck.DeckName + ".xml"))
        {
            File.Delete(Application.persistentDataPath + "/XML Saves/" + deck.DeckName + ".xml");
            File.Delete(Application.persistentDataPath + "/XML Saves/" + deck.DeckName + ".xml.meta");
            Destroy(obj);
        }
    }

    public void DeleteCard()
    {
        if (_createdDeck.EventCards.Count == 1)
        {
            _createdDeck.EventCards.Remove(_currentCard);
            CreateCard();
        }
        else
        {
            _createdDeck.EventCards.Remove(_currentCard);
            if (_currentCardIndex == 0)
                _currentCardIndex--;
            else
                _currentCardIndex = _createdDeck.EventCards.Count - 1;

            UpdateCardInfo();
        }
    }

    public void CLoseMenu()
    {
        LoadMenu.SetActive(false);
        _openC = 0;
    }

    public void AcceptExit()
    {
        Menu.SetActive(true);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    public void CreateNewDeck()
    {
        _createdDeck = new Deck {EventCards = new List<EventCard>()};
        _currentCardIndex = 0;
        CreateCard();
        Menu.SetActive(false);
        _wasSaved = false;
    }

    public void SaveDeck()
    {
        XMLSerializerScript.Ins.DecksToPlay.PlayerDecks[
            XMLSerializerScript.Ins.DecksToPlay.PlayerDecks.IndexOf(_createdDeck)] = _createdDeck;
        XMLSerializerScript.Ins.SaveData(_createdDeck);
    }

    public void SaveNewDeck()
    {
        if (XMLSerializerScript.Ins.DecksToPlay.PlayerDecks.Contains(_createdDeck))
        {
            SaveDeck();
            _wasSaved = true;
            return;
        }

        XMLSerializerScript.Ins.DecksToPlay.PlayerDecks.Add(_createdDeck);
        XMLSerializerScript.Ins.SaveData(_createdDeck);
        _wasSaved = true;
    }

    public void OpenLoadDeckMenu(bool delete)
    {
        if (_openC == 0)
        {
            LoadMenu.SetActive(true);
            _openC++;
            LoadDeckTabel(delete);
        }
        else
        {
            _openC = 0;
            LoadMenu.SetActive(false);
        }
    }

    private void LoadDeckTabel(bool delete)
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
            if(!delete)
            go.GetComponentInChildren<Button>().onClick.AddListener(delegate { LoadDeck(deck);});
            else
            {
                go.GetComponentInChildren<Button>().onClick.AddListener(delegate {DeleteDeck(deck,go); });
            }
        }
    }

    public void LoadDeck(Deck deckToLoad)
    {
        Menu.SetActive(false);
        _createdDeck = deckToLoad;
        _currentCardIndex = 0;
        UpdateCardInfo();
        DeckName.text = _createdDeck.DeckName;
        _wasSaved = true;
    }

    public void CreateCard()
    {
        _currentCard = new EventCard();
        _createdDeck.EventCards.Add(_currentCard);
        _currentCardIndex = _createdDeck.EventCards.Count - 1;
        _currentCard.CardsToBlockNames = new string[0];
        UpdateCardInfo();
        _wasSaved = false;
    }

    public void UpdateDeckName(TMP_InputField input)
    {
        _createdDeck.DeckName = input.text;
        _wasSaved = false;
    }
    public void UpdateCardName(TMP_InputField input)
    {
        _currentCard.Name = input.text;
        _wasSaved = false;
    }
    public void UpdateImagePath(TMP_InputField input)
    {
        _currentCard.CardImagePath = Application.persistentDataPath + "/Cards Images/" + input.text;
        _currentCard.WhileCardImagePath = input.text;
        Sprite sprite = XMLSerializerScript.Ins.LoadNewSprite(_currentCard.CardImagePath);

        if (sprite == null)
        {
            input.text = $"Incorrect path, make sure that \nyou have correct file extension.";
            CardImage.sprite = DefaultSprite;
            return;
        }

        CardImage.sprite = sprite;
        _wasSaved = false;
    }
    public void UpdateCardText(TMP_InputField input)
    {
        _currentCard.TextToSay = input.text;
        _wasSaved = false;
    }
    public void UpdateLeftCardText(TMP_InputField input)
    {
        _currentCard.LeftText = input.text;
        _wasSaved = false;
    }
    public void UpdateRightCardText(TMP_InputField input)
    {
        _currentCard.RightText = input.text;
        _wasSaved = false;
    }
    public void UpdateDelayedText(TMP_InputField input)
    {
        _currentCard.DelayedTextToSay = input.text;
        _wasSaved = false;
    }

    public void CardType(TMP_Dropdown drop)
    {
        switch (drop.value)
        {
            case 0:
                _currentCard.CardType = Type.NoEffect;
                break;
            case 1:
                _currentCard.CardType = Type.Instant;
                break;
            case 2:
                _currentCard.CardType = Type.DelayedEffect;
                break;
            case 3:
                _currentCard.CardType = Type.Blank;
                break;
            default:
                break;
        }
        _wasSaved = false;
    }

    public void CardDelayedType(TMP_Dropdown drop)
    {
        switch (drop.value)
        {
            case 0:
                _currentCard.DelayedEffectSide = Type2.Null;
                break;
            case 1:
                _currentCard.DelayedEffectSide = Type2.Right;
                break;
            case 2:
                _currentCard.DelayedEffectSide = Type2.Left;
                break;
            case 3:
                _currentCard.DelayedEffectSide = Type2.Both;
                break;
            default:
                break;
        }
        _wasSaved = false;
    }

    public void DelayedOnlyCheckBox(Toggle boolToggle)
    {
        _currentCard.DelayedOnly = boolToggle.isOn;
        _wasSaved = false;
    }

    public void OpenDelayedTextWindow()
    {
        if (_openD == 0)
        {
            DelayedTextWindow.SetActive(true);
            ValuesWindow.SetActive(false);
            _openD++;
        }
        else
        {
            DelayedTextWindow.SetActive(false);
            _openD = 0;
        }
    }

    public void OpenValuesWindow()
    {
        if (_openV == 0)
        {
            ValuesWindow.SetActive(true);
            DelayedTextWindow.SetActive(false);
            _openV++;
        }
        else
        {
            ValuesWindow.SetActive(false);
            _openV = 0;
        }
    }

    public void GoToLeftValueWindow()
    {
        ValuesWindowLeft.SetActive(true);
        ValuesWindowRight.SetActive(false);
    }

    public void GoToRightValueWindow()
    {
        ValuesWindowRight.SetActive(true);
        ValuesWindowLeft.SetActive(false);
        ValuesWindowDelayed.SetActive(false);
    }

    public void GoToDelayedValueWindow()
    {
        ValuesWindowRight.SetActive(false);
        ValuesWindowDelayed.SetActive(true);
        ValuesWindowDelayedBlockedCards.SetActive(false);
    }

    public void GoToCardsSetterWindow()
    {
        ValuesWindowDelayed.SetActive(false);
        ValuesWindowDelayedBlockedCards.SetActive(true);
    }

    public void ChangeValue1(Slider slider)
    {
        _currentCard.LResultToPeople = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue2(Slider slider)
    {
        _currentCard.LResultToArmy = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue3(Slider slider)
    {
        _currentCard.LResultToBelief = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue4(Slider slider)
    {
        _currentCard.LResultToMoney = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue5(Slider slider)
    {
        _currentCard.RResultToPeople = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue6(Slider slider)
    {
        _currentCard.RResultToArmy = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue7(Slider slider)
    {
        _currentCard.RResultToBelief = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue8(Slider slider)
    {
        _currentCard.RResultToMoney = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue9(Slider slider)
    {
        _currentCard.Delay = (int)slider.value;
        _wasSaved = false;
    }

    public void ChangeValue10(Slider slider)
    {
        _currentCard.BlockedTime = (int)slider.value;
        _wasSaved = false;
    }

    public void SetDelayedCardName(TMP_InputField input)
    {
        _currentCard.DelayedCardName = input.text;
        _wasSaved = false;
    }

    public void SetBlockedCardsNames(TMP_InputField input)
    {
        var found = new List<int>();
        var cards = new List<string>();
        int v = 0;

        for (int i = 0; i < input.text.Length; i++)
        {
            if (input.text[i] == '_')
            {
                found.Add(i);
                Debug.Log(i);
            }
        }

        for (int d = 0; d < found.Count; d++)
        {
            string substinged = "";
            if (d == 0)
            {
                substinged = input.text.Substring(0, found[0]);
                cards.Add(substinged);
            }
            else
            {
                v = found[d] - found[d - 1];
                substinged = input.text.Substring(found[d - 1] + 1, v - 1);
                cards.Add(substinged);
            }
        }

        _currentCard.CardsToBlockNames = new string[cards.Count];

        for (int i = 0; i < cards.Count; i++)
        {
            _currentCard.CardsToBlockNames[i] = cards[i];
        }
        _wasSaved = false;
    }

    public void SkipRight()
    {
        if (_currentCard == _createdDeck.EventCards[_createdDeck.EventCards.Count - 1])
        {
            _currentCard = _createdDeck.EventCards[0];
            _currentCardIndex = 0;
        }
        else
        {
            _currentCardIndex++;
        }
        UpdateCardInfo();
    }

    public void SkipLeft()
    {
        if (_currentCard == _createdDeck.EventCards[0])
        {
            _currentCard = _createdDeck.EventCards[_createdDeck.EventCards.Count - 1];
            _currentCardIndex = _createdDeck.EventCards.Count - 1;
        }
        else
        {
            _currentCardIndex--;
        }
        UpdateCardInfo();
    }

    public void UpdateLinesTypes()
    {
        CardImagePath.lineType = TMP_InputField.LineType.MultiLineNewline;
        CardName.lineType = TMP_InputField.LineType.MultiLineNewline;
        CardText.lineType = TMP_InputField.LineType.MultiLineNewline;
        CardLeftText.lineType = TMP_InputField.LineType.MultiLineNewline;
        CardRightText.lineType = TMP_InputField.LineType.MultiLineNewline;
        CardDelayText.lineType = TMP_InputField.LineType.MultiLineNewline;
    }

    public void UpdateCardInfo()
    {
        _currentCard = _createdDeck.EventCards[_currentCardIndex];

        CardImagePath.text = _currentCard.WhileCardImagePath;
        UpdateImagePath(CardImagePath);
        CardName.text = _currentCard.Name;
        UpdateImagePath(CardImagePath);
        CardText.text = _currentCard.TextToSay;
        CardLeftText.text = _currentCard.LeftText;
        CardRightText.text = _currentCard.RightText;
        CardDelayText.text = _currentCard.DelayedTextToSay;

        foreach (var st in _currentCard.CardsToBlockNames)
        {
            CardBlockedCardsNames.text += st + "_";
        }

        CardDelayedCardNames.text = _currentCard.DelayedCardName;

        DelayedOnly.isOn = _currentCard.DelayedOnly;

        switch (_currentCard.CardType)
        {
            case Type.NoEffect:
                CardTypeDropDown.value = 0;
                break;
            case Type.Instant:
                CardTypeDropDown.value = 1;
                break;
            case Type.DelayedEffect:
                CardTypeDropDown.value = 2;
                break;
            case Type.Blank:
                CardTypeDropDown.value = 3;
                break;
        }

        switch (_currentCard.DelayedEffectSide)
        {
            case Type2.Null:
                CardDelayedTypeDropDown.value = 0;
                break;
            case Type2.Left:
                CardDelayedTypeDropDown.value = 2;
                break;
            case Type2.Right:
                CardDelayedTypeDropDown.value = 1;
                break;
            case Type2.Both:
                CardDelayedTypeDropDown.value = 3;
                break;
        }

        Slider1.value = _currentCard.LResultToPeople;
        Slider2.value = _currentCard.LResultToArmy;
        Slider3.value = _currentCard.LResultToBelief;
        Slider4.value = _currentCard.LResultToMoney;
        Slider5.value = _currentCard.RResultToPeople;
        Slider6.value = _currentCard.RResultToArmy;
        Slider7.value = _currentCard.RResultToBelief;
        Slider8.value = _currentCard.RResultToMoney;
        Slider9.value = _currentCard.Delay;
        Slider10.value = _currentCard.BlockedTime;

    }

    public void UpdateDeckInfo()
    {
        CreateNewDeck();
        CreateCard();
    }
}
