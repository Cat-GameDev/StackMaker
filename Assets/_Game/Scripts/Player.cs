using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipeDirection
{
    None,
    Up,
    Down,
    Right,
    Left
}

public class Player : MonoBehaviour
{
    private List<GameObject> collectedBricks = new List<GameObject>();
    private List<GameObject> bricksToRemove = new List<GameObject>();
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private float moveSpeed = 7f;
    private Vector2 touchStartPos;
    private bool isMoving;
    private bool isLifted;
    private bool isDowned;
    public bool isEnded;
    public bool isDeaded;
    private bool pushPlayer;

    private Vector3 startPoint,finishPoint;
    private Vector3 savePoint = new Vector3(0f,0f,0f);

    [SerializeField] private Transform holder;
    [SerializeField] private Transform anim;
    private int brickSpawnCount;
    public static Player instance;

    SwipeDirection pushDirection;

    public bool IsLifted { get => isLifted; set => isLifted = value; }
    public bool IsDowned { get => isDowned; set => isDowned = value; }

    private void Awake() 
    {
        instance = this;
        OnInit();
    }
    
    private void Start() 
    {
        startPoint = MapLoader.instance.StartPoint;
        finishPoint = MapLoader.instance.FinishPoint;
        transform.position = startPoint + new Vector3(0f,2.5f,0f);
        savePoint = transform.position;
    }

    void Update()
    {
        if (isMoving)
            return;
        
        if(isEnded)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            StartSwipe();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndSwipe();
        }

