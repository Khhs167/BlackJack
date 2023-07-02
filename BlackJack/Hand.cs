using Raylib_cs;
using System.Numerics;

namespace BlackJack;

public class Hand
{
	private List<Card> cards = new List<Card>();

	public int Length
	{
		get
		{
			return cards.Count;
		}
	}

	public Vector2 Position;

	public void Add(Card card)
	{
		cards.Add(card);
		Sounds.PlaceCard();
	}

	public Vector2 PositionCard(int card)
	{
		int x = Card.CardBack.width * card;
		x += 5 * card;
		return new Vector2(Position.X + x, Position.Y);
	}

	public void EmptyInto(Deck deck)
	{
		for(int i = 0; i < cards.Count; i++)
		{
			Card card = cards[i];
			card.Hidden = !deck.Visible;
			
			VisualCard.Begin(PositionCard(i), deck.Position, 0.1f, card, () => deck.Add(card));
			Sounds.SlideCard();
		}

		cards.Clear();
	}
	
	public Card At(int idx)
	{
		return cards[idx];
	}

	public void Draw()
	{
		int worth = Worth(false);
		Raylib.DrawText("Hand Score: " + (worth < 0 ? ("> " + -worth) : (worth)), (int)Position.X, (int)Position.Y + Card.CardBack.height + 5, 10, Color.GRAY);
		for (int i = 0; i < cards.Count; i++)
		{
			Vector2 pos = PositionCard(i);
			if(Worth(false) > 21)
				cards[i].DrawBust((int)pos.X, (int)pos.Y);
			else
				cards[i].Draw((int)pos.X, (int)pos.Y);
		}
	}

	public int Worth(bool actual)
	{
		bool undetermined = false;
		int score = 0;
		foreach (var card in cards)
		{
			if (card.Hidden && !actual)
				undetermined = true;
			else
				score += card.ActualValue;
		}
		if (undetermined)
			return -score;
		return score;
	}
}
