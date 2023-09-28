using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Update() 
    {
        if(Player.instance.isEnded)
        {
            LoadNextLevel();
        }

        if(Player.instance.isDeaded)
        {
            MapLoader.instance.Reset();
            LevelManager.instance.DestroyCurrentLevel();
            LevelManager.instance.LoadLevel();
            Invoke(nameof(ReLoadLevel),0.00001f);
        }
    }

    private void ReLoadLevel()
    {
        Player.instance.Reset();
    }

    private void LoadNextLevel()
    {
        MapLoader.instance.Reset();
        LevelManager.instance.DestroyCurrentLevel();
        LevelManager.instance.LoadNextLevel();
        Player.instance.Reset();
    }
}
