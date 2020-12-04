using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] float backgroundScrollSpeed = 0.02f;

    //the material from the texture
    Material myMaterial;

    //the movement
    Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        //get the material of the background
        myMaterial = GetComponent<Renderer>().material;
        //scroll in the y-axis
        offset = new Vector2(0f, backgroundScrollSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        //move the material offset every frame
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
    }
}
