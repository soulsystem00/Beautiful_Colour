using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.iOS;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour, ISavable
{
    [SerializeField] string name;
    private Vector2 input;
    public GameObject scanObject;
    public GameManager manager;
    public GameObject battlesystem;
    private Character character;
    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;
    public Character Character { get => character; }
    public string Name { get => name; }

    void Start()
    {
        character = GetComponent<Character>();
    }
 
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();

    }
    void OnMoveOver()
    {
        CheckForTriggerableLayer();
        CheckIfInTrainersView();
    }
    void CheckForTriggerableLayer()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }
    void CheckIfInTrainersView()
    {
        var collider = Physics2D.OverlapCircle(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.FovLayer);

        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
    }
    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer | GameLayers.i.Colour | GameLayers.i.ColourDiable | GameLayers.i.FovLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    public object CaptureState()
    {
        
        // 위치, 유닛 저장
        var saveData = new PlayerSaveData()
        {
            position = new float[] { transform.position.x, transform.position.y, transform.position.z },
            units = GetComponent<UnitParty>().Units.Select(x => x.GetSaveData()).ToList(),
            sceneNum = FindObjectOfType<Portal>().gameObject.scene.buildIndex
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData)state;

        var position = saveData.position;
        Vector3 pos = new Vector3(position[0], position[1], position[2]);
        // 씬 복구
        var tmpPortal = FindObjectOfType<Portal>();
        tmpPortal.SceneToLoad = saveData.sceneNum;
        tmpPortal.OnPlayerTriggered(this, pos);

        
        // 위치 복구
        //transform.position = new Vector3(position[0], position[1], position[2]);

        // 유닛 복구
        GetComponent<UnitParty>().Units = saveData.units.Select(x => new Unit(x)).ToList();
    }
}
[Serializable]
public class PlayerSaveData
{
    public float[] position;
    public List<UnitSaveData> units;
    public int sceneNum;
}