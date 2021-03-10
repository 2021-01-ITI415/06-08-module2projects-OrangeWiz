using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//The SlotDef class is not a subclass of Monobehavior, so it doesn't need a separate C# file
[System.Serializable]
public class SlotDef
{
	public float x;
	public float y;
	public bool faceUp = false;
	public string layerName = "Default";
	public int layerID = 0;
	public int id;
	public List<int> hiddenBy = new List<int>();
	public string type = "slot";
	public Vector2 stagger;
}

public class Layout : MonoBehaviour
{
	public PT_XMLReader xmlr; //Just like Deck, this has a PT_XMLReader
	public PT_XMLHashtable xml; //This variable is for easier xml access
	public Vector2 multiplier; //Sets the spacing of the tableau

	//SlotDef references
	public List<SlotDef> slotDefs; //All the SlotDefs for Row0 - Row3
	public SlotDef drawPile;
	public SlotDef discardPile;

	//This holds all of the possible names for the layers set by layerID
	public string[] sortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Discard", "Draw" };

	//This function is called to read in the LayoutXML.xml file
	public void ReadLayout(string xmlText)
	{
		xmlr = new PT_XMLReader();
		xmlr.Parse(xmlText); 
		xml = xmlr.xml["xml"][0]; 


		multiplier.x = float.Parse(xml["multiplier"][0].att("x"));
		multiplier.y = float.Parse(xml["multiplier"][0].att("y"));

		//Read in the slots
		SlotDef tSD;


		PT_XMLHashList slotsX = xml["slot"];

		for (int i = 0; i < slotsX.Count; i++)
		{
			tSD = new SlotDef(); 
			if (slotsX[i].HasAtt("type"))
			{

				tSD.type = slotsX[i].att("type");
			}
			else
			{

				tSD.type = "slot";
			}

			tSD.x = float.Parse(slotsX[i].att("x"));
			tSD.y = float.Parse(slotsX[i].att("y"));
			tSD.layerID = int.Parse(slotsX[i].att("layer"));


			tSD.layerName = sortingLayerNames[tSD.layerID];


			switch (tSD.type)
			{
				//Pull additional attributes based on the type of this <slot>
				case "slot":
					tSD.faceUp = (slotsX[i].att("faceup") == "1");
					tSD.id = int.Parse(slotsX[i].att("id"));
					if (slotsX[i].HasAtt("hiddenby"))
					{
						string[] hiding = slotsX[i].att("hiddenby").Split(',');
						foreach (string s in hiding)
						{
							tSD.hiddenBy.Add(int.Parse(s));
						}
					}
					slotDefs.Add(tSD);
					break;

				case "drawpile":
					tSD.stagger.x = float.Parse(slotsX[i].att("xstagger"));
					drawPile = tSD;
					break;

				case "discardpile":
					discardPile = tSD;
					break;
			}
		}
	}
}
