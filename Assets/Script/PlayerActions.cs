using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.iOS;

public class PlayerActions : MonoBehaviour
{
    public float moveSpeed;

    float h;
    float v;

    float CurX;
    float CurY;

    float targetX;
    float targetY;

    private bool isMoving;
    private Vector2 input;


    Rigidbody2D rigid;
    BoxCollider2D collider2D;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveFunction1();

        //MoveFunction2();

    }

    void MoveFunction1()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        rigid.velocity = new Vector2(h * moveSpeed, v * moveSpeed);
    }

    void MoveFunction2()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                var targetPos = transform.position;
                var tmp = targetPos;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos, tmp));
            }

        }
    }

    IEnumerator Move(Vector3 targetPos, Vector3 tmp)
    {
        isMoving = true;

        while(((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon))
        {
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        isMoving = false;
    }
}
