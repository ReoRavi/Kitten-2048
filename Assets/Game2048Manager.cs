using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048Manager : MonoBehaviour
{
    // Block Images
    public Sprite[] blockImages;
    // Result Prefab
    public GameObject[] blockPrefabs;
    // UI Blocks Transform
    public Transform[,] blockTransform;
    // Game Blocks
    public GameObject[,] blocks;
    // Game Block Dummy
    public GameObject dummyBlock;
    // Game Over UI
    public GameObject gameOver;
    // Game Block Count
    public int blockCount;

    // Game Box Size
    private int gameSize;
    // Game State
    private bool play;
    // block Z Position
    private int zBlockOrder;

    // Touch Position
    private Vector2 touchStartPosition = Vector2.zero;
    // Swipe Distance
    private float minSwipeDistance = 10.0f;
    // Drag Move State
    private bool moveState;
    // Block Create State
    private bool blockCreateState;
    // Use this for initialization
    void Start()
    {
        gameSize = 4;
        play = true;
        zBlockOrder = -4;

        moveState = true;
        blockCreateState = false;

        blocks = new GameObject[gameSize, gameSize];
        blockTransform = new Transform[gameSize, gameSize];

        for (int height = 0; height < gameSize; height++)
        {
            GameObject blockLine = GameObject.Find("BlockLine" + (height + 1).ToString());

            for (int width = 0; width < gameSize; width++)
            {
                blockTransform[width, height] = blockLine.transform.Find("Block" + (width + 1).ToString());
            }
        }

        CreateStartBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (!play)
            return;

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
        if (moveState)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveUp();
                moveState = false;
                blockCreateState = true;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveDown();
                moveState = false;
                blockCreateState = true;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
                moveState = false;
                blockCreateState = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
                moveState = false;
                blockCreateState = true;
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                moveState = true;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                moveState = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                moveState = true;
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                moveState = true;
            }
        }
#endif

#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1 || UNITY_WEBGL
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPosition = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 swipeDelta = (Input.GetTouch(0).position - touchStartPosition);

                if (swipeDelta.magnitude < minSwipeDistance)
                {
                    return;
                }

                swipeDelta.Normalize();

                if (moveState)
                {
                    if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
                    {
                        MoveUp();

                        moveState = false;
                        blockCreateState = true;
                    }
                    else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
                    {
                        MoveDown();

                        moveState = false;
                        blockCreateState = true;
                    }
                    else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
                    {
                        MoveRight();

                        moveState = false;
                        blockCreateState = true;
                    }
                    else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
                    {
                        MoveLeft();

                        moveState = false;
                        blockCreateState = true;
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                moveState = true;
            }
        }
#endif

        if (blockCreateState)
        {
            if (!CheckBlockMoving())
            {
                CreateBlock();
                blockCreateState = false;
            }
        }

        if (!CheckBlockMovePossible())
        {
            gameOver.SetActive(true);
            play = false;
            CreateReward();
        }
    }

#region LEFT
    private bool MoveLeft()
    {
        bool createFlag = false;

        for (int x = 1; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                GameObject block = blocks[x, y];

                if (block == null)
                    continue;

                int otherBlockNumber = 0;
                GameObject otherBlock = CheckLeftBlock(x, y, ref otherBlockNumber);

                blocks[x, y] = null;

                // 블럭이 없음. 맨 끝으로 이동
                if (otherBlock == null)
                {
                    blocks[0, y] = block;

                    Vector3 pos = blockTransform[0, y].position;

                    block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));

                    createFlag = true;
                }
                // 블럭이 있음
                else
                {
                    // 숫자가 같을 경우
                    if (block.GetComponent<Block>().number == otherBlock.GetComponent<Block>().number)
                    {
                        CombineBlock(otherBlock, block);

                        createFlag = true;
                    }
                    // 숫자가 다를 경우
                    else
                    {
                        blocks[otherBlockNumber, y] = block;

                        Vector3 pos = blockTransform[otherBlockNumber, y].position;
                        block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                    }
                }
            }
        }

        return createFlag;
    }

    private GameObject CheckLeftBlock(int currentPos, int yPos, ref int otherBlockNumber)
    {
        for (int i = currentPos - 1; i >= 0; i--)
        {
            GameObject block = blocks[i, yPos];
            if (block != null)
            {
                otherBlockNumber = i + 1;
                return block;
            }
        }

        return null;
    }
