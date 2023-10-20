using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System.Xml.Serialization;
public class GridManager : MonoBehaviour
{
    //[Range(0, 100)]
    //[HideInInspector] public float width, height;
    //private float currentWidth;
    //private float currentHeight;
    public SaveLoadManager saveLoadManager;
    [HideInInspector] public int VerticalCamSize;
    [HideInInspector] public int HorizontalCamSize;
    [HideInInspector] public int AspectRatio;

    [HideInInspector] public int Columns, Rows;
    public Animator crossFade;
    public int currentCols;
    public int currentRows;

    public float speed = 10f;
    private float timer = 0;
    public bool simulationEnabled = false;
    //private int currentTotalCells;
    //[SerializeField] public int TotalCells;
    //[SerializeField] public int maxCells;

    [SerializeField] private Cell cellPrefab;
    [SerializeField] public Transform cam;

    private Cell[,] cells;
    //Cell[,] cells = new Cell[currentCols, currentRows];
    //[Range(-100, 20)]
    //[SerializeField] public float camZoom;
    //[HideInInspector] public float camSize;
    //private float currentCamSize;

    [HideInInspector] public float cellSize;
    private float currentCellSize;
    //[HideInInspector] public Vector3 OrigCellSize;

    private Dictionary<Vector2, Cell> _cells;
    public List<Cell> generatedObjects = new List<Cell>();

    public GridManager() {
    }

    // Start is called before the first frame update
    void Start() {
        EventManager.StartListening("SavePattern", SavePattern);
        EventManager.StartListening("LoadPattern", LoadPattern);
        VerticalCamSize = (int)Camera.main.orthographicSize;
        HorizontalCamSize = (int)(VerticalCamSize * Camera.main.aspect);
        AspectRatio = (int)Camera.main.aspect;
        //Debug.Log("VeticalCamSize: " + VerticalCamSize + "HorizontalCamSize: " + HorizontalCamSize);
        float correctPositionX = AspectRatio * VerticalCamSize;
        //Camera.main.transform.position = new Vector3(correctPositionX, VerticalCamSize, 0);

        Columns = HorizontalCamSize * 2;
        Rows = VerticalCamSize * 2;

        currentCols = Columns;
        currentRows = Rows;
        //currentCamSize = camSize;
        currentCellSize = cellSize;
        cells = new Cell[currentCols, currentRows];
        StartCoroutine(ActualStart());
        GenerateGrid();

    }
    public void ShowGrid()
    {
        VerticalCamSize = (int)Camera.main.orthographicSize;
        HorizontalCamSize = (int)(VerticalCamSize * Camera.main.aspect);
        AspectRatio = (int)Camera.main.aspect;

        Columns = HorizontalCamSize * 2;
        Rows = VerticalCamSize * 2;

        currentCols = Columns;
        currentRows = Rows;
        //currentCamSize = camSize;
        currentCellSize = cellSize;
        cells = new Cell[currentCols, currentRows];


        GenerateGrid();

    }

    public IEnumerator ActualStart()
    {
        crossFade.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

    }


