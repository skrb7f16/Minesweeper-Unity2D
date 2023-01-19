using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Tile Unknown;
    public Tile Mine;
    public Tile Empty;
    public Tile Exploded;
    public Tile Flag;
    public Tile num1;
    public Tile num2;
    public Tile num3;
    public Tile num4;
    public Tile num5;
    public Tile num6;
    public Tile num7;
    public Tile num8;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }


    public void Draw(Cell[,] state)
    {
        int width = state.GetLength(0);
        int height = state.GetLength(1);
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Cell cell = state[i, j];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            return GetRevealedTile(cell);
        }
        else if (cell.flagged)
        {
            return Flag;
        }
        else
        {
            return Unknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return Empty;
            case Cell.Type.Mine: return cell.exploded ? Exploded : Mine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return num1;
            case 2: return num2;
            case 3: return num3;
            case 4: return num4;
            case 5: return num5;
            case 6: return num6;
            case 7: return num7;
            case 8: return num8;
            default: return null;
        }
    }



}
