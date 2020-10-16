using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Klasa odpowiada za przyciski wyboru poziomu na start gry
public class PickLevelMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadTestLevel()
    {
        SceneManager.LoadScene("TestScene");
    }
    public void LoadFullLevel()
    {
        SceneManager.LoadScene("_Scene_Hat");
    }
}
