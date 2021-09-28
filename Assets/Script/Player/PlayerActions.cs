using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;
    public LayerMask colour;
    IEnumerator coroutine;
    float h;
    float v;

    bool isHorizontalMove;

    float CurX;
    float CurY;

    float targetX;
    float targetY;

    private bool isMoving;
    private Vector2 input;


    Rigidbody2D rigid;
    BoxCollider2D collider2D;
    //PlayerInput PlayerInput;


    Vector3 dirVec;
    public GameObject scanObject;
    public GameManager manager;

    public GameObject camera;
    public GameObject battlesystem;

    public Animator animator;

    public event Action OnEncountered; 
    private void Awake()
    {
        //manager = FindObjectOfType<GameManager>();
        //manager.Player = gameObject;
        //DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //PlayerInput = GameObject.Find("GameController").GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        //MoveFunction1();
            //CheckInput();
        MoveFunction2();
        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
        //if (Input.GetKeyDown(KeyCode.Z) && scanObject != null && (PlayerInput.state == PlayerInput.State.Move || PlayerInput.state == PlayerInput.State.Talk))
        //{
        //    //Debug.Log("this is " + scanObject.name);
        //    manager.Action(scanObject);
        //}

    }
    private void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, dirVec * 1f, new Color(1, 0, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 1f, LayerMask.GetMask("Colour"));

        if (rayHit.collider != null)
        {
            scanObject = rayHit.collider.gameObject;
        }
        else
            scanObject = null;
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

    //void CheckInput()
    //{
    //    bool hDown = PlayerInput.hDown;
    //    bool vDown = PlayerInput.vDown;
    //    bool hUp = PlayerInput.hUp;
    //    bool vUp = PlayerInput.vUp;

    //    if (hDown || vUp)
    //        isHorizontalMove = true;
    //    else if (vDown || hUp)
    //        isHorizontalMove = false;

    //    if (Input.geta && PlayerInput.vRaw == 1)
    //        dirVec = Vector3.up;
    //    else if (vDown && PlayerInput.vRaw == -1)
    //        dirVec = Vector3.down;
    //    else if (hDown && PlayerInput.hRaw == -1)
    //        dirVec = Vector3.left;
    //    else if (hDown && PlayerInput.hRaw == 1)
    //        dirVec = Vector3.right;
    //}

    void MoveFunction2()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            //dirVec = new Vector3(input.x, input.y);

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                var tmp = targetPos;
                targetPos.x += input.x;
                targetPos.y += input.y;
                if (IsWalkable(targetPos))
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

        transform.position = targetPos;
        isMoving = false;
        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.3f, GameLayers.i.SolidObjectsLayer | GameLayers.i.InteractableLayer | GameLayers.i.Colour) != null)
        {
            return false;
        }

        return true;
    }
    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }
    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.GrassLayer) != null)
        {
            if(UnityEngine.Random.Range(1, 101) <= 10)
            {
                Debug.Log("Encounterd pokemon");
                OnEncountered();
            }
        }
    }
}
