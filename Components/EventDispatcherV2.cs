using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventDispatcherV2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
#if PHYSICS_2D_PACKAGE
    public UnityEvent<Collider2D> OnTriggerEnter2DEvent;
    public UnityEvent<Collider2D> OnTriggerExit2DEvent;

    public UnityEvent<Collision2D> OnCollisionEnter2DEvent;
    public UnityEvent<Collision2D> OnCollisionExit2DEvent;
#endif

#if PHYSICS_PACKAGE
    public UnityEvent<Collider> OnTriggerEnterEvent;
    public UnityEvent<Collider> OnTriggerExitEvent;

    public UnityEvent<Collision> OnCollisionEnterEvent;
    public UnityEvent<Collision> OnCollisionExitEvent;
#endif

    public UnityNullable<PointerEventData.InputButton> buttonRestriction;
    public UnityEvent<PointerEventData> OnPointerEnterEvent;
    public UnityEvent<PointerEventData> OnPointerExitEvent;
    public UnityEvent<PointerEventData> OnPointerDownEvent;
    public UnityEvent<PointerEventData> OnPointerUpEvent;

    private float doubleClickTimer = 0.0f;
    public float doubleClickInterval = 0.5f;
    public bool doubleClickUsePointerUp = false;
    public UnityEvent<PointerEventData> OnDoubleClickEvent;

    public UnityEvent<PointerEventData> OnBeginDragEvent;
    public UnityEvent<PointerEventData> OnDragEvent;
    public UnityEvent<PointerEventData> OnEndDragEvent;

    private bool Filter( PointerEventData eventData )
    {
        return !buttonRestriction.HasValue || eventData.button == buttonRestriction.Value;
    }

#if PHYSICS_2D_PACKAGE
    private bool Filter( Collider2D collision )
    {
        return true;
    }

    private bool Filter( Collision2D collision )
    {
        return true;
    }

    private void OnTriggerEnter2D( Collider2D collision )
    {
        if( Filter( collision ) )
            OnTriggerEnter2DEvent?.Invoke( collision );
    }

    private void OnTriggerExit2D( Collider2D collision )
    {
        if( Filter( collision ) )
            OnTriggerExit2DEvent?.Invoke( collision );
    }

    private void OnCollisionEnter2D( Collision2D collision )
    {
        if( Filter( collision ) )
            OnCollisionEnter2DEvent?.Invoke( collision );
    }

    private void OnCollisionExit2D( Collision2D collision )
    {
        if( Filter( collision ) )
            OnCollisionExit2DEvent?.Invoke( collision );
    }
#endif

#if PHYSICS_PACKAGE
    private bool Filter( Collider collision )
    {
        return true;
    }

    private bool Filter( Collision collision )
    {
        return true;
    }

    private void OnTriggerEnter( Collider collision )
    {
        if( Filter( collision ) )
            OnTriggerEnterEvent?.Invoke( collision );
    }

    private void OnTriggerExit( Collider collision )
    {
        if( Filter( collision ) )
            OnTriggerExitEvent?.Invoke( collision );
    }

    private void OnCollisionEnter( Collision collision )
    {
        if( Filter( collision ) )
            OnCollisionEnterEvent?.Invoke( collision );
    }

    private void OnCollisionExit( Collision collision )
    {
        if( Filter( collision ) )
            OnCollisionExitEvent?.Invoke( collision );
    }
#endif

    void IPointerEnterHandler.OnPointerEnter( PointerEventData eventData )
    {
        if( Filter( eventData ) )
            OnPointerEnterEvent?.Invoke( eventData );
    }

    void IPointerExitHandler.OnPointerExit( PointerEventData eventData )
    {
        if( Filter( eventData ) ) 
            OnPointerExitEvent?.Invoke( eventData );
    }

    void IPointerDownHandler.OnPointerDown( PointerEventData eventData )
    {
        if( Filter( eventData ) )
        {
            OnPointerDownEvent?.Invoke( eventData );

            if( !doubleClickUsePointerUp )
            {
                if( Time.time - doubleClickTimer <= doubleClickInterval )
                    OnDoubleClickEvent?.Invoke( eventData );
                doubleClickTimer = Time.time;
            }
        }
    }

    void IPointerUpHandler.OnPointerUp( PointerEventData eventData )
    {
        if( Filter( eventData ) )
        {
            OnPointerUpEvent?.Invoke( eventData );

            if( doubleClickUsePointerUp )
            {
                if( Time.time - doubleClickTimer <= doubleClickInterval )
                    OnDoubleClickEvent?.Invoke( eventData );
                doubleClickTimer = Time.time;
            }
        }
    }

    void IBeginDragHandler.OnBeginDrag( PointerEventData eventData )
    {
        if( Filter( eventData ) ) 
            OnBeginDragEvent?.Invoke( eventData );
    }

    void IEndDragHandler.OnEndDrag( PointerEventData eventData )
    {
        if( Filter( eventData ) ) 
            OnEndDragEvent?.Invoke( eventData );
    }

    void IDragHandler.OnDrag( PointerEventData eventData )
    {
        if( Filter( eventData ) ) 
            OnDragEvent?.Invoke( eventData );
    }
}