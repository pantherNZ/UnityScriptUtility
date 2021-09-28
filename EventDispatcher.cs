using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventDispatcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Action<Collider> OnTriggerEnterEvent;
    public Action<Collider2D> OnTriggerEnter2DEvent;
    public Action<Collider> OnTriggerExitEvent;
    public Action<Collider2D> OnTriggerExit2DEvent;

    public Action<Collision2D> OnCollisionEnter2DEvent;
    public Action<Collision> OnCollisionEnterEvent;
    public Action<Collision2D> OnCollisionExit2DEvent;
    public Action<Collision> OnCollisionExitEvent;

    public Action<PointerEventData> OnPointerEnterEvent;
    public Action<PointerEventData> OnPointerExitEvent;
    public Action<PointerEventData> OnPointerDownEvent;
    public Action<PointerEventData> OnPointerUpEvent;
    public Action<PointerEventData> OnDoubleClickEvent;
    public float doubleClickInterval = 0.5f;
    float doubleClickTimer = 0.0f;

    public Action<PointerEventData> OnBeginDragEvent;
    public Action<PointerEventData> OnDragEvent;
    public Action<PointerEventData> OnEndDragEvent;

    private void OnTriggerEnter2D( Collider2D collision )
    {
        OnTriggerEnter2DEvent?.Invoke( collision );
    }

    private void OnTriggerExit2D( Collider2D collision )
    {
        OnTriggerExit2DEvent?.Invoke( collision );
    }

    private void OnTriggerEnter( Collider collision )
    {
        OnTriggerEnterEvent?.Invoke( collision );
    }

    private void OnTriggerExit( Collider collision )
    {
        OnTriggerExitEvent?.Invoke( collision );
    }

    private void OnCollisionEnter2D( Collision2D collision )
    {
        OnCollisionEnter2DEvent?.Invoke( collision );
    }

    private void OnCollisionExit2D( Collision2D collision )
    {
        OnCollisionExit2DEvent?.Invoke( collision );
    }

    private void OnCollisionEnter( Collision collision )
    {
        OnCollisionEnterEvent?.Invoke( collision );
    }

    private void OnCollisionExit( Collision collision )
    {
        OnCollisionExitEvent?.Invoke( collision );
    }

    void IPointerEnterHandler.OnPointerEnter( PointerEventData eventData )
    {
        OnPointerEnterEvent?.Invoke( eventData );
    }

    void IPointerExitHandler.OnPointerExit( PointerEventData eventData )
    {
        OnPointerExitEvent?.Invoke( eventData );
    }

    void IPointerDownHandler.OnPointerDown( PointerEventData eventData )
    {
        OnPointerDownEvent?.Invoke( eventData );

        if( Time.time - doubleClickTimer <= doubleClickInterval )
            OnDoubleClickEvent?.Invoke( eventData );
        doubleClickTimer = Time.time;
    }

    void IPointerUpHandler.OnPointerUp( PointerEventData eventData )
    {
        OnPointerUpEvent?.Invoke( eventData );
    }

    void IBeginDragHandler.OnBeginDrag( PointerEventData eventData )
    {
        OnBeginDragEvent?.Invoke( eventData );
    }

    void IEndDragHandler.OnEndDrag( PointerEventData eventData )
    {
        OnEndDragEvent?.Invoke( eventData );
    }

    void IDragHandler.OnDrag( PointerEventData eventData )
    {
        OnDragEvent?.Invoke( eventData );
    }
}