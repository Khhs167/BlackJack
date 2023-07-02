using Raylib_cs;
using System.Numerics;

namespace BlackJack;

public class VisualCard
{

	private Vector2 start;
	private Vector2 end;
	private float progression;
	private float totalTime;
	private Card card;
	private Action onDone;

	private VisualCard()
	{
		
	}

	private static List<VisualCard> visualCards = new List<VisualCard>();

	public static void Begin(Vector2 start, Vector2 end, float time, Card card, Action onDone)
	{
		visualCards.Add(new VisualCard()
		{
			start = start,
			end = end,
			progression = 0,
			totalTime = time,
			card = card,
			onDone = onDone
		});
	}

	public static bool AllDone()
	{
		return visualCards.Count == 0;
	}

	public static void UpdateAndDraw(float delta)
	{
		var cards = visualCards.ToArray();
		foreach (var card in cards)
		{
			card.progression += delta;
			Vector2 pos = Raymath.Vector2Lerp(card.start, card.end, card.progression / card.totalTime);
			card.card.Draw((int)pos.X, (int)pos.Y);
			if (card.progression > card.totalTime)
			{
				visualCards.Remove(card);
				card.onDone();
			}
		}
	}
	
}
