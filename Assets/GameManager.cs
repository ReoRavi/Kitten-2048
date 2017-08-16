using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // UI Blocks Transform
    public Transform[,] blockTransform;
    // Game Blocks
    public GameObject[,] blocks;
    // Game Block Dummy
    public GameObject dummyBlock;
    // Game Box Size
    private int gameSize;

    // Use this for initialization
    void Start() {
        gameSize = 4;

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
    void Update() {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //blockManegement.ToUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //blockManegement.ToDown();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //blockManegement.ToRight();
        }
    }

    private void MoveLeft()
    {
        for (int x = 1; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                GameObject block = blocks[x, y];

                if (block == null)
                    continue;

                GameObject otherBlock = CheckLeftBlock(x, y);

                // 블럭이 없음. 맨 끝으로 이동
                if (otherBlock == null)
                {
                    block.transform.position = blockTransform[0, y].position;
                    blocks[0, y] = block;
                    blocks[x, y] = null;
                }
                // 블럭이 있음
                else
                {

                }
            }
        }
    }

    private GameObject CheckLeftBlock(int currentPos, int yPos)
    {
        for (int i = 0; i < currentPos; i++)
        {
            GameObject block = blocks[i, yPos];
            if (block != null)
                return block;
        }

        return null;
    }

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

            dummy.transform.position = new Vector3(blockTransform[randomWidth, randomHeight].position.x, blockTransform[randomWidth, randomHeight].position.y, -2);
            dummy.GetComponent<Block>().Create(2 * (count + 1));

            blocks[randomWidth, randomHeight] = dummy;

            count++;
        }
    }

    //// 급하게 옮김. 해석
    //private GameObject GetObjectAtGridPosition(Vector3 originPos)
    //{
    //    RaycastHit2D hit = Physics2D.Raycast(originPos, Vector2.right, borderSpacing);

    //    if (hit && hit.collider.gameObject.GetComponent<Block>() != null)
    //    {
    //        return hit.collider.gameObject;
    //    }
    //    else
    //    {
    //        return null;
    //    }
    //}

    //private static float borderOffset = 0.05f;
    //private static float horizontalSpacingOffset = -1.65f;
    //private static float verticalSpacingOffset = 1.65f;
    //private static float borderSpacing = 0.1f;

    //private static Vector2 GridToWorldPoint(int x, int y)
    //{
    //    return new Vector2(x + horizontalSpacingOffset + borderSpacing * x,
    //                       -y + verticalSpacingOffset - borderSpacing * y);
    //}
}
