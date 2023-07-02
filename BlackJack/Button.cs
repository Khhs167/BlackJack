using Raylib_cs;
using System.Numerics;

namespace BlackJack;

public class Button
{
	private readonly Vector2 position;
	private readonly string text;
	public Button(Vector2 position, string text)
	{
		this.position = position;
		this.text = text;
	}

	private const int FontSize = 40;

	public Vector2 Size()
	{
		return new Vector2(Raylib.MeasureText(text, FontSize) + 8, FontSize + 8);
	}

	public bool UpdateAndDraw()
	{
		if(!Hovered())
			DrawNonHoveredBase();
		else
			DrawHoveredBase();

		Raylib.DrawText(text, (int)position.X + 4, (int)position.Y + 4, FontSize, Color.BLACK);
		
		return Hovered() && Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT);
	}

	private bool Hovered()
	{
		Vector2 mouse = Raylib.GetMousePosition();
		return mouse.X > position.X && mouse.Y > position.Y && mouse.Y < position.Y + Size().Y && mouse.X < position.X + Size().X;
	}
	
	private void DrawNonHoveredBase() 
	{
		Raylib.DrawRectangleV(position, Size(), Color.GRAY);
		Raylib.DrawRectangleV(position + Vector2.One, Size() - Vector2.One * 2, Color.DARKGRAY);
	}
		
	private void DrawHoveredBase() 
	{
		Raylib.DrawRectangleV(position, Size(), Color.BLACK);
		Raylib.DrawRectangleV(position + Vector2.One, Size() - Vector2.One * 2, Color.GRAY);
	}
}
