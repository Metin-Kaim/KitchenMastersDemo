using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Prefabs and Containers")]
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private Transform gridCellsContainer;
    [SerializeField] private GameObject[] candies;
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 4;

    private GameObject[,] _gridCells;

    private void Start()
    {
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        _gridCells = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 position = new(x, y);
                GameObject cell = Instantiate(gridCellPrefab, position, Quaternion.identity, gridCellsContainer);
                cell.name = $"Cell_{x}_{y}";
                _gridCells[x, y] = cell;
                // Optionally, set cell properties here
            }
        }

        PopulateGridWithCandies();
    }

    private void PopulateGridWithCandies()
    {
        System.Random random = new();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int randomIndex = random.Next(candies.Length);
                GameObject candy = Instantiate(candies[randomIndex]);
                candy.transform.SetParent(_gridCells[x, y].transform);
                candy.transform.localPosition = Vector3.zero;
                candy.name = $"Candy_{x}_{y}";
                // Optionally, set candy properties here
            }
        }

    }
}
