using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager
{

	private static LevelManager _instance;
	public static LevelManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LevelManager();
				
			}

			return _instance;
		}
	}

	private int _level;

	private const int MaximumLevel = 2;
	
	public void LoadFirstLevel()
	{
		_level = 1;
		SceneManager.LoadScene("Level" + _level);
	}

	public void LoadNextLevel()
	{
		_level++;
		if (_level <= MaximumLevel)
		{
			SceneManager.LoadScene("Level" + _level);
		}
		else
		{
			// All levels complete
			SceneManager.LoadScene("Menu");
		}
	}
	
}
