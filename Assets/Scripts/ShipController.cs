using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//alias PartController as PC
using PC = PartController;

public class ShipController : MonoBehaviour
{
    public const int maxWidth = 10, maxHeight = 7;

    //TODO init Dotween in main loading or whatever


    public List<PartController> Parts = new();
    public Dictionary<Vector2Int, PartController> PartMap = new();

    public PartController PartAtPosition(Vector2Int position)
    {
        return PartMap.ContainsKey(position) ? PartMap[position] : null;
    }

    public PartController GetMainPart()
    {
        const string mainPartName = "Main";
        return PartMap.Values.FirstOrDefault(part => part.PartSO._name == mainPartName);
    }

    public bool HasMainPart()
    {
        return GetMainPart() != null;
    }

    public bool InBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < maxWidth && position.y >= 0 && position.y < maxHeight;
    }

    public void RemovePart(PartController part)
    {
        Parts.Remove(part);
        //TODO do it faster through enumeate
        for (int x=0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (PartAtPosition(position) == part)
                {
                    PartMap.Remove(position);
                }
            }
        }

        Destroy(part.gameObject);
    }

    public bool IsConnectedToMainPart(PartController part)
    {
        if (!HasMainPart())
            return false;
        //BFS
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(part.shipPosition);
        Vector2Int mainPartPosition = GetMainPart().shipPosition;
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (visited.Contains(current))
                continue;
            if (PartAtPosition(current) == null)
                continue;
            visited.Add(current);
            if (current == mainPartPosition)
                return true;
            foreach (Vector2Int direction in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + direction;
                if (InBounds(next))
                    queue.Enqueue(next);
            }
        }
        return false;
    }

    public bool IsWholeShipConnectedToMainPart()
    {
        if (!HasMainPart())
            return false;
        foreach (PartController part in Parts)
        {
            if (!IsConnectedToMainPart(part))
                return false;
        }
        return true;
    }






    #region Gizmos
    private void OnDrawGizmos()
    {
        for (int x = 0; x < maxWidth; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Gizmos.color = PartAtPosition(new Vector2Int(x, y)) ? Color.green : Color.red;
                Vector3 cubePos = new Vector3(x + 0.5f, -y - 0.5f, 0);
                Vector3 cubeSize = Vector3.one * 0.99f;
                cubePos = Vector3.Scale(cubePos, transform.lossyScale) + transform.position;
                cubeSize = Vector3.Scale(cubeSize, transform.lossyScale);
                Gizmos.DrawWireCube(cubePos, cubeSize);
            }
        }
    }
    #endregion
}
