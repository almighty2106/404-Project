using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{

    public void loadgame()
    {

        SceneManager.LoadScene(0);


    }

    public void gameover()
    {

        Application.Quit();
    }


}
