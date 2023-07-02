using BlackJack;
using Raylib_cs;
using System.Numerics;

Raylib.InitWindow(1280, 720, "Blackjack");
Card.BuildCardBacks();
Sounds.Load();

Card.CardBack = Raylib.LoadTexture("Assets/cardBack.png");

Deck dealerDeck = new Deck();
dealerDeck.Restock();
dealerDeck.Visible = false;
dealerDeck.TextY = Card.CardBack.height + 2;
dealerDeck.Name = "Dealer's Deck";

Deck burnDeck = new Deck();
burnDeck.Position.Y = 720 - Card.CardBack.height - 5;
burnDeck.Position.X = 1280 - Card.CardBack.width - 5;
burnDeck.Visible = false;
burnDeck.TextY = -18;
burnDeck.Name = "Burn Pile";

Hand playerHand = new Hand();
playerHand.Position = new Vector2(5, 720 - Card.CardBack.height - 20);

Hand houseHand = new Hand();
houseHand.Position = new Vector2(Card.CardBack.width * 2, 5);

GameState gameState = GameState.RequiresGamble;

dealerDeck.Shuffle();

float currentSleepLeft = 0;
GameState nextState = GameState.Cleaning;

Hand targetHand = playerHand;
bool shouldBeHidden = false;
bool drawn = false;

int playerTotal = 500;
int pot = 0;

int visualTotal = playerTotal;
int visualPot = 0;

void DealToHand(Hand hand, bool hidden)
{
	targetHand = hand;
	shouldBeHidden = hidden;
	nextState = gameState;
	drawn = false;
	gameState = GameState.DrawState;
}

void DrawState()
{
	if(!VisualCard.AllDone())
		return;
	
	if (dealerDeck.Empty())
	{
		burnDeck.PutIntoOther(dealerDeck);
		dealerDeck.Shuffle();
		return;
	}
	if (!drawn)
	{
		dealerDeck.DrawIntoHand(targetHand, shouldBeHidden);
		drawn = true;
	}

	Sleep(0.5f, nextState);
}

void SleepState()
{
	currentSleepLeft -= Raylib.GetFrameTime();
	if (currentSleepLeft <= 0)
		gameState = nextState;
}

void Sleep(float time, GameState state)
{
	currentSleepLeft = time;
	nextState = state;
	gameState = GameState.Sleeping;
}

void InitialDeal()
{
	if(!VisualCard.AllDone())
		return;

	if (playerHand.Length != 2)
	{
		DealToHand(playerHand, false);
		return;
	}

	if (houseHand.Length == 0)
	{
		DealToHand(houseHand, false);
		return;
	}

	if (houseHand.Length == 1)
	{
		DealToHand(houseHand, true);
		return;
	}
	
	
	Sleep(1f, GameState.PlayerIsDoingActions);
}

float buttonY = 720 - Card.CardBack.height - 80;
Button standButton = new Button(new Vector2(5f, buttonY), "Stand");
Button hitButton = new Button(new Vector2(150f, buttonY), "Hit");
Button doubleButton = new Button(new Vector2(225f, buttonY), "Double Down");
int score = 0;

void PlayerActions()
{
	if(!VisualCard.AllDone())
		return;

	if (playerHand.Worth(true) >= 21)
	{
		Sleep(1.0f, GameState.HouseActions);
	}

	if (standButton.UpdateAndDraw())
	{
		gameState = GameState.HouseActions;
	}
	else if (hitButton.UpdateAndDraw())
	{
		DealToHand(playerHand, false);
	}
	else if (playerHand.Length == 2 && pot <= playerTotal && doubleButton.UpdateAndDraw())
	{
		DealToHand(playerHand, false);
		playerTotal -= pot;
		pot *= 2;
	}
}

void HouseActions()
{
	if(!VisualCard.AllDone())
		return;

	if (houseHand.At(1).Hidden)
	{
		Sounds.PlaceCard();
		houseHand.At(1).Hidden = false;
		Sleep(1f, GameState.HouseActions);
		return;
	}

	if (houseHand.Worth(true) <= 16)
	{
		DealToHand(houseHand, false);
		return;
	}
	
	CalculateScore();
	Sleep(1f, GameState.Score);
}

void CalculateScore()
{
	int houseWorth = houseHand.Worth(true);
	int playerWorth = playerHand.Worth(true);

	if (houseWorth > 21)
		houseWorth = -1;
	if (playerWorth > 21)
		playerWorth = -1;

	int results = playerWorth - houseWorth;

	if (results < 0)
	{
		pot = 0;
		//Sounds.Lose();
	}
	else if (results > 0)
	{
		Sounds.ThrowChips(pot);
		playerTotal += pot * 2;
		pot = 0;
		//Sounds.Victory();
	}
	else
	{
		Sounds.ThrowChips(pot / 2);
		playerTotal += pot;
		pot = 0;
	}

	score = results;
}

