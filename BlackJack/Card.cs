using Raylib_cs;

namespace BlackJack;

public class Card
{

	private static Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

	public readonly string ValueName;
	public readonly int ActualValue;
	public readonly string ColourName;

	public static Texture2D CardBack;
	public bool Hidden = false;

	public Card(int colour, int value)
	{
		
		if(value <= 10)
			ValueName = value.ToString();
		else
		{
			if (value == 11)
				ValueName = "J";
			if (value == 12)
				ValueName = "K";
			if (value == 13)
				ValueName = "Q";
			if (value == 14)
				ValueName = "A";
		}
		
		if (value > 10)
			value = value == 14 ? 11 : 10;
		ActualValue = value;

		if (colour == 0)
			ColourName = "Clubs";
		if (colour == 1)
			ColourName = "Diamonds";
		if (colour == 2)
			ColourName = "Spades";
		if (colour == 3)
			ColourName = "Hearts";
	}

	public void Draw(int x, int y)
	{
		Raylib.DrawTexture(GetTexture(), x, y, Color.WHITE);
	}
	
	public void DrawBust(int x, int y)
	{
		Raylib.DrawTexture(GetTexture(), x, y, Color.GRAY);
	}

	public Texture2D GetTexture()
	{
		return Hidden ? CardBack : textures[ToString()];
	}

	public override string ToString()
	{
		return "card" + ColourName + ValueName;
	}

	public static void BuildCardBacks()
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 2; j <= 14; j++)
			{
				Card c = new Card(i, j);
				textures[c.ToString()] = Raylib.LoadTexture("Assets/Cards/" + c + ".png");
			}
		}
	}
}
