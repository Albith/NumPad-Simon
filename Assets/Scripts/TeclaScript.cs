using UnityEngine;
using System.Collections;

//This class is attached to each of the 9 flat planes that
//represent the numberpad keys in the game.

//It keeps track of each button's state, and modifies its display,
//as dictated by the GameScript.cs

//Note: tecla means 'key', as in key from a keyboard.
public class TeclaScript : MonoBehaviour {
	
	//The Keytag variable stores the keycode that the this plane responds to.
	string Keytag;
		
	Color32 unPressedColor;
	Color32 pressedColor;
	Color32 sequencePressedColor;
	
	// Use this for initialization
	void Start () {

		//Creating the appropriate keycode for the numberpad key to be detected, 
		//based on the parent object's tag information.
		Keytag="["+gameObject.tag+ "]";
		
		//setting the color information.
		unPressedColor= gameObject.renderer.material.color;
		pressedColor= new Color32(255,0,0,255);
		
		//setting the displayed color while a keypad sequence is being played.
		sequencePressedColor= Color.blue;
		
	}
	
	
	//These two functions are called by the plane's update function,
		//when the player presses or stops pressing the plane's key.
	void keyIsPressed()
	{
		
		gameObject.renderer.material.color= pressedColor;
		
	}
	
	public void keyIsNOTPressed()
	{
		
		gameObject.renderer.material.color= unPressedColor;
		
	}
	
	//The update function checks for player input, 
		//and modifies this plane's display accordingly.
	void Update()
	{
	
		if(GameScript.myGameState.isArraySequenceFinished)
		{
			if(Input.GetKeyDown(Keytag))
			{
					//Checking the game state from the gameScript
						//when the user presses the Key.
					GameScript.myGameState.checkSequence( (int.Parse(gameObject.tag)) );
					keyIsPressed();
		
			}		
	
		}
	
		//Checking if this panel's associated key was pressed.
		if(Input.GetKeyUp(Keytag))
			{						
					keyIsNOTPressed();
				
			}
		
	}
	

	//The next two functions are
	//Display change functions called by the gameScript when running a sequence.
	public void keyIsPressed_Sequence()
	{
		
		gameObject.renderer.material.color= sequencePressedColor;

	}
	
	public void keyIsUnpressed_Sequence()
	{
		gameObject.renderer.material.color= unPressedColor;
			
	}
	
	
}
