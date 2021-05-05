using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FruitType
{
	One=0,
	Two= 1, 
	Three = 2,
	Four =3,
	Five =4,
	Six = 5,
	Seven = 6,
	Eight = 7,
	Nine = 8,
	Ten = 9,
	Eleven = 10,
}

public enum FruitState
{
    Ready = 0,
    StandBy = 1,
    Dropping = 2,
    Collision = 3,
}

public class Fruit : MonoBehaviour {
	//Unity, Public variables or parameters
	//in the script can be visually modified in the Unity engine and the Inspector view
	public FruitType fruitType = FruitType.One;

	private bool IsMove = false;

	public FruitState fruitState = FruitState.Ready;

	public float limit_x = 2f;
	public Vector3 originalScale = Vector3.zero; 
	public float scaleSpeed = 0.01f; 

	public float fuirtScore = 1f;


	void Awake()
    {
		originalScale = new Vector3(0.5f,0.5f,0.5f);
	}
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

		//Game status StandBy&Fruit status StandBy, you can click the mouse to control movement,
		//and release the mouse to drop
		if (GameManager.gameManagerInstance.gameState == GameState.StandBy && fruitState == FruitState.StandBy)
		{
			if (Input.GetMouseButtonDown(0))
			{
				IsMove = true;
			}
			// release the mouse
			if (Input.GetMouseButtonUp(0) && IsMove)
			{
				IsMove = false;
				//Change the gravity and let the fruit fall by itself
				this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1.0f;
				fruitState = FruitState.Dropping;//Fruit Step 2
				GameManager.gameManagerInstance.gameState = GameState.InProgress;//Step2

				//Create a new standby fruit
				GameManager.gameManagerInstance.InvokeCreateFruit(0.5f);
				GameManager.gameManagerInstance.dropSource.Play();
			}
			if (IsMove)
			{
				//change posotion
				Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				this.gameObject.GetComponent<Transform>().position = new Vector3(mousePos.x, this.gameObject.GetComponent<Transform>().position.y, this.gameObject.GetComponent<Transform>().position.z);

			}
		}

		//limit the range in the x direction
		if (this.transform.position.x > limit_x)
        {
            this.transform.position = new Vector3(limit_x, this.transform.position.y, this.transform.position.z);
        }
        if (this.transform.position.x < -limit_x)
        {
            this.transform.position = new Vector3(-limit_x, this.transform.position.y, this.transform.position.z);
        }

		//Size recovery
		if (this.transform.localScale.x<originalScale.x)//If it's smaller than the original size, it grows faster
		{
			this.transform.localScale += new Vector3(1, 1, 1) * scaleSpeed;
		}
		if (this.transform.localScale.x > originalScale.x)
        {
			this.transform.localScale = originalScale;
		}

	}

	void OnCollisionEnter2D(Collision2D collision)
    {
        if (fruitState ==FruitState.Dropping)
		{
			// Collision to Floor
			if (collision.gameObject.tag.Contains("Floor"))
			{
				GameManager.gameManagerInstance.gameState = GameState.StandBy;
				fruitState = FruitState.Collision;//Fruit Step 3

				GameManager.gameManagerInstance.hitSource.Play();
			}
			// Collision to Fruit
			if (collision.gameObject.tag.Contains("Fruit"))
			{
				GameManager.gameManagerInstance.gameState = GameState.StandBy;
				fruitState = FruitState.Collision;//Fruit Step 3

				GameManager.gameManagerInstance.furitCollied.Play();
			}
		}

		//Dropping，Collision，Can be combined
		if ((int)fruitState>=(int)FruitState.Dropping)
        {
			if (collision.gameObject.tag.Contains("Fruit"))
			{
                if (fruitType==collision.gameObject.GetComponent<Fruit>().fruitType&&fruitType!=FruitType.Eleven)
                {
					//Restrict to only perform one combined
					float thisPosxy = this.transform.position.x + this.transform.position.y;
					float collisionPosxy = collision.transform.position.x + collision.transform.position.y;
                    if (thisPosxy> collisionPosxy) // greater than means they have different position.
												   // Combine only in this case
					{
						//Combined, generate a new size one fruit at the collision position,
						//and the size will change from small to large
						GameManager.gameManagerInstance.CombineNewFruit(fruitType, this.transform.position, collision.transform.position);
						//Score
						GameManager.gameManagerInstance.TotalScore += fuirtScore;
						GameManager.gameManagerInstance.totalScore.text = GameManager.gameManagerInstance.TotalScore.ToString();

						Destroy(this.gameObject);
						Destroy(collision.gameObject);// If it is not destroyed, it will be executed twice
													  // In this case, two identical fruits will always be combined, because the program is constantly monitoring
													  // The only difference between them is the location
					}
				}
			}
		}

    }
}
