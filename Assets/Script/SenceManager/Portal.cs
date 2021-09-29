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
    PlayerActions player;
    public void OnPlayerTriggered(PlayerActions player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }
    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(this.gameObject);

        GameController.Instance.PauseGame(true);

        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        GameController.Instance.PauseGame(false);

        Destroy(this.gameObject);
    }
}

public enum DestinationIdentifier { A, B, C, D, E }