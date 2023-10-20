using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour
    //IPointerClickHandler
{
    [SerializeField] private UnityEngine.Color baseColor, offSetColor, highLightColor, deadColor;
    [SerializeField] private SpriteRenderer render;
    public int cellPosX, cellPosY;
    //[SerializeField] private GameObject highlight;

    private bool markedDead;
    private bool markedAlive;
    public bool isAlive = false;
    [HideInInspector] public bool isCellAlive;
    public int numNeighbors = 0;
    public Texture2D cursorTexture;
    //public CursorMode cursorMode;
    public Vector2 hotSpot = Vector2.zero;
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void Init(bool isOffSet) {
    //    render.color = isOffSet ? offSetColor : baseColor;
    //}

    void OnMouseEnter()
    {
        baseColor = render.material.color;
        render.material.color = highLightColor;
        float xspot = cursorTexture.width / 2;
        float yspot = cursorTexture.height / 2;
        hotSpot = new Vector2(xspot, yspot);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
    void OnMouseExit()
    {
        render.material.color = baseColor;
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    public int getCellPosX()
    {
        return cellPosX;
    }

    public int getCellPosY()
    {
        return cellPosY;
    }
    //mark a cell as dead or alive. This will not change the state, just mark for later
    public void MarkDead()
    {
        markedDead = true;
    }

    public void MarkAlive()
    {
        markedAlive = true;
    }

    //update the state of cell. The cell will be dead or alive if it was marked previously
    public void UpdateCell()
    {
        if (markedAlive)
        {
            ActivateCell();
        }
        if (markedDead)
        {
            DeactivateCell();
        }
    }

    //theese methods will kill and revive cells
    public void ActivateCell()
    {
        markedAlive = false;
        markedDead = false;
        isCellAlive = true;

        //render.color = aliveColor; //update graphics
    }

    public void SetAlive(bool alive)
    {

        isAlive = alive;
        //Debug.Log("bool = " + isAlive);
        //render.color = isAlive ? aliveColor : deadColor;
        if(alive)
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }
        else 
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void DeactivateCell()
    {
        markedAlive = false;
        markedDead = false;
        isCellAlive = false;

       // render.color = deadColor; //update graphics
    }

    public void myDestroy(Cell argCell) {
        if(Application.isPlaying == true) {
            Destroy(argCell);
            Destroy(render);
        }
        else {
            DestroyImmediate(argCell);
            DestroyImmediate(render);
        }
    }
}
