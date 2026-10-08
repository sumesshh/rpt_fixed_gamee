using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_Game_7_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SizeReducer() {
        
       
        
            transform.DOScale(Vector3.zero, 1f)
            .OnKill(() => gameObject.SetActive(false));
        

    }
}
