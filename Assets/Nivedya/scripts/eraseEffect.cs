using UnityEngine;
using UnityEngine.UI;

public class eraseEffect : MonoBehaviour
{
    public GameObject mask;
    bool pressed;
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        if (pressed == true)
        {
            GameObject ob = Instantiate(mask, pos, Quaternion.identity);
            ob.transform.parent = GameObject.Find("scrach").transform;
        }
        if (Input.GetMouseButton(0))
        {
            pressed = true;

        }
        else if (Input.GetMouseButtonUp(0))
        {
            pressed = false;
        }
    }
}