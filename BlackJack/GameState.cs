namespace BlackJack;

public enum GameState
{
	RequiresGamble,
	InitialDealing,
	PlayerIsDoingActions,
	HouseActions,
	Score,
	Cleaning,
	Sleeping,
	DrawState
}
