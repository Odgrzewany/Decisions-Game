using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour , IBeginDragHandler, IEndDragHandler, IDragHandler
{

    private Vector2 _offset;
    private readonly int _screenWidth = Screen.width / 2;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _offset = Input.mousePosition - transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = (Vector2)Input.mousePosition - _offset;

        if (transform.position.x < _screenWidth)
        {
            GameController.Gc.HighlightLeft();
        }
        else if (transform.position.x > _screenWidth)
        {
            GameController.Gc.HighlightRight();
        }
        else
        {
            GameController.Gc.NormalSide();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (transform.position.x < _screenWidth)
        {
            GameController.Gc.LeftSide();
        }
        else if (transform.position.x > _screenWidth)
        {
            GameController.Gc.RightSide();
        }

        transform.localPosition = Vector3.zero;
    }
}
