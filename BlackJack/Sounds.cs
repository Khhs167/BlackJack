using Raylib_cs;

namespace BlackJack;

public static class Sounds
{

	private static List<Sound> cardSounds = new List<Sound>();
	private static List<Sound> cardSlideSounds = new List<Sound>();
	private static List<Sound> chipSounds = new List<Sound>();

	private static Sound victory;
	private static Sound lose;

	private const int Instances = 10;

	public static void Load()
	{
		Raylib.InitAudioDevice();

		victory = Raylib.LoadSound("Assets/Audio/victory.wav");
		lose = Raylib.LoadSound("Assets/Audio/lose.ogg");
		
		for (int i = 0; i < Instances; i++)
		{
			cardSounds.Add(Raylib.LoadSound("Assets/Audio/cardPlace1.ogg"));
			cardSounds.Add(Raylib.LoadSound("Assets/Audio/cardPlace2.ogg"));
			cardSounds.Add(Raylib.LoadSound("Assets/Audio/cardPlace3.ogg"));

			cardSlideSounds.Add(Raylib.LoadSound("Assets/Audio/cardSlide1.ogg"));
			cardSlideSounds.Add(Raylib.LoadSound("Assets/Audio/cardSlide2.ogg"));
			cardSlideSounds.Add(Raylib.LoadSound("Assets/Audio/cardSlide3.ogg"));

			chipSounds.Add(Raylib.LoadSound("Assets/Audio/chipsCollide1.ogg"));
			chipSounds.Add(Raylib.LoadSound("Assets/Audio/chipsCollide2.ogg"));
			chipSounds.Add(Raylib.LoadSound("Assets/Audio/chipsCollide3.ogg"));
		}
	}

	public static void PlaceCard()
	{
		Sound sound = cardSounds[Random.Shared.Next(cardSounds.Count)];
		PlaySound(sound);
	}
	
	public static void ThrowChip()
	{
		Sound sound = chipSounds[Random.Shared.Next(chipSounds.Count)];
		PlaySound(sound);
	}
	
	public static void ThrowChips(int amt)
	{
		amt /= 5;
		for (int i = 0; i < amt; i++)
		{
			ThrowChip();
		}
	}

	private static void PlaySound(Sound sound)
	{
		Raylib.StopSound(sound);
		Raylib.SetSoundVolume(sound, Random.Shared.Next(80, 100) / 100f);
		Raylib.SetSoundPitch(sound, Random.Shared.Next(80, 100) / 100f);
		Raylib.PlaySound(sound);
	}

	public static void Victory()
	{
		Raylib.PlaySound(victory);
	}
	
	public static void Lose()
	{
		Raylib.PlaySound(lose);
	}
	
	public static void SlideCard()
	{
		Sound sound = cardSounds[Random.Shared.Next(cardSlideSounds.Count)];
		PlaySound(sound);
	}
}
