using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    private enum ELayoutMode
    {
        Auto,
        ConstrantByWidth,
        ConstrantByHeight,
        FixedRows,
        FixedColumns,
        FixedCellSize,
    }

    private enum ELayoutDirection
    {
        Vertical,
        Horizontal,
    }

    [SerializeField] private ELayoutMode layoutMode = ELayoutMode.Auto;
    [SerializeField] private ELayoutDirection layoutDirection = ELayoutDirection.Vertical;
    [SerializeField] private int calculateRows;
    public int CalculateRows
    {
        get { return calculateRows; }
    }
    [SerializeField] private int calculateColumns;
    public int CalculateColumns
    {
        get { return calculateColumns; }
    }

    [SerializeField] private int columns;
    public int Columns
    {
        get
        {
            return columns;
        }
    }

    [SerializeField] private int rows;
    public int Rows
    {
        get
        {
            return rows;
        }
    }

    [SerializeField] private Vector2 cellSize;
    public Vector2 spacing;
    private Vector2 extraSpacing;
    [SerializeField] private bool forceExpandWidth = false;
    [SerializeField] private bool forceExpandHeight = false;

    [SerializeField] private bool autoResizeWidth = false;
    [SerializeField] private bool autoResizeHeight = false;

    private void CalculateLayout()
    {
        float sqrRt = Mathf.Sqrt(transform.childCount);

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float canvasScale = 1;
        //if(LGUIManager.Instance != null)
        //{
        //    canvasScale = LGUIManager.Instance.GetCanvasScale;
        //}
        if (transform.childCount <= 0)
        {
            return;
        }

        switch (layoutMode)
        {
            case ELayoutMode.Auto:
                calculateRows = Mathf.CeilToInt(sqrRt);
                calculateColumns = Mathf.CeilToInt(sqrRt);


                cellSize.x = parentWidth / (float)calculateColumns - ((spacing.x / (float)calculateColumns) * 2) - (padding.left + padding.right) / (float)calculateColumns;
                cellSize.y = parentHeight / (float)calculateRows - ((spacing.y / (float)calculateRows) * 2) - (padding.top + padding.bottom) / (float)calculateRows;
                break;
            case ELayoutMode.ConstrantByHeight:
                calculateRows = Mathf.CeilToInt(sqrRt);
                calculateColumns = Mathf.CeilToInt(transform.childCount / (float)calculateRows);

                cellSize.x = parentWidth / (float)calculateColumns - ((spacing.x / (float)calculateColumns) * 2) - (padding.left + padding.right) / (float)calculateColumns;
                cellSize.y = parentHeight / (float)calculateRows - ((spacing.y / (float)calculateRows) * 2) - (padding.top + padding.bottom) / (float)calculateRows;
                break;
            case ELayoutMode.ConstrantByWidth:
                calculateColumns = Mathf.CeilToInt(sqrRt);
                calculateRows = Mathf.CeilToInt(transform.childCount / (float)calculateColumns);

                cellSize.x = parentWidth / (float)calculateColumns - ((spacing.x / (float)calculateColumns) * 2) - (padding.left + padding.right) / (float)calculateColumns;
                cellSize.y = parentHeight / (float)calculateRows - ((spacing.y / (float)calculateRows) * 2) - (padding.top + padding.bottom) / (float)calculateRows;
                break;
            case ELayoutMode.FixedColumns:
                calculateRows = Mathf.CeilToInt(transform.childCount / (float)calculateColumns);


                cellSize.x = parentWidth / (float)calculateColumns - ((spacing.x / (float)calculateColumns) * 2) - (padding.left + padding.right) / (float)calculateColumns;
                cellSize.y = parentHeight / (float)calculateRows - ((spacing.y / (float)calculateRows) * 2) - (padding.top + padding.bottom) / (float)calculateRows;
                break;
            case ELayoutMode.FixedRows:
                calculateColumns = Mathf.CeilToInt(transform.childCount / (float)calculateRows);

                cellSize.x = parentWidth / (float)calculateColumns - ((spacing.x / (float)calculateColumns) * 2) - (padding.left + padding.right) / (float)calculateColumns;
                cellSize.y = parentHeight / (float)calculateRows - ((spacing.y / (float)calculateRows) * 2) - (padding.top + padding.bottom) / (float)calculateRows;
                break;
            case ELayoutMode.FixedCellSize:
                calculateRows = Mathf.FloorToInt((parentHeight - padding.top - padding.bottom) / (cellSize.y * canvasScale + spacing.y));
                calculateColumns = Mathf.FloorToInt((parentWidth - padding.left - padding.right) / (cellSize.x * canvasScale + spacing.x));
                break;
        }

        if ((cellSize.x * canvasScale + spacing.x) * calculateColumns >= (parentWidth - padding.left - padding.right))
        {
            calculateColumns--;
        }
        if ((cellSize.y * canvasScale + spacing.y) * calculateRows >= (parentHeight - padding.top - padding.bottom))
        {
            calculateRows--;
        }

        extraSpacing = new Vector2();
        if (true == forceExpandWidth)
        {
            float leftOverWidth = (parentWidth - padding.left - padding.right) - (cellSize.x * canvasScale + spacing.x) * calculateColumns;
            if (leftOverWidth > 0)
            {
                extraSpacing.x = leftOverWidth / calculateColumns;
            }
        }

        if (true == forceExpandHeight)
        {
            float leftOverHeight = (parentHeight - padding.top - padding.bottom) - (cellSize.y * canvasScale + spacing.y) * calculateRows;
            if (leftOverHeight > 0)
            {
                extraSpacing.y = leftOverHeight / calculateRows;
            }
        }



        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            switch(layoutDirection)
            {
                case ELayoutDirection.Vertical:
                    if(0 != calculateColumns)
                    {
                        rowCount = i / calculateColumns;
                        columnCount = i % calculateColumns;
                    }
                    else
                    {
                        rowCount = i;
                        columnCount = 0;
                    }
                    break;
                case ELayoutDirection.Horizontal:
                    if (0 != calculateRows)
                    {
                        rowCount = i / calculateRows;
                        columnCount = i % calculateRows;
                    }
                    else
                    {
                        rowCount = 0;
                        columnCount = i;
                    }
                    break;
            }

            RectTransform item = rectChildren[i];
            float xPos = (cellSize.x * columnCount * canvasScale) + ((spacing.x + extraSpacing.x) * columnCount) + padding.left;
            float yPos = (cellSize.y * rowCount * canvasScale) + ((spacing.y + extraSpacing.y) * rowCount) + padding.top;
            item.localScale = Vector3.one * canvasScale;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }

        if (true == autoResizeWidth)
        {
            columnCount++;
            rectTransform.sizeDelta = new Vector2((cellSize.x * columnCount * canvasScale) + ((spacing.x + extraSpacing.x) * columnCount) + padding.left, rectTransform.sizeDelta.y);
        }
        if (true == autoResizeHeight)
        {
            rowCount++;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (cellSize.y * rowCount * canvasScale) + ((spacing.y + extraSpacing.y) * rowCount) + padding.top);
        }

        switch (layoutDirection)
        {
            case ELayoutDirection.Vertical:
                rows = rowCount + 1;
                if (0 != calculateColumns)
                {
                    columns = calculateColumns;
                }
                else
                {
                    columns = 1;
                }
                break;
            case ELayoutDirection.Horizontal:
                columns = columnCount + 1;
                if (0 != calculateRows)
                {
                    rows = calculateRows;
                }
                else
                {
                    rows = 1;
                }
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        CalculateLayout();
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        CalculateLayout();
    }
    public override void CalculateLayoutInputVertical()
    {
    }

    public override void SetLayoutHorizontal()
    {
    }

    public override void SetLayoutVertical()
    {
    }
}
