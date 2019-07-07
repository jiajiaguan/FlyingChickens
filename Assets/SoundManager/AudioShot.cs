using UnityEngine;
using System.Collections;

public class AudioShot : MonoBehaviour
{
    public AudioSource audioSource;
    public string audioName;

    private void Update()
    {
        if(audioSource != null && !audioSource.isPlaying) {
            SoundController.instance.ClearAudioSource();
            Destroy(gameObject);
        }
    }
}
public class SpawnPool { 


    public GameObject Spawn(string prefabPath,Transform parent) {
        var gameobject = GameObject.Instantiate(Resources.Load<GameObject>(prefabPath),parent);
        //gameobject.
        return gameobject;
    }
}