    private void LoadPattern()
    {
        string path = "patterns";

        if(!Directory.Exists(path)) 
        {
            return;   
        }

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));
        string patternName = saveLoadManager.loadDialog.patternName.options[saveLoadManager.loadDialog.patternName.value].text;


        path = path + "/" + patternName + ".xml";

        StreamReader sr = new StreamReader(path);
        Pattern pattern = (Pattern)serializer.Deserialize(sr.BaseStream);

        sr.Close();

        bool isAlive;

        int x = 0, y = 0;

        foreach(char c in pattern.patternString) 
        {
            if (c.ToString() == "1")
            {
                isAlive = true;
            }
            else
            {
                isAlive = false;
            }

            cells[x, y].SetAlive(isAlive);

            x++;

            if(x == currentCols)
            {
                x = 0;
                y++;
            }
        }
    }
    private void SavePattern()
    {
        string path = "patterns";

        if(!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        Pattern pattern = new Pattern();
        string patternString = null;

        for(int y = 0; y < currentRows; y++)
        {
            for(int x = 0; x < currentCols; x++)
            {
                if (cells[x, y].isAlive == false)
                {
                    patternString += "0";
                }
                else
                {
                    patternString += "1";
                }
            }
        }

        pattern.patternString = patternString;

        XmlSerializer serializer = new XmlSerializer(typeof(Pattern));

        StreamWriter writer = new StreamWriter(path + "/" + saveLoadManager.saveDialog.patternName.text + ".xml");

        serializer.Serialize(writer.BaseStream, pattern);
        writer.Close();

        Debug.Log(pattern.patternString);
    }


    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        if(simulationEnabled)
        {
            if (timer >= speed)
            {
                timer = 0;
                CountNeighbors();

                PopulationControl();
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        UserInput();


        //check if Columns or Rows variables have changed. if they have redraw grid.
        if (Columns != currentCols || Rows != currentRows || cellSize != currentCellSize)
        {
            //Debug.Log("Variable changed!");
            //currentCols = Columns;
            //currentRows = Rows;
            //GenerateGrid();
        }

        //resize cell based on slide cell size.
        currentCellSize = cellSize;
        foreach (var cell in generatedObjects)
        {
            cell.transform.localScale = new Vector3(cellSize, cellSize, 0);
        }


    }
    private void Awake()
    {
        generatedObjects.Clear();
    }

    void UserInput()
    {

        if(!saveLoadManager.isActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                //Debug.Log("screen pos: " + screenPosition);
                Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

                Debug.Log("wp: " + worldPosition);
                worldPosition.x = Mathf.Round(worldPosition.x);
                worldPosition.y = Mathf.Round(worldPosition.y);

                int x = (int)worldPosition.x;
                int y = (int)worldPosition.y;
                Debug.Log("x: " + x + ", " + "y: " + y);
                Debug.Log("Current Cols: " + currentCols + " Current Rows: " + currentRows);

                if (x >= 0 && y >= 0 && x < currentCols && y < currentRows)
                {
                    cells[x, y].SetAlive(!cells[x, y].isAlive);
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            //pause
            simulationEnabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //resume
            simulationEnabled = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //save
            //SavePattern();
            saveLoadManager.ShowSaveDialog();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            //load
            //LoadPattern();
            saveLoadManager.ShowLoadDialog();
        }
    }

    public Cell GetCellAtPosition(Vector2 pos)
    {
        if (_cells.TryGetValue(pos, out var cell))
        {
            return cell;
        }

        return null;
    }
    public void ClearGrid() {
        if(generatedObjects.Count != 0) {
            foreach(var cell in generatedObjects) {
                cell.myDestroy(cell);
            }
            generatedObjects.Clear();
        }
    }


    void GenerateGrid() {

        ClearGrid();
        generatedObjects.Clear();

        _cells = new Dictionary<Vector2, Cell>();
        Debug.Log("horirzontal cam size: " + HorizontalCamSize);
        for (int x = 0; x < currentCols; x++) {
            for (int y = 0; y < currentRows; y++) {

 
                float posX = x;
                float posY = y;
                var spawnedCell = Instantiate(cellPrefab, new Vector3(posX, posY), Quaternion.identity);

                cells[x,y] = spawnedCell;
                spawnedCell.cellPosX = x; 
                spawnedCell.cellPosY = y;
                spawnedCell.SetAlive(RandomAliveCell());

                spawnedCell.name = $"Cell {x} {y}";
                generatedObjects.Add(spawnedCell);
                _cells[new Vector2(x, y)] = spawnedCell;
            }
        }

        //Used to list Cells positions.
        foreach (var c in _cells)
        {
            //Debug.Log("cell pos: " + c.Key);
        }

        //cam.transform.position = new Vector3((float)width/2 -0.5f, (float)height/2 -0.5f, camZoom);
        cam.transform.position = new Vector3((float)HorizontalCamSize -0.5f, (float)VerticalCamSize -0.5f, -10);

        //Camera.main.transform.position = new Vector3(currentCols/2 -.5f, currentRows/2 - .5f, 0);
    }

    public bool RandomAliveCell()
    {
        int rand = UnityEngine.Random.Range(0, 100);

        if (rand > 80) {
            return true;
        }
        return false;
    }


    public void CountNeighbors()
    {
        //Debug.Log("cells length: " + cells.Length);
        for(int y = 0; y < currentRows; y++)
        {
            for (int x = 0; x < currentCols; x++)
            {
                int numNeighbors = 0;
                
                //Northn b  
                if (y + 1 < currentRows)
                {
                    if (cells[x, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }


                //East
                if (x + 1 < currentCols)
                {
                    if (cells[x + 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                //South
                if (y - 1 >= 0)
                {
                    if (cells[x, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                //West
                if (x - 1 >= 0)
                {
                    if (cells[x - 1, y].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                //Northeast
                if(x + 1 < currentCols && y + 1 < currentRows)
                {
                    if (cells[x+1, y+1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                //NorthWest
                if (x - 1 >= 0 && y + 1 < currentRows)
                {
                    if (cells[x - 1, y + 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                //Southeast
                if (x + 1 < currentCols && y - 1 >= 0)
                {
                    if (cells[x + 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }

                //SouthWest
                if (x - 1 >= 0 && y - 1 >= 0)
                {
                    if (cells[x - 1, y - 1].isAlive)
                    {
                        numNeighbors++;
                    }
                }
                cells[x, y].numNeighbors = numNeighbors;
            } 


        }
    }


    void PopulationControl()
    {
        for(int y = 0; y < currentRows; y++)
        {
            for(int x = 0; x < currentCols; x++)
            {
                if (cells[x,y].isAlive)
                {
                    //cell alive
                    if (cells[x,y].numNeighbors != 2 && cells[x,y].numNeighbors != 3)
                    {
                        cells[x, y].SetAlive(false);
                    }
                }
                else
                {
                    //cell is dead
                    if (cells[x,y].numNeighbors == 3)
                    {
                        cells[x,y].SetAlive(true);    
                    }
                }
            }
        }
    }


    void OnDestroy()
    {
        Debug.Log("OnDestroy1");
        generatedObjects.Clear();
    }
}
