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

    // Touch Position
    private Vector2 touchStartPosition = Vector2.zero;
    // Swipe Distance
    private float minSwipeDistance = 10.0f;
    
    // Use this for initialization
    void Start()
    {
        gameSize = 4;
        play = true;

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

        bool createFlag = false;

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();

            createFlag = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();

            createFlag = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();

            createFlag = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();

            createFlag = true;
        }
#endif

#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1 || UNITY_WEBGL
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPosition = Input.GetTouch(0).position;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Vector2 swipeDelta = (Input.GetTouch(0).position - touchStartPosition);

                if (swipeDelta.magnitude < minSwipeDistance)
                {
                    return;
                }

                swipeDelta.Normalize();

                if (swipeDelta.y > 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
                {
                    MoveUp();

                    createFlag = true;
                }
                else if (swipeDelta.y < 0.0f && swipeDelta.x > -0.5f && swipeDelta.x < 0.5f)
                {
                    MoveDown();

                    createFlag = true;
                }
                else if (swipeDelta.x > 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
                {
                    MoveRight();

                    createFlag = true;
                }
                else if (swipeDelta.x < 0.0f && swipeDelta.y > -0.5f && swipeDelta.y < 0.5f)
                {
                    MoveLeft();

                    createFlag = true;
                }
            }
        }


#endif

        if (!CheckBlockMoveLeft())
        {
            gameOver.SetActive(true);

            CreateReward();

            play = false;
        }

        if (createFlag)
            CreateBlock();
    }

#region LEFT
    private void MoveLeft()
    {
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

                    block.transform.position = new Vector3(pos.x, pos.y, -2);
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

                        block.transform.position = new Vector3(pos.x, pos.y, -2);
                    }
                }
            }
        }
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

                    block.transform.position = new Vector3(pos.x, pos.y, -2);
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

                        block.transform.position = new Vector3(pos.x, pos.y, -2);
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

                    block.transform.position = new Vector3(pos.x, pos.y, -2);
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

                        block.transform.position = new Vector3(pos.x, pos.y, -2);
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

                    block.transform.position = new Vector3(pos.x, pos.y, -2);
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

                        block.transform.position = new Vector3(pos.x, pos.y, -2);
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
            int randomWidth = Random.Range(0, gameSize);
            int randomHeight = Random.Range(0, gameSize);

            if (blocks[randomWidth, randomHeight] != null)
                continue;

            GameObject dummy = Instantiate(dummyBlock);
            Vector3 pos = blockTransform[randomWidth, randomHeight].transform.position;

            int score = 2 * (count + 1);

            dummy.transform.position = new Vector3(pos.x, pos.y, -2);
            dummy.GetComponent<Block>().Create(score, blockImages[(score / 2) - 1]);

            blocks[randomWidth, randomHeight] = dummy;

            count++;
            blockCount++;
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

            dummy.transform.position = new Vector3(pos.x, pos.y, -2);
            dummy.GetComponent<Block>().Create(score, blockImages[(score / 2) - 1]);

            blocks[randomWidth, randomHeight] = dummy;

            break;
        }

        blockCount++;
    }

    private void CombineBlock(GameObject combineBlock, GameObject block)
    {
        int score = combineBlock.GetComponent<Block>().number * 2;
        combineBlock.GetComponent<Block>().number = score;

        Destroy(block);

        // 거듭제곱 구하기
        int imagePower = GetBlockPower(score);

        combineBlock.GetComponent<Block>().SetImage(blockImages[imagePower]);

        blockCount--;
    }

    private void CreateReward()
    {
        int count = 0;

        foreach (GameObject block in blocks)
        {
            // 거듭제곱 구하기
            int power = GetBlockPower(block.GetComponent<Block>().number);

            Instantiate(blockPrefabs[power], new Vector3(-2.5F + ((count / 3) * 1F), 2F + (count % 3), -4), Quaternion.identity);

            count++;
        }
    }

    private bool CheckBlockMoveLeft()
    {
        if (blockCount < gameSize * gameSize)
        {
            return true;
        }

        for (int x = 0; x < gameSize - 1; x++)
        {
            for (int y = 0; y < gameSize - 1; y++)
            {
                Block currentBlock = blocks[x, y].GetComponent<Block>();
                Block rightBlock = blocks[x + 1, y].GetComponent<Block>();
                Block downBlock = blocks[x, y + 1].GetComponent<Block>();

                if (x != gameSize - 1 && currentBlock.number == rightBlock.number)
                {
                    return true;
                }
                else if (y != gameSize - 1 && currentBlock.number == downBlock.number)
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
