using UnityEngine;

public class Watermelon : MonoBehaviour
{

    private void Awake()
    {
        //Collider collider = GetComponent<Collider>();
        //collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
       
        Debug.Log("collision");
        print("collision");
        Destroy(gameObject, 1.0f);
    }



}