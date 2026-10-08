using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TRAIN : MonoBehaviour
{
    public AudioClip trainsound;
    public RectTransform train;
    //public GameObject SRcanvas;
    //public GameObject currentCanvas;
    public List<GameObject> gameObjects = new List<GameObject>();

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Play train sound
        PlayTrainSound();

        ActivateAllObjects();
        train_move();
    }

    
    private void train_move()
    {
        Vector3 targetPosition = new Vector3(train.transform.position.x - 500f,train.transform.position.y,train.transform.position.z);
        train.transform.DOMove(targetPosition, 4f);
    }

    void ActivateAllObjects()
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                Vector3 targetPosition = new Vector3(obj.transform.position.x -500f, obj.transform.position.y,obj.transform.position.z);
                
                obj.transform.DORotate(new Vector3(0, 0, 360), 4f, RotateMode.FastBeyond360);
                obj.transform.DOMove(targetPosition, 4f);
              
            }
        }
       
    }

    void PlayTrainSound()
    {
        if (trainsound != null && audioSource != null)
        {
            audioSource.clip = trainsound;
            audioSource.loop = false; // Set to true if you want it to loop
            audioSource.Play();
        }
    }

}
