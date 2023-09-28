using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<string> levels = new List<string>();
    private int currentLevel = 0;
  
    public static LevelManager instance;

    private void Awake() 
    {
        instance = this;
        LoadLevel();
    }

    public void LoadLevel()
    {
        MapLoader.instance.LoadMapFromFile(levels[currentLevel]);
    }


    public void LoadNextLevel()
    {
        currentLevel++;
        if (currentLevel < levels.Count)
        {
            LoadLevel();
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }

    public void DestroyCurrentLevel()
    {
        Transform mapLoaderTransform = MapLoader.instance.transform;
        int childCount = mapLoaderTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = mapLoaderTransform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

}