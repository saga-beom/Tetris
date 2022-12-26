using TMPro;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    //define variable
    public Vector3 RotationPoint;
    public static int height = 20;
    public static int width = 10;
    public float pivotTime = 0.1f;
    public float delayTime = 0.2f;
    public GameObject[] Tetris;
    public GameObject gameoverPanel;
    private Vector3 spawnLocation = new(5.5f, 17.5f, 0);
    private GameObject currentBlock;
    public static int gameScore = 0;
  

    private Transform[,] occupied = new Transform[width, height];
    public TMP_Text text;

    void Start()
    {
        currentBlock = Instantiate(Tetris[Random.Range(0, Tetris.Length)], spawnLocation, Quaternion.identity);
        gameoverPanel.SetActive(false);
        text.text = gameScore.ToString();
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }

        // if currentBlock is "SquareBlock" we don't need to rotate
        if (Input.GetKeyDown(KeyCode.Space) &&
            !currentBlock.CompareTag("SquareBlock"))
        {
            Rotate();
        }

        // make delay for slow down
        if (pivotTime < delayTime)
        {
            GravityDown();
            pivotTime += 0.2f;
        }
        else
        {
            delayTime += Time.deltaTime;
        }


    }

     
    void MoveLeft()
    {
        if (LeftVaildMove())
        {
            currentBlock.transform.position -= new Vector3(1, 0, 0);
        }
    }

    void MoveRight()
    {
        if (RightVaildMove())
        {

            currentBlock.transform.position += new Vector3(1, 0, 0);

        }
    }

    void MoveDown()
    {
        if (VerticalVaildMove())
        {
            currentBlock.transform.position -= new Vector3(0, 1, 0);
        }
    }

    void Rotate()
    {
        currentBlock.transform.RotateAround(currentBlock.transform.TransformPoint(RotationPoint),
                new Vector3(0, 0, 1), 90);
        if (!VaildMove())
        {
            currentBlock.transform.RotateAround(currentBlock.transform.TransformPoint(RotationPoint),
                new Vector3(0, 0, 1), -90);
        }
    }

    // block move down without pressing a key 
    void GravityDown()
    {
        if (VerticalVaildMove())
        {
            currentBlock.transform.position -= new Vector3(0, 1, 0);
        }
        else
        {
            AddOccupy();
            foreach (Transform Children in currentBlock.transform)
            {
                if (CheckLine(Mathf.FloorToInt(Children.position.y)))
                {
                    DeleteLine(Mathf.FloorToInt(Children.position.y));
                    RowDown(Mathf.FloorToInt(Children.position.y));
                }
            }
            if (JudgeGameOver())
            {
                Debug.Log("Game over");
            }
            else
            {
                currentBlock = Instantiate(Tetris[Random.Range(0, Tetris.Length)], spawnLocation, Quaternion.identity);
            }

        }


    }

    // check a block is located in background
    bool VaildMove()
    {
        foreach (Transform Children in currentBlock.transform)
        {
            int locationX = Mathf.FloorToInt(Children.transform.position.x);
            int locationY = Mathf.FloorToInt(Children.transform.position.y);

            if (locationX < 0 || locationX >= width || locationY < 0 || locationY >= height)
            {
                return false;
            }

            if (occupied[locationX, locationY] != null)
            {
                return false;
            }

        }
        return true;
    }

    // check a block can move vertical
    bool VerticalVaildMove()
    {
        foreach (Transform Children in currentBlock.transform)
        {
            int locationX = Mathf.FloorToInt(Children.transform.position.x);
            int locationY = Mathf.FloorToInt(Children.transform.position.y - 1);

            if (locationY < 0 || locationY >= height)
            {
                return false;
            }

            if (occupied[locationX, locationY] != null)
            {
                return false;
            }

        }
        return true;
    }

    // check a block can move left
    bool LeftVaildMove()
    {
        foreach (Transform Children in currentBlock.transform)
        {
            int leftLocationX = Mathf.FloorToInt(Children.transform.position.x - 1);
            int locationY = Mathf.FloorToInt(Children.transform.position.y);

            if (leftLocationX < 0)
            {
                return false;
            }

            if (occupied[leftLocationX, locationY] != null)
            {
                return false;
            }
        }

        return true;
    }

    // check a block can move right
    bool RightVaildMove()
    {
        foreach (Transform Children in currentBlock.transform)
        {
            int rightLocationX = Mathf.FloorToInt(Children.transform.position.x + 1);
            int locationY = Mathf.FloorToInt(Children.transform.position.y);

            if (rightLocationX >= width)
            {
                return false;
            }

            if (occupied[rightLocationX, locationY] != null)
            {
                return false;
            }
        }

        return true;
    }


    // to indicate occupied space in background
    void AddOccupy()
    {
        int locationX;
        int locationY;
        foreach (Transform Children in currentBlock.transform)
        {
            locationX = Mathf.FloorToInt(Children.position.x);
            locationY = Mathf.FloorToInt(Children.position.y);

            occupied[locationX, locationY] = Children;

        }
    }

    // check the target line has "null" value
    bool CheckLine(int targetLine)
    {

        for (int i = 0; i < width; i++)
        {
            if (occupied[i, targetLine] == null)
            {
                return false;
            }
        }


        return true;
    }

    // delete the target line's game object 
    void DeleteLine(int targetLine)
    {
        for (int i = 0; i < width; i++)
        {
            gameScore += 100;
            text.text = gameScore.ToString();
            Destroy(occupied[i, targetLine].gameObject);
            occupied[i, targetLine] = null;
        }
    }

    // the blocks located in over deleted blocks move down
    void RowDown(int targetLine)
    {
        for (int i = targetLine; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (occupied[j, i] != null)
                {
                    occupied[j, i - 1] = occupied[j, i];
                    occupied[j, i] = null;
                    occupied[j, i - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    // if height is higher than 18, game over
    bool JudgeGameOver()
    {
        foreach (Transform Children in currentBlock.transform)
        {
            if (Mathf.FloorToInt(Children.position.y) >= 18)
            {
                gameoverPanel.SetActive(true);
                this.enabled = false;
                return true;
            }
        }

        return false;
    }


}
