using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggedItemController : MonoBehaviour
{
    [SerializeField]
    private PartItemScript _holdingPartItem;

    //TODO make the parts throwable
    //TODO grab the part at exact position
    //TODO make the part return to upward rotation

    public void GrabItem(PartItemScript item)
    {
        Debug.Log("GrabItem");
        item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        _holdingPartItem = item;

    }

    private void MoveItem()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _holdingPartItem.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
    }

    private void Update()
    {
        if (_holdingPartItem == null)
            return;
        MoveItem();
        if (Input.GetMouseButtonUp(0))
        {
            var shipyard = InventoryManager.Instance._shipYard;

            Vector2Int shipPosition = shipyard.WorldToShipPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (shipyard.InBounds(shipPosition) && shipyard.CanPlacePart(_holdingPartItem.partSO, shipPosition))
            {
                shipyard.PlacePart(_holdingPartItem.partSO, shipPosition);
                InventoryManager.Instance.RemovePiece(_holdingPartItem);
                Destroy(_holdingPartItem.gameObject);
                _holdingPartItem = null;
                return;
            }
            var rb = _holdingPartItem.GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
            _holdingPartItem = null;
        }

    }
}
