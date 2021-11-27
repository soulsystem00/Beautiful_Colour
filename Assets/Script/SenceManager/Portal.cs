using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;

    public Transform SpawnPoint => spawnPoint;

    public int SceneToLoad { get => sceneToLoad; set => sceneToLoad = value; }

    PlayerActions player;
    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    public void OnPlayerTriggered(PlayerActions player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }
    public void OnPlayerTriggered(PlayerActions player, Vector3 pos)
    {
        this.player = player;
        StartCoroutine(SwitchScene(pos));
    }
    IEnumerator SwitchScene()
    {
        UnloadScene();
        DontDestroyOnLoad(this.gameObject);
        GameController.Instance.PauseGame(true);
        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(SceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);
        yield return fader.FadeOut(0.5f);
        destPortal.loadScene();
        GameController.Instance.PauseGame(false);

        Destroy(this.gameObject);
    }
    IEnumerator SwitchScene(Vector3 pos)
    {
        UnloadScene();
        DontDestroyOnLoad(this.gameObject);
        GameController.Instance.PauseGame2(true);
        yield return fader.FadeIn(0.5f);
        yield return SceneManager.LoadSceneAsync(SceneToLoad);

        var destPortal = FindObjectOfType<Portal>();
        player.Character.SetPositionAndSnapToTile(pos);
        yield return fader.FadeOut(0.5f);
        destPortal.loadScene();
        GameController.Instance.PauseGame2(false);

        Destroy(this.gameObject);
    }
    public void UnloadScene()
    {
        SavingSystem.i.CaptureEntityStates(GetSavableEntities());
    }
    public void loadScene()
    {
        SavingSystem.i.RestoreEntityStates(GetSavableEntities());
    }
    List<SavableEntity> GetSavableEntities()
    {
        var curscene = SceneManager.GetSceneByName(gameObject.scene.name);
        var savableEntities = FindObjectsOfType<SavableEntity>().Where(x => x.gameObject.scene == curscene).ToList();
        return savableEntities;
    }
}
public enum DestinationIdentifier { A, B, C, D, E }