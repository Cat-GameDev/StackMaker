using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class MapLoader : MonoBehaviour
{
    //public List<string> fileNames = new List<string>();
    [SerializeField] private List<GameObject> brickPrefabs = new List<GameObject>();
    private List<Vector2Int> walkableBrickList = new List<Vector2Int>(); // Danh sách các vị trí có thể di chuyển
    private Vector3 startPoint;
    private Vector3 finishPoint;
    public static MapLoader instance;

    public Vector3 StartPoint { get => startPoint;}
    public Vector3 FinishPoint { get => finishPoint;}

    private void Awake() 
    {
        instance = this;
        //LoadMapFromFile(fileNames[0]);
    }


    public void LoadMapFromFile(string fileName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');

            int numRows = lines.Length;

            for (int row = 0; row < numRows; row++)
            {
                string line = lines[row];
                int numColumns = line.Length;

                for (int col = 0; col < numColumns; col++)
                {
                    char cellChar = line[col];
                    Vector3 spawnPosition = ConvertToVector3(col, row);
                    if(cellChar == '0') // Tranwg
                    {
                        Instantiate(brickPrefabs[0], spawnPosition, Quaternion.identity, transform);
                    }
                    else if (cellChar == '1') // vang
                    {
                        Instantiate(brickPrefabs[1], spawnPosition, Quaternion.identity, transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    }
                    else if (cellChar == '2') // di qua
                    {
                        Instantiate(brickPrefabs[2], spawnPosition+ new Vector3(0f,2.5f,0f), Quaternion.Euler(-90f, 0f, 0f), transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    } 
                    else if(cellChar == '9') // end
                    {
                        Instantiate(brickPrefabs[3], spawnPosition ,Quaternion.Euler(0f, 90f, 0f), transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                        finishPoint = spawnPosition;
                    }
                    else if(cellChar == '4')
                    {
                        Instantiate(brickPrefabs[4], spawnPosition + new Vector3(0f,2.5f,0f) ,Quaternion.Euler(-90f, 0f, 90f), transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    }
                    else if(cellChar == '7')
                    {
                        Instantiate(brickPrefabs[1], spawnPosition, Quaternion.identity, transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                        startPoint = spawnPosition;
                    }
                    else if(cellChar == '3')
                    {
                        Instantiate(brickPrefabs[5], spawnPosition ,Quaternion.Euler(0f, 90f, 0f), transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    }
                    
                    else if(cellChar == '6')
                    {
                        Instantiate(brickPrefabs[8], spawnPosition ,Quaternion.Euler(0f, 180f, 0f), transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    }

                    else if(cellChar == '5')
                    {
                        Instantiate(brickPrefabs[6], spawnPosition ,Quaternion.identity, transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    }

                    else if(cellChar == '8')
                    {
                        Instantiate(brickPrefabs[7], spawnPosition ,Quaternion.Euler(0f, 270f, 0f), transform);
                        walkableBrickList.Add(new Vector2Int(col, row)); 
                    }
                    

                }
            }
        }
        // else
        // {
        //     Debug.LogError("Không tìm thấy tệp văn bản: " + fileName);
        // }
    }

    public Vector3 ConvertToVector3(int col, int row)
    {
        return new Vector3(col , 0, row );
    }

    public bool CanMoveToCell(Vector3 position, SwipeDirection swipeDirection)
    {
        
        Vector2Int currentCell = new Vector2Int(Mathf.RoundToInt(position.x ), Mathf.RoundToInt(position.z));
        Vector2Int targetCell = currentCell;

        
        switch (swipeDirection)
        {
            case SwipeDirection.Up:
                targetCell += Vector2Int.up;
                break;

            case SwipeDirection.Down:
                targetCell += Vector2Int.down;
                break;

            case SwipeDirection.Left:
                targetCell += Vector2Int.left;
                break;

            case SwipeDirection.Right:
                targetCell += Vector2Int.right;
                break;

            case SwipeDirection.None:
                return false;
        }
        return walkableBrickList.Contains(targetCell);
    }

    public void Reset()
    {
        walkableBrickList.Clear();
    }

}