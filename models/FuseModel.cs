using Godot;
using System;
using System.Linq;
using TextToSpeech.models;
//using System.Speech.Synthesis;
//using System.Globalization;

public class FuseModel : Spatial
{
	private AudioStreamPlayer3D speachPlayer;
	private AnimationTree animationTree;
	private bool isSpeaking = false;
	private string lastSentence = string.Empty;

	//private SpeechSynthesizer voice;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		speachPlayer = GetNode<AudioStreamPlayer3D>("SpeachPlayer");
		animationTree = GetNode<AnimationTree>("AnimationTree");

		//voice = new SpeechSynthesizer();
		//voice.Rate = -1;
		//voice.VisemeReached += VisemeReached;
		//SetLanguage();
	}
	/*
	public void SetLanguage()
	{
		string cultureTag = "en-US";
		voice?.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult, 0, CultureInfo.GetCultureInfo(cultureTag));
	}
	public void Speak(string sentence)
	{
		lastSentence = sentence;
		voice?.SpeakAsync(sentence);
	}
	public void CancelSpeak()
	{
		voice?.SpeakAsyncCancelAll();
	}
	

	private void VisemeReached(object sender, VisemeReachedEventArgs e)
	{
		int pos = 8;
		switch (e.Viseme)
		{
			case 21:
				pos = 0;
				break;
			case 15:
			case 16:
			case 17:
			case 19:
			case 20:
				pos = 1;
				break;
			case 1:
			case 4:
			case 6:
			case 12:
				pos = 2;
				break;
			case 9:
			case 10:
			case 11:
				pos = 3;
				break;
			case 2:
			case 3:
			case 5:
				pos = 4;
				break;
			case 7:
			case 8:
			case 13:
				pos = 5;
				break;
			case 18:
				pos = 6;
				break;
			case 14:
				pos = 7;
				break;
			case 0:
				pos = 8;
				break;
		}

		animationTree.Set($"parameters/SpeakAnims/Transition/current", pos);
	}
	*/
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("speak"))
		{
			SpeakAsync("merchant_female");
		}
		//if (Input.IsActionJustPressed("text_to_speech"))
		//{
		//	Speak("This is an example of text to speech");
		//}
	}

	private Speach speach;//this holds the Rhubarb  output
	public async System.Threading.Tasks.Task SpeakAsync(string dialog)//The method is async to wait for the signal
	{
		string audioFile = $"res://Speach/{dialog}.wav";
		string speachFile = $"res://Speach/{dialog}.json";
		//Read the Json file
		File file = new File();
		file.Open(speachFile, File.ModeFlags.Read);
		var jsonString = file.GetAsText();
		file.Close();
		speach = Newtonsoft.Json.JsonConvert.DeserializeObject<Speach>(jsonString);
		//Read the audio file
		var stream = GD.Load<AudioStreamSample>(audioFile);
		stream.LoopMode = AudioStreamSample.LoopModeEnum.Disabled;
		speachPlayer.Stream = stream;
		speachPlayer.Play();
		//Create a tween to call UpdateSpeak, we will pass a delta calculated from zero to speach.Metadata.Duration
		var tween = new Tween();
		AddChild(tween);
		tween.InterpolateMethod(this, nameof(UpdateSpeak), 0.0f, speach.Metadata.Duration, speach.Metadata.Duration, Tween.TransitionType.Linear, Tween.EaseType.InOut);
		tween.Start();
		//Wait for the tween_completed signal 
		await ToSignal(tween, "tween_completed");
		tween.QueueFree();
		isSpeaking = false;
	}

	//As mentioned UpdateSpeak will recieve the delta
	private void UpdateSpeak(float delta)
	{
		//Find the first cue in the current delta
		var cue = speach.MouthCues.FirstOrDefault(x => x.Start <= delta && x.End >= delta);
		if (cue != default)
		{
			//Update the transition, we set the "current" parameter in our  Transition node
			animationTree.Set($"parameters/SpeakAnims/Transition/current", GetVisemeValue(cue.Value));
		}
	}

	//GetVisemeValue gets the index to update our transition
	// the "A,B,C,D,E,F,G,H,X,O" string has the same order as our animations
	private int GetVisemeValue(string value)
	{
		var arrString = "A,B,C,D,E,F,G,H,X,O".Split(",");
		return Array.FindIndex(arrString, x => x.Equals(value));
	}
}
