using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public Board board;
    public int width = 16;
    public int height = 16;
    public int mineCount=32;
    public Cell[,] state;
    private bool gameover;


    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }
    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        state = new Cell[width, height];

        GenerateCells();
        GenerateMines();
        GenerateNumber();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10);
        board.Draw(state);
    }

    public void GenerateCells()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(i, j, 0);
                cell.type = Cell.Type.Empty;
                state[i, j] = cell;
            }
        }
    }

    public void GenerateMines()
    {
        for(int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;
                if (x >= width)
                {
                    x = 0;
                    y++;
                    if (y >= height)
                    {
                        y = 0;

                    }
                }
            }
            state[x, y].type = Cell.Type.Mine;
        }
    }

    public void GenerateNumber()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (state[i, j].type == Cell.Type.Mine) continue;
                Cell cell = state[i, j];
                int num = CountMinesNear(cell.position.x, cell.position.y);
                if (num > 0)
                {
                    cell.type = Cell.Type.Number;
                    cell.number = num;

                }
            
                state[i, j] = cell;
            }
        }
    }

    public int CountMinesNear(int x,int y)
    {
        int count = 0;
        for(int i = -1; i <= 1; i++)
        {
            for(int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int newx = i + x;
                int newy = i + y;
                if (isValid(newx, newy) && state[newx,newy].type==Cell.Type.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            NewGame();
        }
        else if (!gameover)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Flag();
                board.Draw(state);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Reveal();
                board.Draw(state);
            }
        }
    }

    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);
        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
        {
            return;
        }

        if (cell.type == Cell.Type.Mine)
        {
            ExplodeTheCell(cell);
            return;
        }
        else if (cell.type == Cell.Type.Empty)
        {
            Flood(cell);
            checkWinCondition();
            return;
        }
        else
        {
            cell.revealed = true;
            state[cell.position.x, cell.position.y] = cell;
            checkWinCondition();
        }
    }

    private void Flood(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.type ==Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;
        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;
        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
        }
    }

    private void checkWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

               
                if (cell.type != Cell.Type.Mine && !cell.revealed)
                {
                    return; 
                }
            }
        }

        Debug.Log("Winner!");
        gameover = true;

        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    private void ExplodeTheCell(Cell cell)
    {
        Debug.Log("Game Over!");
        gameover = true;

        
        cell.exploded = true;
        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
    }

    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.type == Cell.Type.Invalid || cell.revealed)
        {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }


    public Cell GetCell(int x,int y)
    {
        if (isValid(x, y)) return state[x, y];
        else return new Cell();
    }
    public bool isValid(int x,int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
}
