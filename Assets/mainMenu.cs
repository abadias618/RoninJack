﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class mainMenu : MonoBehaviour
{
	// Start is called before the first frame update
	public void playGame() {
		SceneManager.LoadScene("Hunters Test Scene");
	}
	public void quitGame() {
		Debug.Log("Quit Game");
		Application.Quit();
	} 
}
