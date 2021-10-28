using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Object: " + this.GetComponent<GameObject>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col) {
        Debug.Log(col);
    }

    void OnCollisionStay2D(Collision2D col) {
        Debug.Log(col);
    }

    void OnCollisionExit2D(Collision2D col) {
        Debug.Log(col);
    }
}
