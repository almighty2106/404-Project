using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopLine : MonoBehaviour {
	public bool IsMove = false;
	public float speed = 0.1f;
	public float limit_y = -5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (IsMove)
		{
			if (this.transform.position.y> limit_y)
			{
				this.transform.Translate(Vector3.down * speed);
			}
            else
            {
				IsMove = false;
				Invoke("ReLoadScene",1f);// Reload the game
			}
		}
	}

	//Collision trigger
	void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag.Contains("Fruit"))
		{
			Debug.Log("topLinefruit");
			//Determine if the game is over
			if ((int)GameManager.gameManagerInstance.gameState< (int)GameState.GameOver)
			{
				//And the fruit is in the Collision state
				if (collider.gameObject.GetComponent<Fruit>().fruitState == FruitState.Collision)
				{
					//GameOver
					GameManager.gameManagerInstance.gameState = GameState.GameOver;
					GameManager.gameManagerInstance.start.Play();
					Invoke("OpenMoveAndCalculateScore", 0.5f);
				}
			}

            //Calculate score
            if (GameManager.gameManagerInstance.gameState == GameState.CalculateScore)
            {
				Debug.Log("Score");
				float currentScore = collider.GetComponent<Fruit>().fuirtScore;
				GameManager.gameManagerInstance.TotalScore += currentScore;
				GameManager.gameManagerInstance.totalScore.text = GameManager.gameManagerInstance.TotalScore.ToString();
				Destroy(collider.gameObject);
			}
		}

    }

	//Turn on the line moving down switch, and the GameState state changes to CalculateScore
	void OpenMoveAndCalculateScore()
    {
		IsMove = true;
		GameManager.gameManagerInstance.gameState = GameState.CalculateScore;
	}
	void ReLoadScene()
    {
		//Set the highest score
		float highestScore = PlayerPrefs.GetFloat("HighestScore");
        if (highestScore < GameManager.gameManagerInstance.TotalScore)
		{
			PlayerPrefs.SetFloat("HighestScore", GameManager.gameManagerInstance.TotalScore);
		}

		SceneManager.LoadScene("Build Watermelon!");
    }

}