        if(transform.position.x == finishPoint.x)   
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(4f,0f,0f), 3f);
        }

          
        if(pushPlayer)
        {
            EndSwipe();
        }
    }

    private void OnInit()
    {
        isDeaded = isDowned = isEnded = isLifted = isMoving = pushPlayer = false;
        brickSpawnCount = 1;
        transform.position = savePoint;
    }

    private void StartSwipe()
    {
        touchStartPos = Input.mousePosition;
    }

    private void EndSwipe()
    {
        
        if(pushPlayer)
        {
            if (MapLoader.instance.CanMoveToCell(transform.position, pushDirection))
            {
                List<Vector3> path = new List<Vector3>();
                Vector3 currentTarget = transform.position;
                while (MapLoader.instance.CanMoveToCell(currentTarget, pushDirection))
                {
                    SetTargetPosition(pushDirection, ref currentTarget);
                    path.Add(currentTarget);
                }
                StartCoroutine(MovePlayerAlongPath(path));
            }  

        } 
        else
        {
            Vector2 swipeDirection = (Vector2)Input.mousePosition - touchStartPos;
            SwipeDirection direction = GetSwipeDirection(swipeDirection);
            pushDirection = direction;
            if (MapLoader.instance.CanMoveToCell(transform.position, direction))
            {
                List<Vector3> path = new List<Vector3>();
                Vector3 currentTarget = transform.position;
                while (MapLoader.instance.CanMoveToCell(currentTarget, direction))
                {
                    SetTargetPosition(direction, ref currentTarget);
                    path.Add(currentTarget);
                }
                StartCoroutine(MovePlayerAlongPath(path));
            }  
        }
        pushPlayer = false;
    }

    private IEnumerator MovePlayerAlongPath(List<Vector3> path)
    {
        isMoving = true;

        foreach (Vector3 target in path)
        {
            while (transform.position != target)
            {
                if (isDeaded)
                {
                    yield break; 
                }
                MovePlayer(target);
                yield return null;
            }
        }
        
        isMoving = false;
        path.Clear();
    }

    private void MovePlayer(Vector3 target)
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    private SwipeDirection GetSwipeDirection(Vector2 swipeVector)
    {
        float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;

        if (swipeAngle < 0f)
            swipeAngle += 360f;

        float rightThreshold = 45f;
        float upThreshold = 135f;
        float leftThreshold = 225f;
        float downThreshold = 315f;

        if (swipeAngle < rightThreshold || swipeAngle >= downThreshold)
        {
            return SwipeDirection.Down; // Left
        }
        else if (swipeAngle >= rightThreshold && swipeAngle < upThreshold)
        {
            return SwipeDirection.Right; // Up
        }
        else if (swipeAngle >= upThreshold && swipeAngle < leftThreshold)
        {
            return SwipeDirection.Up; // Right
        }
        else if (swipeAngle >= leftThreshold && swipeAngle < downThreshold)
        {
            return SwipeDirection.Left; //Down
        }

        return SwipeDirection.None;
    }

    private void SetTargetPosition(SwipeDirection direction, ref Vector3 target)
    {
        switch (direction)
        {
            case SwipeDirection.Up:
                target += Vector3.forward;
                break;

            case SwipeDirection.Down:
                target += Vector3.back;
                break;

            case SwipeDirection.Left:
                target += Vector3.left;
                break;

            case SwipeDirection.Right:
                target += Vector3.right;
                break;

            case SwipeDirection.None:
                break;
        }
 
    }

    private void OnTriggerEnter(Collider collider) 
    {
        if (collider.tag == "CollectibleBrick")
        {
            AddStack(collider.gameObject);
        }

        if (collider.tag == "PassThroughBrick")
        {
            RemoveStack();
            collider.gameObject.tag = "Untagged";
            
        } 

        if(collider.tag == "FinishPoint")
        {
            isEnded = true;
            Debug.Log("end");
            ClearBrick();
        }

        if(collider.tag == "BrickPush")
        {
            pushPlayer = true;
            
            if(pushDirection == SwipeDirection.Right)
            {
                pushDirection = SwipeDirection.Up;
            }
            else if(pushDirection == SwipeDirection.Down)
            {
                pushDirection = SwipeDirection.Left;
            }

        }

        if(collider.tag == "BrickPush2")
        {
            pushPlayer = true;
            if(pushDirection == SwipeDirection.Right)
            {
                pushDirection = SwipeDirection.Down;
            } 
            else if(pushDirection == SwipeDirection.Up)
            {
                pushDirection = SwipeDirection.Left;
            } 
        }

        if(collider.tag == "BrickPush3")
        {
            pushPlayer = true;
            if(pushDirection == SwipeDirection.Left)
            {
                pushDirection = SwipeDirection.Down;
            } 
            else if(pushDirection == SwipeDirection.Up)
            {
                pushDirection = SwipeDirection.Right;
            } 
        }

        if(collider.tag == "BrickPush4")
        {
            pushPlayer = true;
            if(pushDirection == SwipeDirection.Down)
            {
                pushDirection = SwipeDirection.Right;
            } 
            else if(pushDirection == SwipeDirection.Left)
            {
                pushDirection = SwipeDirection.Up;
            } 
        }


    }

    private void AddStack(GameObject brick)
    {
        isLifted = true;
        collectedBricks.Add(brick);
        BrickSpawn();
        brick.SetActive(false);
    }

    private void RemoveStack()
    {
        if(isEnded) return;
        isDowned = true;
        if (collectedBricks.Count > 0)
        {
            BricksToRemoveInScence();
            GameObject brickToRemove = collectedBricks[0];
            collectedBricks.RemoveAt(0);
            Destroy(brickToRemove);
            brickSpawnCount = collectedBricks.Count;
            if(collectedBricks.Count == 0)
            {
                ClearBrick();
                isDeaded = true;
                Debug.Log("Không còn viên gạch để mất.");
            }
        }
        // else
        // {
        //     Debug.Log("Không còn viên gạch để mất.");
        // }
    }

    private void BrickSpawn()
    {
        if(collectedBricks.Count == 1)
        {
            Vector3 spawnPos = transform.position;
            Quaternion rotation = Quaternion.Euler(-90f,0f,-180f);
            GameObject newBrick = Instantiate(brickPrefab, spawnPos, rotation);
            newBrick.transform.SetParent(holder);
            brickSpawnCount = 0;
            bricksToRemove.Add(newBrick);
        } 
        else 
        {
            Vector3 spawnPos = transform.position + new  Vector3(0f,.3f * brickSpawnCount,0f);
            Quaternion rotation = Quaternion.Euler(-90f,0f,-180f);
            GameObject newBrick = Instantiate(brickPrefab, spawnPos, rotation);
            newBrick.transform.SetParent(holder);
            anim.position = anim.position + new Vector3(0f,0.3f,0f);
            bricksToRemove.Add(newBrick);
        }

        brickSpawnCount = collectedBricks.Count;
    }


    private void BricksToRemoveInScence()
    {
        if(bricksToRemove.Count > 0)
        {
            int lastIndex = bricksToRemove.Count - 1;
            GameObject brickToRemove = bricksToRemove[lastIndex];
            bricksToRemove.RemoveAt(lastIndex);
            Destroy(brickToRemove);
            anim.position = anim.position + new Vector3(0f,-0.3f,0f);
        }
    }

    private void ClearBrick()
    {
        if(bricksToRemove.Count > 0)
        {
            for (int i =0; i< bricksToRemove.Count; i++)
            {
                Destroy(bricksToRemove[i]);
                anim.position = anim.position + new Vector3(0f,-0.3f,0f);
            }
            bricksToRemove.Clear();
        }

        if (collectedBricks.Count > 0)
        {
            collectedBricks.Clear();
        }
    }

    public void Reset()
    {
        OnInit();
        Start();
        anim.position = anim.position + new Vector3(0f,0.3f,0f);
        //ClearBrick();
    }


 
}
