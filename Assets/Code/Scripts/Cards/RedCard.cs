using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RedCard : MonoBehaviour
{
    public Card card;
    public Sprite[] cardRareza = new Sprite[3];

    public SpriteRenderer spriteRenderer;

    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI descriptionTMP;
    public TextMeshProUGUI manaCostTMP;
    public TextMeshProUGUI attackTMP;
    public TextMeshProUGUI blockTMP;
   
    public SpriteRenderer background;
    public SpriteRenderer artWork;


    void Start()
    {
        //FALTA AÑADIR RAREZABUFF  

        switch (card.rareza) {
            case Card.Rareza.Common: { 
                    spriteRenderer.sprite = cardRareza[0]; 
                    spriteRenderer.sortingLayerName = "CommonCard"; 
                    background.sortingLayerName = "CommonCard"; 
                    artWork.sortingLayerName = "CommonCardText"; 
                }break;
            case Card.Rareza.Rare: { 
                    spriteRenderer.sprite = cardRareza[1];
                    spriteRenderer.sortingLayerName = "RareCard";
                    background.sortingLayerName = "RareCard";
                    artWork.sortingLayerName = "RareCardText";
                } break;
            case Card.Rareza.SuperRare: { 
                    spriteRenderer.sprite = cardRareza[2];
                    spriteRenderer.sortingLayerName = "SuperRareCard";
                    background.sortingLayerName = "SuperRareCard";
                    artWork.sortingLayerName = "SuperRareCardText";
                } break;
            case Card.Rareza.Legendary: { 
                    spriteRenderer.sprite = cardRareza[3];
                    spriteRenderer.sortingLayerName = "LegendaryCard";
                    background.sortingLayerName = "LegendaryCard";
                    artWork.sortingLayerName = "LegendaryCardText";
                } break;
        }

        nameTMP.text = card.name;
        descriptionTMP.text = card.description;
        manaCostTMP.text = card.manaCost.ToString();
        attackTMP.text = card.attack.ToString();
        blockTMP.text = card.block.ToString();

        background.color = card.color;
        artWork.sprite = card.artWork;
    }


}
