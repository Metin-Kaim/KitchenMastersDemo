using Assets.Game.Scripts.Handlers;
using System.Collections;
using UnityEngine;

namespace Assets.Game.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GridCellHandler selectedCell;
        [SerializeField] private float dragThreshold = 0.1f;

        [SerializeField] private bool isDragged;
        [SerializeField] private Vector2Int dragDirection;
        [SerializeField] private Vector3 mouseTouchedPosition;

        private void Update()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out GridCellHandler cell))
                    {
                        mouseTouchedPosition = mousePos;
                        selectedCell = cell;
                    }
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (selectedCell == null) return;
                if (mousePos != mouseTouchedPosition)
                {
                    if (mousePos.x - mouseTouchedPosition.x > dragThreshold)
                    {
                        isDragged = true;
                        dragDirection = Vector2Int.right;
                        mouseTouchedPosition = mousePos;
                    }
                    else if (mousePos.x - mouseTouchedPosition.x < -dragThreshold)
                    {
                        isDragged = true;
                        dragDirection = Vector2Int.left;
                        mouseTouchedPosition = mousePos;
                    }
                    else if (mousePos.y - mouseTouchedPosition.y > dragThreshold)
                    {
                        isDragged = true;
                        dragDirection = Vector2Int.up;
                        mouseTouchedPosition = mousePos;
                    }
                    else if (mousePos.y - mouseTouchedPosition.y < -dragThreshold)
                    {
                        isDragged = true;
                        dragDirection = Vector2Int.down;
                        mouseTouchedPosition = mousePos;
                    }

                    if (isDragged)
                    {
                        selectedCell.SwapItemWithNeighbourCell(dragDirection);
                        mouseTouchedPosition = Vector3.zero;
                        isDragged = false;
                        selectedCell = null;
                        return;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0) && selectedCell)
            {
                mouseTouchedPosition = Vector3.zero;
                selectedCell = null;
                isDragged = false;
            }
        }
    }
}