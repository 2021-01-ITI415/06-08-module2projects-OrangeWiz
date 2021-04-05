using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An enum defines a variable type with a few prenamed values
public enum State
{
    drawpile,
    tableau,
    target,
    discard
}
public class ClockCard : Card
{
    [Header("Set Dynamically: ClockCard")]
    public State state = State.drawpile;
    public int layoutID;
    public SlotDef slotDef;

    override public void OnMouseUpAsButton()
    {
        //ClockProspector.S.CardClicked(this);
        base.OnMouseUpAsButton();
    }
}