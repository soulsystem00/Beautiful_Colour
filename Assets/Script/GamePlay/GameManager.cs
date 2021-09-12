using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject MenuCanvas;
    public GameObject Gamecontroller;

    public GameObject TextUI;
    public Text talkText;
    public GameObject scanObject;
    public GameObject Player;
    public PlayerInput playerInput;
    public TalkManager talkManager;

    public GameObject additional;
    Colour colour;
    public int talkindex;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(MenuCanvas);
        DontDestroyOnLoad(Gamecontroller);
        DontDestroyOnLoad(TextUI);
        DontDestroyOnLoad(talkManager);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Action(GameObject scanObj)
    {
        //if(playerInput.state == PlayerInput.State.Talk)
        //{
        //    playerInput.state = PlayerInput.State.Move;
        //    TextUI.SetActive(false);
        //}
        //else
        //{
        //    playerInput.state = PlayerInput.State.Talk;
        //    TextUI.SetActive(true);
        //    scanObject = scanObj;
        //    ObjData objData = scanObject.GetComponent<ObjData>();
        //    Talk(objData.id, objData.isNpc);
        //    //talkText.text = "이것의 이름은 " + scanObject.name + "이라고 한다.";
        //}
        TextUI.SetActive(true);
        scanObject = scanObj;
        colour = scanObject.GetComponent<Colour>();
        Talk(colour.id, colour.isNpc, colour);
        

    }

    void Talk(int id, bool isNpc,Colour colour)
    {
        string talkData = talkManager.GetTalk(id, talkindex);
        if(talkData == null)
        {
            talkindex = 0;
            //TextUI.SetActive(false);
            playerInput.state = PlayerInput.State.Move;

            if(colour.Character != Colour.character.NPC)
            {
                additional.SetActive(true);
                additional.GetComponent<AdditionalMenu>().colour = colour;
                additional.GetComponent<AdditionalMenu>().player = Player;
            }
            else
            {
                TextUI.SetActive(false);
            }
            //colour.teleport(Player);
            return;
        }

        if(isNpc)
        {
            talkText.text = talkData;
        }
        else
        {
            talkText.text = talkData;
        }
        playerInput.state = PlayerInput.State.Talk;
        talkindex++;
    }
}
