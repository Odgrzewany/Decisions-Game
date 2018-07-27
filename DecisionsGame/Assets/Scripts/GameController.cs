using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public static GameController Gc;    //Static Instance;

    private int _people;                //Main Values;
    private int _army;
    private int _belief;
    private int _money;
    private bool _over = false;

    [Header("UI")]
    public Image CardImage;
    public Image LeftSideColor;
    public Image RightSideColor;
    public TextMeshProUGUI CardName;
    public TextMeshProUGUI CardText;
    public TextMeshProUGUI DecisionText;
    public GameObject EndScreen;

    public GameObject PeopleImage;
    public GameObject ArmyImage;
    public GameObject MoneyImage;
    public GameObject BeliefImage;

    private Color _leftColor;
    private Color _rightColor;

    private readonly Color _green = new Color
    {
        a = 1,
        r = 0,
        g = 1,
        b = 0
    };

    private readonly Color _red = new Color
    {
        a = 1,
        r = 1,
        g = 0,
        b = 0
    };

    private Color _base = new Color();

    private Color _peopleColor;
    private Color _armyColor;
    private Color _moneyColor;
    private Color _beliefColor;

    private int _lToPeople;
    private int _lToArmy;
    private int _lToBelief;
    private int _rToPeople;
    private int _rToArmy;
    private int _rToBelief;
    private int _lToMoney;
    private int _rToMoney;

    private float _peopleScaler;
    private float _armyScaler;
    private float _moneyScaler;
    private float _beliefScaler;

    private int _fakePeople;
    private int _fakeArmy;
    private int _fakeMoney;
    private int _fakeBelief;

    private int _randomCardIndex;

    [Header("Deck used in this game")]
    private EventCard _currentCard;
    private List<EventCard> _currentPlayedCards;    //Cards In Current Shuffle (10 cards, -1 on each player decision, resets when <=0)  {10 bc of optimalization and Card Delays}
    private List<EventCard> _copiedDeck;            //Temporary Deck, resets in every game;

    public bool GenerateCardProcedurally;

    private void Awake()                            //Awake
    {
        if (Gc == null) Gc = this;                  //Set Ins to not empty
        else Destroy(this);                         //Or destroing it (not used in this case, but for practice ;))
    }

    public void Start()
    {
        _currentPlayedCards = new List<EventCard>();
        _peopleColor = PeopleImage.GetComponentInChildren<Image>().color;
        _armyColor = ArmyImage.GetComponentInChildren<Image>().color;
        _moneyColor = MoneyImage.GetComponentInChildren<Image>().color;
        _beliefColor = BeliefImage.GetComponentInChildren<Image>().color;
        _base = PeopleImage.GetComponentInChildren<Image>().color;

        _people = 50;
        _army = 50;
        _belief = 50;
        _money = 50;

        _fakeArmy = _army;
        _fakeBelief = _belief;
        _fakeMoney = _money;
        _fakePeople = _people;

        ScaleValuesAndImages();

        EndScreen.SetActive(false);

        _rightColor = RightSideColor.color;         //Sets images;
        _leftColor = LeftSideColor.color;

        _copiedDeck = XMLSerializerScript.Ins.DeckChoosen.EventCards;         //Copy Deck;

        _currentPlayedCards = new List<EventCard>();    //Current Played Cards too;

        GenerateDeck(false);                             //Generates Deck (10 cards);
    }

    public void LeftSide()                          //Executes when player drop card on specific side;
    {
        if (_currentCard.CardType == Type.Blank)    //Skips blank cards (traps);
        {
            RemoveCard(_currentCard);
            return;
        }

        _people += _lToPeople;
        _army += _lToArmy;
        _belief += _lToBelief;
        _money += _lToMoney;

        UpdateScaleValue();
        ColorsToBase();

        if (_people <= 0 || _army <= 0 || _belief <= 0 || _money <= 0)  //Over? :)
        {
            GameOver();
            return;
        }

        if (_currentCard.CardType == Type.DelayedEffect)     //Check if delay needs to be set
        {
            if (_currentCard.DelayedEffectSide == Type2.Left || _currentCard.DelayedEffectSide == Type2.Both) 
                ShuffleDelayedCard(_currentCard);                                                               //Sets Delay
        }
        else
        {
            RemoveCard(_currentCard);                       //ClearThisCard
        }
    }

    public void RightSide()                         //Executes when player drop card on specific side;
    {
        if (_currentCard.CardType == Type.Blank)    //Skips blank cards (traps) {next as left, but right :)}
        {
            RemoveCard(_currentCard);
            return;
        }

        _people += _rToPeople;
        _army += _rToArmy;
        _belief += _rToBelief;
        _money += _rToMoney;

        UpdateScaleValue();
        ColorsToBase();

        if (_people <= 0 || _army <= 0 || _belief <= 0 || _money <= 0)
        {
            GameOver();
            return;
        }

        if (_currentCard.CardType == Type.DelayedEffect)        //Check card type;
        {
            if (_currentCard.DelayedEffectSide == Type2.Right || _currentCard.DelayedEffectSide == Type2.Both)
                ShuffleDelayedCard(_currentCard);
        }
        else
        {
            RemoveCard(_currentCard);
        }
    }

    public void NormalSide()                        //Sets Left_Right Side to invisible;
    {
        _rightColor.a = 0f;
        _leftColor.a = 0f;

        RightSideColor.color = _rightColor;
        LeftSideColor.color = _leftColor;

        ColorsToBase();
    }

    private void UpdateColors()
    {
        PeopleImage.GetComponentInChildren<Image>().color = _peopleColor;
        ArmyImage.GetComponentInChildren<Image>().color = _armyColor;
        MoneyImage.GetComponentInChildren<Image>().color = _moneyColor;
        BeliefImage.GetComponentInChildren<Image>().color = _beliefColor;
    }

    private void ColorsToBase()
    {
        PeopleImage.GetComponentInChildren<Image>().color = _base;
        ArmyImage.GetComponentInChildren<Image>().color = _base;
        MoneyImage.GetComponentInChildren<Image>().color = _base;
        BeliefImage.GetComponentInChildren<Image>().color = _base;
    }

    public void HighlightLeft()                     //Executes when player is dragging card on a specific side;
    {
        if (Math.Abs(LeftSideColor.color.a - 0.5f) <= 0) return;

        UpdateFakesLeft();


        if (_currentCard.LResultToArmy != 0)
        {
            if (_currentCard.LResultToArmy < 0)
            {
                _armyColor = _red;
            }
            else if (_currentCard.LResultToArmy > 0)
            {
                _armyColor = _green;
            }
        }
        else
        {
            _armyColor = _base;
        }

        if (_currentCard.LResultToBelief != 0)
        {
            if (_currentCard.LResultToBelief < 0)
            {
                _beliefColor = _red;
            }
            else if (_currentCard.LResultToBelief > 0)
            {
                _beliefColor = _green;
            }
        }
        else
        {
            _beliefColor = _base;
        }

        if (_currentCard.LResultToMoney != 0)
        {
            if (_currentCard.LResultToMoney < 0)
            {
                _moneyColor = _red;
            }
            else if (_currentCard.LResultToMoney > 0)
            {
                _moneyColor = _green;
            }
        }
        else
        {
            _moneyColor = _base;
        }

        if (_currentCard.LResultToPeople != 0)
        {
            if (_currentCard.LResultToPeople < 0)
            {
                _peopleColor = _red;
            }
            else if (_currentCard.LResultToPeople > 0)
            {
                _peopleColor = _green;
            }
        }
        else
        {
            _peopleColor = _base;
        }

        UpdateColors();

        _fakeArmy = _army;
        _fakeBelief = _belief;
        _fakeMoney = _money;
        _fakePeople = _people;

        _rightColor.a = 0f;
        _leftColor.a = 0.5f;

        RightSideColor.color = _rightColor;
        LeftSideColor.color = _leftColor;

        DecisionText.text = _currentCard.LeftText;

        ScaleValuesAndImages();
    }

    public void HighlightRight()                    //Executes when player is dragging card on a specific side;
    {
        if (Math.Abs(RightSideColor.color.a - 0.5f) <= 0) return;
        
        UpdateFakesRight();


        if (_currentCard.RResultToArmy != 0)
        {
            if (_currentCard.RResultToArmy < 0)
            {
                _armyColor = _red;
            }
            else if (_currentCard.RResultToArmy > 0)
            {
                _armyColor = _green;
            }
        }
        else
        {
            _armyColor = _base;
        }

        if (_currentCard.RResultToBelief != 0)
        {
            if (_currentCard.RResultToBelief < 0)
            {
                _beliefColor = _red;
            }
            else if (_currentCard.RResultToBelief > 0)
            {
                _beliefColor = _green;
            }
        }
        else
        {
            _beliefColor = _base;
        }

        if (_currentCard.RResultToMoney != 0)
        {
            if (_currentCard.RResultToMoney < 0)
            {
                _moneyColor = _red;
            }
            else if (_currentCard.RResultToMoney > 0)
            {
                _moneyColor = _green;
            }
        }
        else
        {
            _moneyColor = _base;
        }

        if (_currentCard.RResultToPeople != 0)
        {
            if (_currentCard.RResultToPeople < 0)
            {
                _peopleColor = _red;
            }
            else if (_currentCard.RResultToPeople > 0)
            {
                _peopleColor = _green;
            }
        }
        else
        {
            _peopleColor = _base;
        }

        UpdateColors();

        _rightColor.a = 0.5f;
        _leftColor.a = 0f;

        RightSideColor.color = _rightColor;
        LeftSideColor.color = _leftColor;

        DecisionText.text = _currentCard.RightText;

        ScaleValuesAndImages();
    }

    private void UpdateFakesLeft()
    {
        _fakeArmy = _army;
        _fakeBelief = _belief;
        _fakeMoney = _money;
        _fakePeople = _people;

        _fakePeople += _currentCard.LResultToPeople;
        _fakeArmy += _currentCard.LResultToArmy;
        _fakeBelief += _currentCard.LResultToBelief;
        _fakeMoney += _currentCard.LResultToMoney;

    }

    private void UpdateFakesRight()
    {
        _fakeArmy = _army;
        _fakeBelief = _belief;
        _fakeMoney = _money;
        _fakePeople = _people;

        _fakePeople += _currentCard.RResultToPeople;
        _fakeArmy += _currentCard.RResultToArmy;
        _fakeBelief += _currentCard.RResultToBelief;
        _fakeMoney += _currentCard.RResultToMoney;

    }

    private void GenerateDeck(bool fillToDelayy)                     //Core of the script (not exacly but...);
    {
        if(GenerateCardProcedurally)
            ProceduralCardGeneration();
        else
            RandomCardGeneration(fillToDelayy);
    }

    private void RandomCardGeneration(bool fillToDelayy)             //There we are! (1/2 cores :P);
    {
        if (fillToDelayy)
        {
            var off = _currentCard.Delay - _currentPlayedCards.Count;   //How many cards add?
            
            if (_copiedDeck.Count >= off)           //If able to do that;
            {
                for (int x = 0; x < off; x++)       //Add reaming cards;
                {
                    if (_copiedDeck[x].DelayedOnly && _copiedDeck.Count == 1)       //If there is only one card left in the deck, and is delayed only - end game;
                    {
                        GameOver();
                        return;
                    } 

                    if(_copiedDeck[x].DelayedOnly) continue;                    //If card is delayed only - continue;
                    _currentPlayedCards.Add(_copiedDeck[x]);                    //Add this card to the playing deck;
                    _copiedDeck.Remove(_copiedDeck[x]);                         //Remove from the base deck;
                }

                ShuffleDelayedCard(_currentCard);                               //Repeat Shuffling;

                return;                                                         //Come out;
            }

            foreach (var c in _copiedDeck)
            {
                _currentPlayedCards.Add(c);
            }

            return;
        }

        _currentPlayedCards = new List<EventCard>();

        if (_copiedDeck.Count <= 0)
        {
            GameOver();
            return;
        }

        if (_copiedDeck.Count < 10 && _copiedDeck.Count > 0)
        {
            for (int x = 0; x < _copiedDeck.Count; x++)
            {
                if (_copiedDeck[x].DelayedOnly)continue;
                _currentPlayedCards.Add(_copiedDeck[x]);
                _copiedDeck.Remove(_copiedDeck[x]);
            }
            DisplayCard();
            return;
        }

        for (int x = 0; x < 10; x++)
        {
            _randomCardIndex = UnityEngine.Random.Range(0, _copiedDeck.Count);  //Lotto 1,2,3;
            if (_copiedDeck[_randomCardIndex].DelayedOnly) continue;            //If is only for delay activation, continue;
            _currentPlayedCards.Add(_copiedDeck[_randomCardIndex]);             //Add to current played cards;
            _copiedDeck.Remove(_copiedDeck[_randomCardIndex]);                  //Remove selected card from deck (for no repeating);
        }
        DisplayCard();                  //Update card view;
    }

    private void ProceduralCardGeneration()         //Experimental Card Generation;
    {
        //Next Day
    }

    private void DisplayCard() //Updates card view;
    {
        if (_over) return;
        _currentCard = _currentPlayedCards[0];
        EventCard card = _currentCard;
        UpdateValues(card);                         //Values;
        Sprite icon = XMLSerializerScript.Ins.LoadNewSprite(card.CardImagePath);
        CardImage.sprite = icon;       //Image;
        CardName.text = card.Name;                  //Name;
        CardText.text = card.TextToSay;   //Text
        DecisionText.text = $"Hmmm...";
    }

    private void UpdateValues(EventCard card)       //Update card ints;
    {
 
        _lToArmy = card.LResultToArmy;
        _lToBelief = card.LResultToBelief;
        _lToMoney = card.LResultToMoney;
        _lToPeople = card.LResultToPeople;
        //////////////////////////////////
        _rToPeople = card.RResultToPeople;
        _rToMoney = card.RResultToMoney;
        _rToBelief = card.RResultToBelief;
        _rToArmy = card.RResultToArmy;
    }

    private void RemoveCard(EventCard card)         //Removes card from Deck;
    {
        _currentPlayedCards.Remove(card);

        if (_currentPlayedCards.Count <= 0)         //Check if current deck is empty;
        {
            GenerateDeck(false);
        }

        if (_currentCard.IsDelayed) _currentCard.IsDelayed = false;     //Unchecks delay;

        DisplayCard();                              //Updates card view;
        NormalSide();                               //Sets Right/Left Sides to invisble;
                                                    ////R.I.P
    }
    private void GameOver()                         // R.I.P
    {
        EndScreen.SetActive(true);
        _over = true;
    }

    private void ShuffleDelayedCard(EventCard card)     //Shufle Delayed Card;
    {
        if (_currentPlayedCards.Count >= card.Delay)    //If there is enough storage;
        {
            var delayedCard = new EventCard();

            foreach (EventCard delCard in _copiedDeck)
            {
                if (card.DelayedCardName != delCard.Name) continue;
                delayedCard = delCard;
            }

            if (_currentPlayedCards.Count == card.Delay)    //If storage is equal to delay;
            {
                delayedCard.TextToSay = card.DelayedTextToSay;
                _currentPlayedCards.Add(delayedCard);  //Add Delayed Card;
                _copiedDeck.Remove(delayedCard);       //Remove doubles :) ;
                delayedCard.IsDelayed = true;          //Set IsDelayed to true;
            }           
            else                                        //If there is enough storage;
            {
                var replacedCard = _currentPlayedCards[card.Delay];     //Save card on this position;
                    
                _currentPlayedCards[card.Delay] = delayedCard;     //Replace cards;
                _currentPlayedCards.Add(replacedCard);                  //Add saved card;
            }
        }
        else
        {
            RandomCardGeneration(true);
        }

        RemoveCard(card);                               //Remove card;
    }

    private void UpdateScaleValue()
    {
        _peopleScaler = (float)_fakePeople / 100;
        _armyScaler = (float)_fakeArmy / 100;
        _moneyScaler = (float)_fakeMoney / 100;
        _beliefScaler = (float)_fakeBelief/ 100;
    }

    private void ScaleValuesAndImages()
    {
        UpdateScaleValue();

        PeopleImage.transform.localScale = new Vector2(PeopleImage.transform.localScale.x, _peopleScaler);
        ArmyImage.transform.localScale = new Vector2(ArmyImage.transform.localScale.x, _armyScaler);
        MoneyImage.transform.localScale = new Vector2(MoneyImage.transform.localScale.x, _moneyScaler);
        BeliefImage.transform.localScale = new Vector2(BeliefImage.transform.localScale.x, _beliefScaler);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}