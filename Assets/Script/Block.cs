using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Block : MonoBehaviour
{

    // Block Number
    public int number;
    // Move State
    public bool move;
    // Combine State
    public bool combine;

    // Move Target
    private Vector3 moveTarget;
    // CallBack
    private Action callBack;
    // Speed
    private float speed;

    // Use this for initialization
    void Start()
    {
        move = false;
        combine = false;
        speed = 40F;
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            if (transform.position == moveTarget)
            {
                if (combine)
                    Destroy(gameObject);
                else
                    move = false;

                if (callBack != null)
                    callBack();
            }

            float step = speed * Time.deltaTime;
            speed += 0.1F;
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, step);
        }
    }

    public void Move(Vector3 target)
    {
        move = true;
        moveTarget = target;
        speed = 40F;
    }

    public void Combine(Vector3 target, Action callback)
    {
        combine = true;
        move = true;
        moveTarget = target;
        callBack = callback;
    }

    public int combineDirection;

    public void StartCombineCorutine()
    {
        combineDirection = 1;
        StartCoroutine(CombineCorutine());
    }

    private IEnumerator CombineCorutine()
    {
        Vector3 originSize = transform.localScale;
        
        while (true)
        {
            transform.localScale = new Vector3(transform.localScale.x + (0.075F * combineDirection), transform.localScale.y + (0.075F * combineDirection), originSize.z);
            
            if (transform.localScale.x > originSize.x + 0.5F)
            {
                combineDirection = -1;
            }

            if (transform.localScale.x < originSize.x)
            {
                transform.localScale = new Vector3(originSize.x, originSize.y, originSize.z);

                break;
            }

            yield return null;
        }
    }

    private IEnumerator CreateCorutine()
    {
        Vector3 originSize = transform.localScale;
        transform.localScale = new Vector3(0.1F, 0.1F, transform.localScale.z);

        while (true)
        {
            Vector3 currentSize = transform.localScale;

            transform.localScale = new Vector3(currentSize.x + 0.2F, currentSize.y + 0.2F, currentSize.z);

            if (transform.localScale.x >= originSize.x)
            {
                transform.localScale = new Vector3(originSize.x, originSize.y, originSize.z);

                break;
            }

            yield return null;
        }
    }

    public void Create(int number, Sprite sprite)
    {
        this.number = number;

        GetComponent<SpriteRenderer>().sprite = sprite;
        StartCoroutine(CreateCorutine());
    }

    public void SetImage(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
