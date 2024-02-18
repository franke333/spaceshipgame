using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//alias PartController as PC
using PC = PartController;

public class ShipBuilderController : ShipController
{
    public PartController partControllerPrefab;
    public bool IsReady { get; private set; }
    public bool CanPlacePart(PartSO part, Vector2Int position)
    {
        for (int i = 0; i < PC.maxWidth; i++)
            for (int j = 0; j < PC.maxHeight; j++)
                if (part.shape[i * PC.maxWidth + j] &&
                    (PartAtPosition(position + new Vector2Int(i, j)) || !InBounds(position + new Vector2Int(i, j))))
                    return false;
        return true;
    }

    public void PlacePart(PartSO part, Vector2Int position)
    {
        if (!CanPlacePart(part, position))
        {
            Debug.LogError("Can't place part at " + position);
            return;
        }
        PartController partC = Instantiate(partControllerPrefab, transform);
        partC.PartSO = part;
        partC.transform.localPosition = new Vector3(position.x, -position.y, 0);
        partC.shipPosition = position;
        Parts.Add(partC);
        for (int i = 0; i < PC.maxWidth; i++)
            for (int j = 0; j < PC.maxHeight; j++)
                if (part.shape[j * PC.maxWidth + i])
                    PartMap.Add(position + new Vector2Int(i, j), partC);
    }

    public Vector2Int WorldToShipPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = transform.InverseTransformPoint(worldPosition) - new Vector3(1,-1,0)/2f;
        return new Vector2Int(Mathf.RoundToInt(localPosition.x), -Mathf.RoundToInt(localPosition.y));
    }


    public void PickUpPart()
    {
        Vector2Int mousePos = WorldToShipPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Debug.Log("PickUpPart " + mousePos);
        if (InBounds(mousePos) && PartAtPosition(mousePos) != null)
        {
            PartSO part = PartAtPosition(mousePos).PartSO;
            RemovePart(PartAtPosition(mousePos));
            var item = InventoryManager.Instance.AddPiece(part);
            InventoryManager.Instance.draggedItemController.GrabItem(item);
        }
    }

    private void OnMouseDown()
    {
        
        PickUpPart();
    }
}
