using UnityEngine;

[System.Serializable]
public class EventCard
{
    [Header("Info")]
    public string Name;
    [TextArea] public string TextToSay;
    public string CardImagePath;
    public string WhileCardImagePath;

    [Header("Type")]
    public Type CardType;
    public Type2 DelayedEffectSide;

    [Header("Detalis - Left Side")]
    [Range(-20, 15)] public int LResultToPeople;
    [Range(-20, 15)] public int LResultToArmy;
    [Range(-20, 15)] public int LResultToBelief;
    [Range(-20, 15)] public int LResultToMoney;
    [TextArea] public string LeftText;

    [Header("Detalis - Right Side")]
    [Range(-20, 15)]public int RResultToPeople;
    [Range(-20, 15)] public int RResultToArmy;
    [Range(-20, 15)] public int RResultToBelief;
    [Range(-20, 15)] public int RResultToMoney;
    [TextArea] public string RightText;

    [Header("Delayed Info")]
    public string DelayedCardName;
    public bool IsDelayed;
    public bool DelayedOnly;
    [Range(3,10)]public int Delay;
    [TextArea] public string DelayedTextToSay;

    [Header("Blocked Cards")]
    [Range(3, 10)]
    public int BlockedTime;
    public string[] CardsToBlockNames;
}

public enum Type {NoEffect, Instant,DelayedEffect, Blank}
public enum Type2 {Null, Left ,Right, Both}