#endregion

#region RIGHT
    private void MoveRight()
    {
        for (int x = gameSize - 2; x >= 0; x--)
        {
            for (int y = 0; y < gameSize; y++)
            {
                GameObject block = blocks[x, y];

                if (block == null)
                    continue;

                int otherBlockNumber = 0;
                GameObject otherBlock = CheckRightBlock(x, y, ref otherBlockNumber);

                blocks[x, y] = null;

                // 블럭이 없음. 맨 끝으로 이동
                if (otherBlock == null)
                {
                    blocks[gameSize - 1, y] = block;

                    Vector3 pos = blockTransform[gameSize - 1, y].position;

                    block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                }
                // 블럭이 있음
                else
                {
                    // 숫자가 같을 경우
                    if (block.GetComponent<Block>().number == otherBlock.GetComponent<Block>().number)
                    {
                        CombineBlock(otherBlock, block);
                    }
                    // 숫자가 다를 경우
                    else
                    {
                        blocks[otherBlockNumber, y] = block;

                        Vector3 pos = blockTransform[otherBlockNumber, y].position;

                        block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                    }
                }
            }
        }
    }

    private GameObject CheckRightBlock(int currentPos, int yPos, ref int otherBlockNumber)
    {
        for (int i = currentPos + 1; i < gameSize; i++)
        {
            GameObject block = blocks[i, yPos];
            if (block != null)
            {
                otherBlockNumber = i - 1;
                return block;
            }
        }

        return null;
    }
#endregion

#region UP
    private void MoveUp()
    {
        for (int y = 1; y < gameSize; y++)
        {
            for (int x = 0; x < gameSize; x++)
            {
                GameObject block = blocks[x, y];

                if (block == null)
                    continue;

                int otherBlockNumber = 0;
                GameObject otherBlock = CheckUpBlock(y, x, ref otherBlockNumber);

                blocks[x, y] = null;

                // 블럭이 없음. 맨 끝으로 이동
                if (otherBlock == null)
                {
                    blocks[x, 0] = block;

                    Vector3 pos = blockTransform[x, 0].position;

                    block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                }
                // 블럭이 있음
                else
                {
                    // 숫자가 같을 경우
                    if (block.GetComponent<Block>().number == otherBlock.GetComponent<Block>().number)
                    {
                        CombineBlock(otherBlock, block);
                    }
                    // 숫자가 다를 경우
                    else
                    {
                        blocks[x, otherBlockNumber] = block;

                        Vector3 pos = blockTransform[x, otherBlockNumber].position;

                        block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                    }
                }
            }
        }
    }

    private GameObject CheckUpBlock(int currentYPos, int xPos, ref int otherBlockNumber)
    {
        for (int i = currentYPos - 1; i >= 0; i--)
        {
            GameObject block = blocks[xPos, i];
            if (block != null)
            {
                otherBlockNumber = i + 1;
                return block;
            }
        }

        return null;
    }
#endregion

