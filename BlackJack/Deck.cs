using Raylib_cs;
using System.Numerics;

namespace BlackJack;

public class Deck
{
	private List<Card> cards = new List<Card>();
	public Vector2 Position;
	public bool Visible;

	public int TextY = -7;
	public string Name = "Unnamed Deck";

	public void Restock()
	{
		cards.Clear();
		for (int i = 0; i < 4; i++)
		{
			for (int j = 2; j <= 14; j++)
			{
				Card c = new (i, j);
				cards.Add(c);
			}
		}
	}

	public void Shuffle()
	{
		List<Card> shuffled = new List<Card>();
		while (cards.Count > 0)
		{
			Card c = cards[Random.Shared.Next(0, cards.Count)];
			c.Hidden = false;
			shuffled.Add(c);
			cards.Remove(c);
		}
		cards = shuffled;
	}

	public void Add(Card card)
	{
		cards.Add(card);
	}

	public void DrawIntoHand(Hand hand, bool hidden)
	{
		Card card = cards[0];
		cards.RemoveAt(0);
		card.Hidden = hidden;
		Sounds.SlideCard();
		
		VisualCard.Begin(Position, hand.PositionCard(hand.Length), 0.1f, card, () =>
		{
			hand.Add(card);
		});
	}

	public void PutIntoOther(Deck other)
	{

		foreach (var card in cards)
		{
			card.Hidden = !other.Visible;
			VisualCard.Begin(Position, other.Position, 0.1f, card, () => other.Add(card));
		}
		
		cards.Clear();
	}

	public void Draw()
	{
		Texture2D texture = Card.CardBack;
		if (cards.Count > 0)
			texture = Visible ? cards[0].GetTexture() : Card.CardBack;
		Raylib.DrawTextureV(texture, Position, cards.Count > 0 ? Color.WHITE : new Color(255, 255, 255, 30));

		
		Raylib.DrawText(Name, (int)Position.X + 5, (int)Position.Y + TextY, 5, Color.GRAY);
		Raylib.DrawText("Contains " + cards.Count, (int)Position.X + 5, (int)Position.Y + TextY + 8, 5, Color.GRAY);
	}

	public bool Empty()
	{
		return cards.Count == 0;
	}
}
