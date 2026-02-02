using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ResponsiveGrid : MonoBehaviour {
    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 4; // x * x
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private float spacing = 10f;

    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    private int lastGridSize;

    void Awake() {
        grid = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();

        InitGrid();
    }

    void Update() {
        // 👇 Runtime grid size change detect
        if (gridSize != lastGridSize) {
            InitGrid();
        }
    }

    void OnRectTransformDimensionsChange() {
        ResizeCells();
    }

    void InitGrid() {
        lastGridSize = gridSize;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = gridSize;
        grid.spacing = new Vector2(spacing, spacing);

        GenerateGrid();
        ResizeCells();
    }

    void GenerateGrid() {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        int totalCells = gridSize * gridSize;

        for (int i = 0; i < totalCells; i++) {
            Instantiate(cellPrefab, transform);
        }
    }

    void ResizeCells() {
        if (gridSize <= 0)
            return;

        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;

        float cellWidth =
            (width - spacing * (gridSize - 1)) / gridSize;
        float cellHeight =
            (height - spacing * (gridSize - 1)) / gridSize;

        float size = Mathf.Min(cellWidth, cellHeight);

        grid.cellSize = new Vector2(size, size);
    }

    // 👇 OPTIONAL: Call from Button / Game Logic
    public void SetGridSize(int newSize) {
        gridSize = newSize;
        InitGrid();
    }
}
