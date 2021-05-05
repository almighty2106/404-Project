using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// < summary >
/// Default state is Ready;
/// Click the mouse to control the fruit position StandBy;
/// Release the mouse fruit drop, InProgress
/// After the fruit falls and hits the floor or other fruit, come back to the StandBy state
/// Fruit is upper the Topline, GameOver
/// After the game is over, delay for 0.5s and calculate the score CalculateScore
/// 
///</ summary >
public enum GameState
{
    Ready = 0,
    StandBy = 1,
    InProgress = 2,
	GameOver = 3,
    CalculateScore = 4,
}

public class GameManager : MonoBehaviour {
	public GameObject[] fruitList;
	public GameObject bornFruitPosition;

	public GameObject startBtn;

	public static GameManager gameManagerInstance;// Static instances can be used directly in other classes

	public GameState gameState = GameState.Ready;

	public Vector3 combineScale = new Vector3(0,0,0);

	public float TotalScore = 0f;
	public Text totalScore;
	public Text highestScoreText;

	public AudioSource combineSource;
	public AudioSource hitSource;
	public AudioSource dropSource;
	public AudioSource furitCollied;
	public AudioSource start;

	// Called once before the GameObject is enabled
	void Awake()
    {
		gameManagerInstance = this;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void StartGame()
    {
		Debug.Log("start");

		float highestScore = PlayerPrefs.GetFloat("HighestScore");
		highestScoreText.text = " " + highestScore;

		CreateFruit();
		gameState = GameState.StandBy;//step1
		startBtn.SetActive(false);
	}
	public void InvokeCreateFruit(float invokeTime)
    {
		Invoke("CreateFruit", invokeTime);
    }
	// The standby fruit is weightless
	public void CreateFruit()
	{
		int index = Random.Range(0, 5);//0,1,2,3,4
		if (fruitList.Length >= index && fruitList[index] != null)
		{
			GameObject fruitObj = fruitList[index];
			var currentFruit = Instantiate(fruitObj, bornFruitPosition.transform.position, fruitObj.transform.rotation);
			currentFruit.GetComponent<Fruit>().fruitState = FruitState.StandBy;//Fruit Step1
		}
	}


	public void CombineNewFruit(FruitType currentFruitType,Vector3 currentPos,Vector3 collisionPos)
    {
		Vector3 centerPos = (currentPos + collisionPos) / 2; 
		// Calculate the collision center position to combine the new fruit

		int index = (int)currentFruitType + 1; //become bigger fruit
		GameObject combineFruitObj = fruitList[index]; // The index value tells which fruit is being combined
		var combineFruit = Instantiate(combineFruitObj, centerPos, combineFruitObj.transform.rotation);
		// instantiate

		combineFruit.GetComponent<Rigidbody2D>().gravityScale = 1f;
		// The newly combined fruit doesnt have gravity since it is on standby state.
		// We're going to put gravity on it
		combineFruit.GetComponent<Fruit>().fruitState = FruitState.Collision;
		combineFruit.transform.localScale = combineScale;
		// The effect of fruit composition is increased from small to large

		combineSource.Play();
	}


}
