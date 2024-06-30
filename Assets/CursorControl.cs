using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CusorControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.transform.position = new Vector3(mpos.x, mpos.y, 0.0f);
    }
}