Button okButton = new Button(new Vector2(1280 / 2f - 30, 720 - 60), "OK");

void ScoreState()
{
	Raylib.DrawRectangle(0, 0, 1280, 720, new Color(0, 0, 0, 100));

	string scoreText = score switch
	{
		> 0 => "You Win",
		< 0 => "You Lose...",
		_ => "Tie!"
	};

	DrawCenteredText(scoreText, 1280 / 2, 720 / 2, 40, Color.WHITE);

	if (okButton.UpdateAndDraw())
	{
		gameState = GameState.Cleaning;
	}
	
	
}

void Clean()
{
	if(!VisualCard.AllDone())
		return;

	if (playerHand.Length != 0)
	{
		playerHand.EmptyInto(burnDeck);
		houseHand.EmptyInto(burnDeck);
	}
	else
	{
		Sleep(0.1f, GameState.RequiresGamble);
	}
}

void DrawCenteredText(string text, int x, int y, int size, Color color)
{
	int textLength = Raylib.MeasureText(text, size);
	Raylib.DrawText(text, x - textLength / 2, y - size / 2, size, color);
}

int currentPot = 0;

float incrementTimer = 0;

void GambleState()
{
	Raylib.DrawRectangle(0, 0, 1280, 720, new Color(0, 0, 0, 100));
	
	string potText = "Pot: $" + currentPot;
	DrawCenteredText(potText, 1280 / 2, 720 / 2, 40, Color.WHITE);

	if (Raylib.IsKeyDown(KeyboardKey.KEY_UP))
	{
		if (incrementTimer <= 0)
		{
			currentPot++;
			incrementTimer = 0.05f;
		}
	}
	if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN))
	{
		if (incrementTimer <= 0)
		{
			currentPot--;
			incrementTimer = 0.05f;
		}
	}
	incrementTimer -= Raylib.GetFrameTime();

	DrawCenteredText("How much do you want to gamble?", 1280 / 2, 720 / 2 - 45, 20, Color.LIGHTGRAY);
	
	DrawCenteredText("Up arrow to increase pot. Down arrow to decrease pot.", 1280 / 2, 720 / 2 + 45, 20, Color.LIGHTGRAY);

	if (currentPot < 50)
		currentPot = 50;
	if (currentPot > playerTotal)
		currentPot = playerTotal;
	if (currentPot > 100)
		currentPot = 100;

	if (okButton.UpdateAndDraw())
	{
		pot = currentPot;
		playerTotal -= pot;
		gameState = GameState.InitialDealing;
		Sounds.ThrowChips(pot);
	}
}

float visualUpdateTimer = 0;

while (!Raylib.WindowShouldClose())
{
	Raylib.BeginDrawing();
	Raylib.ClearBackground(Color.RAYWHITE);
	
	VisualCard.UpdateAndDraw(Raylib.GetFrameTime());
	
	dealerDeck.Draw();
	playerHand.Draw();
	burnDeck.Draw();
	houseHand.Draw();

	visualUpdateTimer -= Raylib.GetFrameTime();
	if (visualUpdateTimer <= 0)
	{
		if (visualTotal < playerTotal)
			visualTotal++;
		if (visualTotal > playerTotal)
			visualTotal--;

		if (visualPot < pot)
			visualPot++;
		if (visualPot > pot)
			visualPot--;

		visualUpdateTimer = 0.01f;
	}

	string bankText = "Bank: $" + visualTotal;
	string potText = "Pot: $" + visualPot;

	Raylib.DrawText(potText, 5, 720 / 2 - 25, 40, Color.DARKBLUE);
	Raylib.DrawText(bankText, 5, 720 / 2 + 25, 40, Color.DARKBLUE);

	if (gameState == GameState.InitialDealing)
		InitialDeal();
	else if (gameState == GameState.PlayerIsDoingActions)
		PlayerActions();
	else if (gameState == GameState.HouseActions)
		HouseActions();
	else if (gameState == GameState.Cleaning)
		Clean();
	else if (gameState == GameState.Sleeping)
		SleepState();
	else if(gameState == GameState.Score)
		ScoreState();
	else if (gameState == GameState.DrawState)
		DrawState();
	else if (gameState == GameState.RequiresGamble)
		GambleState();

	Raylib.EndDrawing();
}