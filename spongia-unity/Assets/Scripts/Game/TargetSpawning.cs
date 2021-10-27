using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawning : MonoBehaviour
{
    public GameObject targetPrefab;

    private Vector2[] sectors = new Vector2[]{Vector2.up+Vector2.right, Vector2.right+Vector2.down, Vector2.down+Vector2.left, Vector2.left+Vector2.up};


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = ((Vector2) Input.mousePosition) - new Vector2(Screen.width/2f, Screen.height/2f);


            Vector2 sector  = new Vector2(mousePosition.x/Mathf.Abs(mousePosition.x), mousePosition.y/Mathf.Abs(mousePosition.y));

            spawnTarget(sector);
        }
        
    }


    private void spawnTarget(Vector2 sector)
    {
        if (sector == sectors[0])
        {
            Debug.Log("Spawn 1.");
            
        }

        else if (sector == sectors[1])
        {
            Debug.Log("Spawn 2.");
        }

        else if (sector == sectors[2])
        {
            Debug.Log("Spawn 3.");
        }

        else if (sector == sectors[3])
        {
            Debug.Log("Spawn 4.");
        }
    }

    
    private Vector2 getDirecton(UnityEngine.Transform originTransform)
    {
        Vector2 target = new Vector2(0f, 0f);
        Vector2 origin = originTransform.position;

        Vector2 diff = target - origin;

        return diff;
    }
}
