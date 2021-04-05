using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ClockProspector : MonoBehaviour
{
	static public ClockProspector S;

	[Header("Set in Inspector")]
	public TextAsset deckpXML;
	public TextAsset layoutpXML;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;
	public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
	public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
	public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
	public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
	public float reloadDelay = 1f;
	public Text gameOverText, roundResultText, highScoreText;

	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;
	public List<CardProspector> drawPile;
	public Transform layoutAnchor;
	public CardProspector target;
	public List<CardProspector> tableau;
	public List<CardProspector> discardPile;
	public List<List<CardProspector>> cardpiles = new List<List<CardProspector>>();
	public int ckings = 4;
	List<CardProspector> pile = new List<CardProspector>();
	List<CardProspector> midpile = new List<CardProspector>();
	public FloatingScore fsRun;

	void Awake()
	{
		S = this;
		SetUpUITexts();
	}

	void SetUpUITexts()
	{
		GameObject go = GameObject.Find("HighScore");
		if (go != null)
		{
			highScoreText = go.GetComponent<Text>();
		}
		int highScore = ScoreManager.HIGH_SCORE;
		string hScore = "High Score: " + Utils.AddCommasToNumber(highScore);
		go.GetComponent<Text>().text = hScore;

		go = GameObject.Find("GameOver");
		if (go != null)
		{
			gameOverText = go.GetComponent<Text>();
		}

		go = GameObject.Find("RoundResult");
		if (go != null)
		{
			roundResultText = go.GetComponent<Text>();
		}

		ShowResultsUI(false);
	}

	void ShowResultsUI(bool show)
	{
		gameOverText.gameObject.SetActive(show);
		roundResultText.gameObject.SetActive(show);
	}

	void Start()
	{
		Scoreboard.S.score = ScoreManager.SCORE;

		deck = GetComponent<Deck>();
		deck.InitDeck(deckpXML.text);
		Deck.Shuffle(ref deck.cards);

		layout = GetComponent<Layout>();
		layout.ReadLayout(layoutpXML.text);
		drawPile = ConvertListCardsToListCardProspectors(deck.cards);
		LayoutGame();
	}

	List<CardProspector> ConvertListCardsToListCardProspectors(List<Card> lCD)
	{
		List<CardProspector> lCP = new List<CardProspector>();
		CardProspector tCP;
		foreach (Card tCD in lCD)
		{
			tCP = tCD as CardProspector;
			lCP.Add(tCP);
		}
		return (lCP);
	}


	CardProspector Draw()
	{
		CardProspector cd = drawPile[0];
		drawPile.RemoveAt(0);
		return (cd);
	}


	void LayoutGame()
	{

		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			layoutAnchor = tGO.transform;
			layoutAnchor.transform.position = layoutCenter;
		}

		CardProspector cp;
		int counter = 0;
		foreach (SlotDef tSD in layout.slotDefs)
		{
			counter++;
			cp = Draw();
			pile.Add(cp);
			cp.faceUp = tSD.faceUp;
			cp.transform.parent = layoutAnchor;
			cp.transform.localPosition = new Vector3(
				layout.multiplier.x * tSD.x,
				layout.multiplier.y * tSD.y,
				-tSD.layerID);
			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			cp.state = eCardState.tableau;
			cp.SetSortingLayerName(tSD.layerName);

			if (Random.Range(0, 100) < 10)
				cp.gold = true;

			if (counter % 4 == 0)
			{
				cardpiles.Add(pile);
				pile = new List<CardProspector>();
			}

			if (counter > 48)
			{
				midpile.Add(cp);
			}
			if (counter == 52)
			{
				cardpiles.Add(midpile);
			}

			tableau.Add(cp);
		}

		//MoveToTarget(Draw());

		UpdateDrawPile();

	}

	CardProspector FindCardByLayoutID(int layoutID)
	{
		foreach (CardProspector tCP in tableau)
		{

			if (tCP.layoutID == layoutID)
			{
				return (tCP);
			}
		}
		return (null);
	}


	void SetTableauFaces(CardProspector card)
	{
		if (card.rank <= 13)
		{
			(cardpiles[card.rank - 1])[(cardpiles[card.rank - 1].Count - 1)].faceUp = true;
		}
	}

	void MoveToDiscard(CardProspector cd)
	{

		cd.state = eCardState.discard;
		discardPile.Add(cd);
		cd.transform.parent = layoutAnchor;


		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID + 0.5f);
		cd.faceUp = true;

		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
	}


	void MoveToTarget(CardProspector cd)
	{

		if (target != null) MoveToDiscard(target);
		target = cd;
		cd.state = eCardState.target;
		cd.transform.parent = layoutAnchor;


		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID);

		cd.faceUp = true;
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}


	void UpdateDrawPile()
	{
		CardProspector cd;

		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			Vector2 dpStagger = layout.drawPile.stagger;
			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
				layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
				-layout.drawPile.layerID + 0.1f * i);

			cd.faceUp = false;
			cd.state = eCardState.drawpile;
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10 * i);
		}
	}


	public void CardClicked(CardProspector cd)
	{

		switch (cd.state)
		{
			case eCardState.target:

				break;

			case eCardState.drawpile:

				MoveToDiscard(target);
				MoveToTarget(Draw());
				UpdateDrawPile();
				ScoreManager.EVENT(eScoreEvent.draw, false);
				FloatingScoreHandler(eScoreEvent.draw);
				break;

			case eCardState.tableau:
				bool validMatch = true;
				CardProspector temp = target;
				if (!cd.faceUp)
				{
					validMatch = false;
				}
				if (!validMatch) return;

				tableau.Remove(cd);

				foreach (List<CardProspector> x in cardpiles)
				{
					x.Remove(cd);
				}
				MoveToDiscard(cd);

				if (cd.rank == 13)
				{
					ckings--;
				}

				if (ckings > 0)
					SetTableauFaces(cd);

				ScoreManager.EVENT(eScoreEvent.mine, false);
				FloatingScoreHandler(eScoreEvent.mine);
				break;
		}
		CheckForGameOver();
	}

	void CheckForGameOver()
	{
		int king = 0;
		foreach (List<CardProspector> list in cardpiles)
		{
			foreach (CardProspector cd in list)
			{
				if (cd.rank == 13)
				{
					king++;
				}
			}
		}

		if (king <= 0)
			GameOver(true);

	}

	void GameOver(bool won)
	{
		int score = ScoreManager.SCORE;
		if (fsRun != null) score += fsRun.score;
		if (won)
		{
			gameOverText.text = "Round Over";
			roundResultText.text = "You won this round! \nRound Score: " + score;
			ShowResultsUI(true);
			ScoreManager.EVENT(eScoreEvent.gameWin, false);
			FloatingScoreHandler(eScoreEvent.gameWin);
		}
		else
		{
			gameOverText.text = "Game Over";
			if (ScoreManager.HIGH_SCORE <= score)
			{
				string str = "You got the high score!\nHigh score: " + score;
				roundResultText.text = str;
			}
			else
			{
				roundResultText.text = "Your final score was: " + score;
			}
			ShowResultsUI(true);

			ScoreManager.EVENT(eScoreEvent.gameLoss, false);
			FloatingScoreHandler(eScoreEvent.gameLoss);
		}

		Invoke("ReloadLevel", reloadDelay);
	}

	void ReloadLevel()
	{
		SceneManager.LoadScene("ClockSolitaire");
	}

	void FloatingScoreHandler(eScoreEvent evt)
	{
		List<Vector2> fsPts;
		switch (evt)
		{
			case eScoreEvent.draw:
			case eScoreEvent.gameWin:
			case eScoreEvent.gameLoss:
				if (fsRun != null)
				{
					fsPts = new List<Vector2>();
					fsPts.Add(fsPosRun);
					fsPts.Add(fsPosMid2);
					fsPts.Add(fsPosEnd);
					fsRun.reportFinishTo = Scoreboard.S.gameObject;
					fsRun.Init(fsPts, 0, 1);
					fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
					fsRun = null;
				}
				break;

			case eScoreEvent.mine:
				FloatingScore fs;
				Vector2 p0 = Input.mousePosition;
				p0.x /= Screen.width;
				p0.y /= Screen.height;
				fsPts = new List<Vector2>();
				fsPts.Add(p0);
				fsPts.Add(fsPosMid);
				fsPts.Add(fsPosRun);
				fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
				fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
				if (fsRun == null)
				{
					fsRun = fs;
					fsRun.reportFinishTo = null;
				}
				else
				{
					fs.reportFinishTo = fsRun.gameObject;
				}
				break;
		}
	}

}