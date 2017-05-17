using UnityEngine;
using System.Collections;

//This class handles the game's logic and some of the display functions too.
	//The rest of the display logic is delegated to other functions.
public class GameScript : MonoBehaviour {
	
	//This class's global variable turns it into a singleton.
	public static GameScript myGameState;
	
	//Game Elements.
		public TextMesh GameMessageText;
		public TeclaScript[] NumPadKeys;
		public AudioClip[] NumPadAudioClips;
	
	//Variables that control the sequence playback for the key-panels.
		public bool isArraySequenceFinished;
		float seq_waitTime= 0.2f;
		float seq_keyPressTime=0.5f;
		float seq_endOfSequence= 0.2f;
		float seq_beginASequence=0.6f;
		int currentIndexInSequence;
	
	
	//Game Logic variables.
		bool isSpacePressed;
		int currentLevel;
		int numberOfLives;	
		ArrayList numberSequence;
		//variable that keeps track of the player's progress copying a keypad sequence.
		int playerSeq_index;
	
	
	// Initialization function.
	void Start () {
	
		//Note: the game requires a number pad in your keyboard in order to be played.
			//Also, there isn't a winning ending.
			//The game proceeds to get harder until the player reaches a game over state.

		//Global variable is initialized.
			myGameState=this;
		
		//Initializing the GameMessage text..
			GameMessageText.text= Constants.startGametxt;
		 
		
		//Setting Game Logic vars.
			isSpacePressed=false;
			currentLevel=1;
			numberOfLives=3;	//This refers to the number of tries available in the game.
		
			//Initializing the arrayList that will hold the button sequences
				//the player must copy.
			numberSequence= new ArrayList();
		
			//Setting the current sequence number at 0.
			playerSeq_index=0;		
			
		//Setting playback variables.
			currentIndexInSequence=0;
			isArraySequenceFinished=false;
		
	} 
	
	//GameScript checks the player input once, in the beginning, to start the game.
	void Update()
	{
			
		//only used for the first time you start the game.
		if( (Input.GetKeyDown("space")) && !isSpacePressed)
		{
			isSpacePressed=true;

			resetAndPlayNextLevel();
		}	
	}
	
	void resetAndPlayNextLevel()
	{

			//The game will begin by generating a sequence of button highlights
			//that the player must imitate.

			//The sequence hasn't started yet, so this counter is reset.
			currentIndexInSequence=0;
			
			//Empty out the array list of sequences previously generated.
			numberSequence.Clear();
			
			generateSequence();
			
			//Once a sequence has been generated, play it back for the player to follow.
			StartCoroutine(playSequence());	
			GameMessageText.text=Constants.listentxt;
		
		
	}
	
	//This function is called if the player has failed to recreate the sequence.
	void resetSameLevel()
	{
			//resetting the sequence counter.
			currentIndexInSequence=0;
			
			//The same level is replayed. No sequence is generated.
			
			//Replay the current sequence.
			StartCoroutine(playSequence());	
			GameMessageText.text=Constants.listentxt;
		
		
	}	
	
	
//This is the coroutine that plays the button sequence to be followed.
	IEnumerator playSequence()	
	{
		
		while(!isArraySequenceFinished)	
		{
			
			//Get the current array element (the key to be highlighted)
			int currentKey= (int)numberSequence[currentIndexInSequence];
			
			//play an audio effect for the specific key.
			audio.clip=NumPadAudioClips[currentKey];
			audio.Play();
			
			//Iisplay the key highlight.
			NumPadKeys[currentKey].keyIsPressed_Sequence();
			
			//wait one moment.
			yield return new WaitForSeconds(seq_keyPressTime);
			
			//Turn off the key highlight; in other words, dePress the Key.
			NumPadKeys[currentKey].keyIsUnpressed_Sequence();
			
			//increase the currentIndex of the sequence.
			currentIndexInSequence++;
			
			//if the entire button sequence has been played:
			if(currentIndexInSequence >= numberSequence.Count)
			{
				//set the boolean variable and display a text prompt for the player.
				isArraySequenceFinished= true;
				GameMessageText.text= Constants.yourTurntxt;
				yield return new WaitForSeconds(seq_endOfSequence);
			}

			//if still playing the sequence, wait between presses.
			else 
				yield return new WaitForSeconds(seq_waitTime);
			
		}	
			
	}	
	
//Functions that generate button sequences and perform them.
	void generateSequence()
	{
		int tempInt;

		//We will add random keypad numbers in a loop.
		for(int i=0; i<currentLevel; i++)	
		{
			tempInt= Random.Range(0,8);
			numberSequence.Add(tempInt);
		}	

		//increasing the counter keeping track of 
		//the number of sequences.		
		currentLevel++;
		
	}
	
