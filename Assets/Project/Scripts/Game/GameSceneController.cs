using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : MonoBehaviour
{

    [Header("Game")]
    public Player player;

    [Header("UI")]
    public Text instructionText;
    public Text timeText;
    public Text endGameText;

    private float _gameTimer;
    private bool _levelEnded;
    private float _endGameTimer = 5f;

    // Start is called before the first frame update
    void Start()
    {
        endGameText.gameObject.SetActive(false);
        player.OnCollectOrb = OnCollectOrb;
    }

    // Update is called once per frame
    void Update()
    {
        if (_levelEnded == false)
        {
            // Update game timer
            _gameTimer += Time.deltaTime;
        }
        else
        {
            _endGameTimer -= Time.deltaTime;
            if (_endGameTimer <= 0f)
            {
                LevelManager.Instance.LoadNextLevel();
            }
        }

        timeText.text = "Time: " + Mathf.FloorToInt(_gameTimer) + "s";
    }

    // 4:3
    // 16:9
    
    void OnCollectOrb()
    {
        _levelEnded = true;

        // Hide in-game text and show game over text
        instructionText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        endGameText.gameObject.SetActive(true);

        // Show end game message
        endGameText.text = "Nice job!\nYour time: " + Mathf.FloorToInt(_gameTimer) + "s";
    }
    
}