#region DOWN
    private void MoveDown()
    {
        for (int y = gameSize - 2; y >= 0; y--)
        {
            for (int x = 0; x < gameSize; x++)
            {
                GameObject block = blocks[x, y];

                if (block == null)
                    continue;

                int otherBlockNumber = 0;
                GameObject otherBlock = CheckDownBlock(y, x, ref otherBlockNumber);

                blocks[x, y] = null;

                // 블럭이 없음. 맨 끝으로 이동
                if (otherBlock == null)
                {
                    blocks[x, gameSize - 1] = block;

                    Vector3 pos = blockTransform[x, gameSize - 1].position;

                    block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                }
                // 블럭이 있음
                else
                {
                    // 숫자가 같을 경우
                    if (block.GetComponent<Block>().number == otherBlock.GetComponent<Block>().number)
                    {
                        CombineBlock(otherBlock, block);
                    }
                    // 숫자가 다를 경우
                    else
                    {
                        blocks[x, otherBlockNumber] = block;

                        Vector3 pos = blockTransform[x, otherBlockNumber].position;

                        block.GetComponent<Block>().Move(new Vector3(pos.x, pos.y, zBlockOrder));
                    }
                }
            }
        }
    }

    private GameObject CheckDownBlock(int currentYPos, int xPos, ref int otherBlockNumber)
    {
        for (int i = currentYPos + 1; i < gameSize; i++)
        {
            GameObject block = blocks[xPos, i];
            if (block != null)
            {
                otherBlockNumber = i - 1;
                return block;
            }
        }

        return null;
    }
    #endregion

    private void CreateStartBlock()
    {
        int count = 0;

        while (count < 2)
        {
            CreateBlock();

            count++;
        }
    }

    private void CreateBlock()
    {
        if (blockCount == 16)
            return;

        while (true)
        {
            int randomWidth = Random.Range(0, gameSize);
            int randomHeight = Random.Range(0, gameSize);

            if (blocks[randomWidth, randomHeight] != null)
                continue;

            GameObject dummy = Instantiate(dummyBlock);
            Vector3 pos = blockTransform[randomWidth, randomHeight].transform.position;

            int score = 2 * (Random.Range(0, 2) + 1);

            dummy.transform.position = new Vector3(pos.x, pos.y, -3);
            dummy.GetComponent<Block>().Create(score, blockImages[(score / 2) - 1]);

            blocks[randomWidth, randomHeight] = dummy;

            break;
        }

        blockCount++;
    }

    private void CreateReward()
    {
        int count = 0;

        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                //StartCoroutine(RotateBlock(blocks[x, y]));
                

                int power = GetBlockPower(blocks[x, y].GetComponent<Block>().number);

                GameObject obj = Instantiate(blockPrefabs[power], new Vector3(-0.8F + ((count / 4) * 0.6F), 3.5F - ((count % 4) * 0.6F), -4), Quaternion.identity);

                count++;
            }
        }
    }

    IEnumerator RotateBlock(GameObject block)
    {
        while (true)
        {
            float yRotation = transform.eulerAngles.y + 10F;

            if (yRotation >= 350)
                break;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, yRotation, transform.eulerAngles.z);

            yield return null;
        }
    }

    private void CombineBlock(GameObject combineBlock, GameObject block)
    {
        Block combineObject = combineBlock.GetComponent<Block>();
        int score = combineObject.number * 2;
        combineObject.number = score;

        block.GetComponent<Block>().Combine(combineBlock.transform.position, () =>
        {
            int imagePower = GetBlockPower(score);

            combineBlock.GetComponent<Block>().SetImage(blockImages[imagePower]);
            combineObject.StartCombineCorutine();

            blockCount--;
        });
    }

    private bool CheckBlockMoving()
    {
        foreach (GameObject block in blocks)
        {
            if (block == null)
                continue;

            Block b = block.GetComponent<Block>();

            if (b.move)
                return true;
        }

        return false;
    }

    private bool CheckBlockMovePossible()
    {
        if (blockCount < gameSize * gameSize)
        {
            return true;
        }

        for (int x = 0; x < gameSize - 1; x++)
        {
            for (int y = 0; y < gameSize - 1; y++)
            {
                GameObject currentBlock = blocks[x, y];
                GameObject rightBlock = blocks[x + 1, y];
                GameObject downBlock = blocks[x, y + 1];

                if (currentBlock == null)
                    return true;

                if (rightBlock == null)
                    return true;

                if (x != gameSize - 1 && currentBlock.GetComponent<Block>().number == rightBlock.GetComponent<Block>().number)
                {
                    return true;
                }

                if (downBlock == null)
                    return true;

                if (y != gameSize - 1 && currentBlock.GetComponent<Block>().number == downBlock.GetComponent<Block>().number)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int GetBlockPower(int score)
    {
        int powerValue = 2;
        int count = 0;

        while (score != powerValue)
        {
            powerValue *= 2;

            count++;
        }

        return count;
    }
}
