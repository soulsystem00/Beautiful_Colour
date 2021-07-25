using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.iOS;

public class PlayerActions : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    IEnumerator coroutine;
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

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //MoveFunction1();

        MoveFunction2();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Gate"))
        {
            StopCoroutine(coroutine);
            isMoving = false;
        }
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

                if(IsWalkable(targetPos))
                {
                    coroutine = Move(targetPos, tmp);
                    StartCoroutine(coroutine);
                }
                    
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

    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.3f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }
}
