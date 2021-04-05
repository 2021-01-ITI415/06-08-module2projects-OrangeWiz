using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// An enum defines a variable type with a few prenamed values
public enum eCardState
{
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card
{ 
    [Header("Set Dynamically: CardProspector")]
    public eCardState state = eCardState.drawpile;
    public List<CardProspector> hiddenBy = new List<CardProspector>();
    public int layoutID;
    public SlotDef slotDef;

    override public void OnMouseUpAsButton()
    {

        if(SceneManager.GetActiveScene().name=="ClockSolitaire")
            ClockProspector.S.CardClicked(this);
        else Prospector.S.CardClicked(this);


        base.OnMouseUpAsButton();
    }
}