	public void checkSequence(int numberOfKey)
	{
		
		//Check the numberOfKey with the numberSequence(playerSeq_Index).
		//If the numbers match, increase the playerSeq_Index.
		
		
		//numberOfKey is offset by 1, to compare data with the array.
		//If the key pressed by the player is correct:
		if((numberOfKey-1) == (int)numberSequence[playerSeq_index])
		{
			//increase the player's index in sequence counter.
			playerSeq_index++;

			
			//play the key's audio effect.
			audio.clip=NumPadAudioClips[numberOfKey-1];
			audio.Play();
			
			//If we have reached the last matching number,
				//Change the Game Message Text to "Good!".
				//Do a delay.
				//Run the next level.
			if(playerSeq_index>= numberSequence.Count)
			{
				
				//unlighting the Last Key Pressed.
				//NumPadKeys[numberOfKey-1].keyIsNOTPressed();

				
				print("To next level.");
				
				StartCoroutine(levelClearedRoutine());
			
			}	
			

			
		}	
		
		//If the numbers do not match, play the sequence again.
			//Also, increase the mistakes counter by 1.
			//Change the Game Message Text to "Oops!".
		else
		{
			
			playerSeq_index=0;
			
			//play a 'failure' audio effect
			audio.clip=NumPadAudioClips[Constants.mistakeSoundIndex];
			audio.Play();
			
			StartCoroutine(levelFailedRoutine());	
			
		}
		
		
	}
	

	//2 more coroutines.
	IEnumerator levelClearedRoutine()	
	{
		
		playerSeq_index=0;
		
		isArraySequenceFinished=false;

		
		//Change the Game Message Text to "Good!".
		//Perform a delay.
		//Run the next level.		
		
		GameMessageText.text= Constants.goodtxt;
		
		yield return new WaitForSeconds(seq_beginASequence);

		resetAndPlayNextLevel();
		
		
	}
	
	
	IEnumerator levelFailedRoutine()	
	{
		
		isArraySequenceFinished=false;

		
		//If the numbers do not match, play the sequence again
			//also increase the mistakes counter by 1.
			//Change the Game Message Text to "Oops!".		
		
		numberOfLives--;
		
		//If the player has lost all their lives:
		if(numberOfLives<=0)
		{
			isArraySequenceFinished=false;
		
			GameMessageText.text= Constants.gameOvertxt;
			
		}
		
		//If not, try the same sequence again.
		else
		{
		
			GameMessageText.text= Constants.mistaketxt;
		
			yield return new WaitForSeconds(seq_beginASequence);
		
			resetSameLevel();
		
		}
	
	}
	
	
	//GUI event loop for controlling the unityGUI in the upper left corner.
	//Only checking if the player has pressed the 'reset' button during the game.
	void OnGUI(){	
		
		GUI.color= Color.black;
		
		GUILayout.Label("You have "+ numberOfLives + " lives left.");
		
		GUILayout.Label("Current level is "+ ( currentLevel -1) );

		GUI.color= Color.white;
		
		if(GUILayout.Button("Restart")) 
			Application.LoadLevel(Application.loadedLevel);
		
	}
	
}
