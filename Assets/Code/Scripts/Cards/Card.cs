using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public enum Rareza { Common, Rare, SuperRare, Legendary };
    public enum RarezaBuff { Normal, Shiny, Foil };

    public Rareza rareza;
    public RarezaBuff rareBuff;

    public new string name;
    public string description;

    public Color color = new Color();

    public int attack;
    public int manaCost;
    public int block;

    public Sprite artWork;
}
