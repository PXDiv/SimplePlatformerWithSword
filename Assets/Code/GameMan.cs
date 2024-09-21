using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMan : MonoBehaviour
{
    //   [SerializeField] Slider healthBar;
    [SerializeField] Player player;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        //        healthBar.value = player.health_current;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
