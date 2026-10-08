using UnityEngine;
using System.Collections;

public class lockUnlock : MonoBehaviour
{
    public GameObject unlock;
    public GameObject locked;
    public GameObject currectNumber;
    public float timedelay=.5f;
    public GameObject keyNumber;

    
     
    public void activateLock()
    {
        StartCoroutine(DelayedAction());
        currectNumber.SetActive(true);
        keyNumber.SetActive(false);

    }

    IEnumerator DelayedAction()
    {
        yield return new WaitForSeconds(timedelay); 
        unlock.SetActive(true);
        
        locked.SetActive(false);
    }
}
