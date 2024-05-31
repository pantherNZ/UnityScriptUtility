﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro
{
    [AddComponentMenu( "UI/MultiDropdown - TextMeshPro", 35 )]
    [RequireComponent( typeof( RectTransform ) )]
    public class TMP_MultiDropdown : Selectable, IPointerClickHandler, IEventSystemHandler, ISubmitHandler, ICancelHandler
    {
        internal interface ITweenValue
        {
            bool ignoreTimeScale
            {
                get;
            }

            float duration
            {
                get;
            }

            void TweenValue( float floatPercentage );

            bool ValidTarget();
        }

        internal class TweenRunner<T> where T : struct, ITweenValue
        {
            protected MonoBehaviour m_CoroutineContainer;

            protected IEnumerator m_Tween;

            private static IEnumerator Start( T tweenInfo )
            {
                if( tweenInfo.ValidTarget() )
                {
                    float elapsedTime = 0f;
                    while( elapsedTime < tweenInfo.duration )
                    {
                        elapsedTime += ( tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime );
                        float percentage = Mathf.Clamp01( elapsedTime / tweenInfo.duration );
                        tweenInfo.TweenValue( percentage );
                        yield return null;
                    }

                    tweenInfo.TweenValue( 1f );
                }
            }

            public void Init( MonoBehaviour coroutineContainer )
            {
                m_CoroutineContainer = coroutineContainer;
            }

            public void StartTween( T info )
            {
                if( m_CoroutineContainer == null )
                {
                    Debug.LogWarning( "Coroutine container not configured... did you forget to call Init?" );
                    return;
                }

                StopTween();
                if( !m_CoroutineContainer.gameObject.activeInHierarchy )
                {
                    info.TweenValue( 1f );
                    return;
                }

                m_Tween = Start( info );
                m_CoroutineContainer.StartCoroutine( m_Tween );
            }

            public void StopTween()
            {
                if( m_Tween != null )
                {
                    m_CoroutineContainer.StopCoroutine( m_Tween );
                    m_Tween = null;
                }
            }
        }

        internal struct FloatTween : ITweenValue
        {
            public class FloatTweenCallback : UnityEvent<float>
            {
            }

            private FloatTweenCallback m_Target;

            private float m_StartValue;

            private float m_TargetValue;

            private float m_Duration;

            private bool m_IgnoreTimeScale;

            public float startValue
            {
                get
                {
                    return m_StartValue;
                }
                set
                {
                    m_StartValue = value;
                }
            }

            public float targetValue
            {
                get
                {
                    return m_TargetValue;
                }
                set
                {
                    m_TargetValue = value;
                }
            }

            public float duration
            {
                get
                {
                    return m_Duration;
                }
                set
                {
                    m_Duration = value;
                }
            }

            public bool ignoreTimeScale
            {
                get
                {
                    return m_IgnoreTimeScale;
                }
                set
                {
                    m_IgnoreTimeScale = value;
                }
            }

            public void TweenValue( float floatPercentage )
            {
                if( ValidTarget() )
                {
                    float arg = Mathf.Lerp( m_StartValue, m_TargetValue, floatPercentage );
                    m_Target.Invoke( arg );
                }
            }

            public void AddOnChangedCallback( UnityAction<float> callback )
            {
                if( m_Target == null )
                {
                    m_Target = new FloatTweenCallback();
                }

                m_Target.AddListener( callback );
            }

            public bool GetIgnoreTimescale()
            {
                return m_IgnoreTimeScale;
            }

            public float GetDuration()
            {
                return m_Duration;
            }

            public bool ValidTarget()
            {
                return m_Target != null;
            }
        }

        internal class TMP_ObjectPool<T> where T : new()
        {
            private readonly Stack<T> m_Stack = new Stack<T>();

            private readonly UnityAction<T> m_ActionOnGet;

            private readonly UnityAction<T> m_ActionOnRelease;

            public int countAll
            {
                get;
                private set;
            }

            public int countActive => countAll - countInactive;

            public int countInactive => m_Stack.Count;

            public TMP_ObjectPool( UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease )
            {
                m_ActionOnGet = actionOnGet;
                m_ActionOnRelease = actionOnRelease;
            }

            public T Get()
            {
                T val;
                if( m_Stack.Count == 0 )
                {
                    val = new T();
                    countAll++;
                }
                else
                {
                    val = m_Stack.Pop();
                }

                if( m_ActionOnGet != null )
                {
                    m_ActionOnGet( val );
                }

                return val;
            }

            public void Release( T element )
            {
                if( m_Stack.Count > 0 && ( object )m_Stack.Peek() == ( object )element )
                {
                    Debug.LogError( "Internal error. Trying to destroy object that is already released to pool." );
                }

                if( m_ActionOnRelease != null )
                {
                    m_ActionOnRelease( element );
                }

                m_Stack.Push( element );
            }
        }

        internal static class TMP_ListPool<T>
        {
            private static readonly TMP_ObjectPool<List<T>> s_ListPool = new TMP_ObjectPool<List<T>>( null, delegate ( List<T> l )
            {
                l.Clear();
            } );

            public static List<T> Get()
            {
                return s_ListPool.Get();
            }

            public static void Release( List<T> toRelease )
            {
                s_ListPool.Release( toRelease );
            }
        }

        protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, ICancelHandler
        {
            [SerializeField]
            private TMP_Text m_Text;

            [SerializeField]
            private Image m_Image;

            [SerializeField]
            private RectTransform m_RectTransform;

            [SerializeField]
            private Toggle m_Toggle;

            public TMP_Text text
            {
                get
                {
                    return m_Text;
                }
                set
                {
                    m_Text = value;
                }
            }

            public Image image
            {
                get
                {
                    return m_Image;
                }
                set
                {
                    m_Image = value;
                }
            }

            public RectTransform rectTransform
            {
                get
                {
                    return m_RectTransform;
                }
                set
                {
                    m_RectTransform = value;
                }
            }

            public Toggle toggle
            {
                get
                {
                    return m_Toggle;
                }
                set
                {
                    m_Toggle = value;
                }
            }

            public virtual void OnPointerEnter( PointerEventData eventData )
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject( base.gameObject );
            }

            public virtual void OnCancel( BaseEventData eventData )
            {
                TMP_Dropdown componentInParent = GetComponentInParent<TMP_Dropdown>();
                if( ( bool )componentInParent )
                {
                    componentInParent.Hide();
                }
            }
        }

        [Serializable]
        public class OptionData
        {
            [SerializeField]
            private string m_Text;

            [SerializeField]
            private Sprite m_Image;

            public string text
            {
                get
                {
                    return m_Text;
                }
                set
                {
                    m_Text = value;
                }
            }

            public Sprite image
            {
                get
                {
                    return m_Image;
                }
                set
                {
                    m_Image = value;
                }
            }

            public OptionData()
            {
            }

            public OptionData( string text )
            {
                this.text = text;
            }

            public OptionData( Sprite image )
            {
                this.image = image;
            }

            public OptionData( string text, Sprite image )
            {
                this.text = text;
                this.image = image;
            }
        }

        [Serializable]
        public class OptionDataList
        {
            [SerializeField]
            private List<OptionData> m_Options;

            public List<OptionData> options
            {
                get
                {
                    return m_Options;
                }
                set
                {
                    m_Options = value;
                }
            }

            public OptionDataList()
            {
                options = new List<OptionData>();
            }
        }

        [Serializable]
        public class MultiDropdownEvent : UnityEvent<List<int>>
        {
        }

        [SerializeField]
        private RectTransform m_Template;

        [SerializeField]
        private TMP_Text m_CaptionText;

        [SerializeField]
        private string m_DefaultCaptionText = string.Empty;

        [SerializeField]
        private Image m_CaptionImage;

        [SerializeField]
        private Graphic m_Placeholder;

        [Space]
        [SerializeField]
        private TMP_Text m_ItemText;

        [SerializeField]
        private Image m_ItemImage;

        [Space]
        [SerializeField]
        private List<int> m_Values;

        [Space]
        [SerializeField]
        private OptionDataList m_Options = new OptionDataList();

        [Space]
        [SerializeField]
        private MultiDropdownEvent m_OnValuesChanged = new MultiDropdownEvent();

        [SerializeField]
        private float m_AlphaFadeSpeed = 0.15f;

        private GameObject m_Dropdown;

        private GameObject m_Blocker;

        private List<DropdownItem> m_Items = new List<DropdownItem>();

        private TweenRunner<FloatTween> m_AlphaTweenRunner;

        private bool validTemplate = false;

        private Coroutine m_Coroutine = null;

        private static OptionData s_NoOptionData = new OptionData();

        public RectTransform template
        {
            get
            {
                return m_Template;
            }
            set
            {
                m_Template = value;
                RefreshShownValue();
            }
        }

        public TMP_Text captionText
        {
            get
            {
                return m_CaptionText;
            }
            set
            {
                m_CaptionText = value;
                RefreshShownValue();
            }
        }

        public Image captionImage
        {
            get
            {
                return m_CaptionImage;
            }
            set
            {
                m_CaptionImage = value;
                RefreshShownValue();
            }
        }

        public Graphic placeholder
        {
            get
            {
                return m_Placeholder;
            }
            set
            {
                m_Placeholder = value;
                RefreshShownValue();
            }
        }

        public TMP_Text itemText
        {
            get
            {
                return m_ItemText;
            }
            set
            {
                m_ItemText = value;
                RefreshShownValue();
            }
        }

        public Image itemImage
        {
            get
            {
                return m_ItemImage;
            }
            set
            {
                m_ItemImage = value;
                RefreshShownValue();
            }
        }

        public List<OptionData> options
        {
            get
            {
                return m_Options.options;
            }
            set
            {
                m_Options.options = value;
                RefreshShownValue();
            }
        }

        public MultiDropdownEvent onValuesChanged
        {
            get
            {
                return m_OnValuesChanged;
            }
            set
            {
                m_OnValuesChanged = value;
            }
        }

        public float alphaFadeSpeed
        {
            get
            {
                return m_AlphaFadeSpeed;
            }
            set
            {
                m_AlphaFadeSpeed = value;
            }
        }

        public ReadOnlyCollection<int> values
        {
            get
            {
                return m_Values.AsReadOnly();
            }
            set
            {
                SetValues( value );
            }
        }

        public bool IsExpanded => m_Dropdown != null;

        public void SetValuesWithoutNotify( ReadOnlyCollection<int> input )
        {
            SetValues( input, sendCallback: false );
        }

        private void SetValues( ReadOnlyCollection<int> values, bool sendCallback = true )
        {
            if( values == null )
                values = new ReadOnlyCollection<int>( new List<int>() );

            if( !Application.isPlaying || ( values != m_Values.AsReadOnly() && options.Count != 0 ) )
            {
                m_Values.Clear();
                for( int i = 0; i < values.Count; ++i )
                    m_Values.Add(Mathf.Clamp( values[i], 0, options.Count - 1 ));
                RefreshShownValue();
                if( sendCallback )
                {
                    UISystemProfilerApi.AddMarker( "Dropdown.value", this );
                    m_OnValuesChanged.Invoke( m_Values );
                }
            }
        }

        protected TMP_MultiDropdown()
        {
        }

        protected override void Awake()
        {
            if( Application.isPlaying )
            {
                if( ( bool )m_CaptionImage )
                {
                    m_CaptionImage.enabled = ( m_CaptionImage.sprite != null );
                }

                if( ( bool )m_Template )
                {
                    m_Template.gameObject.SetActive( value: false );
                }
            }
        }

        protected override void Start()
        {
            m_AlphaTweenRunner = new TweenRunner<FloatTween>();
            m_AlphaTweenRunner.Init( this );
            base.Start();
            RefreshShownValue();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if( IsActive() )
            {
                RefreshShownValue();
            }
        }
#endif

        protected override void OnDisable()
        {
            ImmediateDestroyDropdownList();
            if( m_Blocker != null )
            {
                DestroyBlocker( m_Blocker );
            }

            m_Blocker = null;
            base.OnDisable();
        }

        public void RefreshShownValue()
        {
            OptionData optionData = s_NoOptionData;
            if( options.Count > 0 && m_Values.Count == 1 )
            {
                optionData = options[Mathf.Clamp( m_Values[0], 0, options.Count - 1 )];
            }

            if( ( bool )m_CaptionText )
            {
                if( optionData != null && optionData.text != null )
                {
                    m_CaptionText.text = optionData.text;
                }
                else if( m_Values.Count > 1 )
                {
                    m_CaptionText.text = "Multiple..";
                }
                else
                {
                    m_CaptionText.text = m_DefaultCaptionText;
                }
            }

            if( ( bool )m_CaptionImage )
            {
                if( optionData != null )
                {
                    m_CaptionImage.sprite = optionData.image;
                }
                else
                {
                    m_CaptionImage.sprite = null;
                }

                m_CaptionImage.enabled = ( m_CaptionImage.sprite != null );
            }

            if( ( bool )m_Placeholder )
            {
                m_Placeholder.enabled = ( options.Count == 0 || values.IsEmpty() );
            }
        }

        public void AddOptions( List<OptionData> options )
        {
            this.options.AddRange( options );
            RefreshShownValue();
        }

        public void AddOptions( List<string> options )
        {
            for( int i = 0; i < options.Count; i++ )
            {
                this.options.Add( new OptionData( options[i] ) );
            }

            RefreshShownValue();
        }

        public void AddOptions( List<Sprite> options )
        {
            for( int i = 0; i < options.Count; i++ )
            {
                this.options.Add( new OptionData( options[i] ) );
            }

            RefreshShownValue();
        }

        public void ClearOptions()
        {
            options.Clear();
            m_Values.Clear();
            RefreshShownValue();
        }

        private void SetupTemplate()
        {
            validTemplate = false;
            if( !m_Template )
            {
                Debug.LogError( "The dropdown template is not assigned. The template needs to be assigned and must have a child GameObject with a Toggle component serving as the item.", this );
                return;
            }

            GameObject gameObject = m_Template.gameObject;
            gameObject.SetActive( value: true );
            Toggle componentInChildren = m_Template.GetComponentInChildren<Toggle>();
            validTemplate = true;
            if( !componentInChildren || componentInChildren.transform == template )
            {
                validTemplate = false;
                Debug.LogError( "The dropdown template is not valid. The template must have a child GameObject with a Toggle component serving as the item.", template );
            }
            else if( !( componentInChildren.transform.parent is RectTransform ) )
            {
                validTemplate = false;
                Debug.LogError( "The dropdown template is not valid. The child GameObject with a Toggle component (the item) must have a RectTransform on its parent.", template );
            }
            else if( itemText != null && !itemText.transform.IsChildOf( componentInChildren.transform ) )
            {
                validTemplate = false;
                Debug.LogError( "The dropdown template is not valid. The Item Text must be on the item GameObject or children of it.", template );
            }
            else if( itemImage != null && !itemImage.transform.IsChildOf( componentInChildren.transform ) )
            {
                validTemplate = false;
                Debug.LogError( "The dropdown template is not valid. The Item Image must be on the item GameObject or children of it.", template );
            }

            if( !validTemplate )
            {
                gameObject.SetActive( value: false );
                return;
            }

            DropdownItem dropdownItem = componentInChildren.gameObject.AddComponent<DropdownItem>();
            dropdownItem.text = m_ItemText;
            dropdownItem.image = m_ItemImage;
            dropdownItem.toggle = componentInChildren;
            dropdownItem.rectTransform = ( RectTransform )componentInChildren.transform;
            Canvas canvas = null;
            Transform parent = m_Template.parent;
            while( parent != null )
            {
                canvas = parent.GetComponent<Canvas>();
                if( canvas != null )
                {
                    break;
                }

                parent = parent.parent;
            }

            Canvas orAddComponent = GetOrAddComponent<Canvas>( gameObject );
            orAddComponent.overrideSorting = true;
            orAddComponent.sortingOrder = 30000;
            if( canvas != null )
            {
                Component[] components = canvas.GetComponents<BaseRaycaster>();
                Component[] array = components;
                for( int i = 0; i < array.Length; i++ )
                {
                    Type type = array[i].GetType();
                    if( gameObject.GetComponent( type ) == null )
                    {
                        gameObject.AddComponent( type );
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>( gameObject );
            }

            GetOrAddComponent<CanvasGroup>( gameObject );
            gameObject.SetActive( value: false );
            validTemplate = true;
        }

        private static T GetOrAddComponent<T>( GameObject go ) where T : Component
        {
            T val = go.GetComponent<T>();
            if( !( UnityEngine.Object )val )
            {
                val = go.AddComponent<T>();
            }

            return val;
        }

        public virtual void OnPointerClick( PointerEventData eventData )
        {
            Show();
        }

        public virtual void OnSubmit( BaseEventData eventData )
        {
            Show();
        }

        public virtual void OnCancel( BaseEventData eventData )
        {
            Hide();
        }

        public void Show()
        {
            if( m_Coroutine != null )
            {
                StopCoroutine( m_Coroutine );
                ImmediateDestroyDropdownList();
            }

            if( !IsActive() || !IsInteractable() || m_Dropdown != null )
            {
                return;
            }

            List<Canvas> list = TMP_ListPool<Canvas>.Get();
            base.gameObject.GetComponentsInParent( includeInactive: false, list );
            if( list.Count == 0 )
            {
                return;
            }

            Canvas canvas = list[list.Count - 1];
            for( int i = 0; i < list.Count; i++ )
            {
                if( list[i].isRootCanvas )
                {
                    canvas = list[i];
                    break;
                }
            }

            TMP_ListPool<Canvas>.Release( list );
            if( !validTemplate )
            {
                SetupTemplate();
                if( !validTemplate )
                {
                    return;
                }
            }

            m_Template.gameObject.SetActive( value: true );
            m_Template.GetComponent<Canvas>().sortingLayerID = canvas.sortingLayerID;
            m_Dropdown = CreateDropdownList( m_Template.gameObject );
            m_Dropdown.name = "Dropdown List";
            m_Dropdown.SetActive( value: true );
            RectTransform rectTransform = m_Dropdown.transform as RectTransform;
            rectTransform.SetParent( m_Template.transform.parent, worldPositionStays: false );
            DropdownItem componentInChildren = m_Dropdown.GetComponentInChildren<DropdownItem>();
            GameObject gameObject = componentInChildren.rectTransform.parent.gameObject;
            RectTransform rectTransform2 = gameObject.transform as RectTransform;
            componentInChildren.rectTransform.gameObject.SetActive( value: true );
            Rect rect = rectTransform2.rect;
            Rect rect2 = componentInChildren.rectTransform.rect;
            Vector2 vector = rect2.min - rect.min + ( Vector2 )componentInChildren.rectTransform.localPosition;
            Vector2 vector2 = rect2.max - rect.max + ( Vector2 )componentInChildren.rectTransform.localPosition;
            Vector2 size = rect2.size;
            m_Items.Clear();
            Toggle toggle = null;
            for( int j = 0; j < options.Count; j++ )
            {
                OptionData data = options[j];
                var isSelected = values.Contains( j );
                DropdownItem item = AddItem( data, isSelected, componentInChildren, m_Items );
                if( !( item == null ) )
                {
                    item.toggle.isOn = isSelected;
                    item.toggle.onValueChanged.AddListener( delegate
                    {
                        OnToggleItem( item.toggle );
                    } );
                    if( item.toggle.isOn )
                    {
                        item.toggle.Select();
                    }

                    if( toggle != null )
                    {
                        Navigation navigation = toggle.navigation;
                        Navigation navigation2 = item.toggle.navigation;
                        navigation.mode = Navigation.Mode.Explicit;
                        navigation2.mode = Navigation.Mode.Explicit;
                        navigation.selectOnDown = item.toggle;
                        navigation.selectOnRight = item.toggle;
                        navigation2.selectOnLeft = toggle;
                        navigation2.selectOnUp = toggle;
                        toggle.navigation = navigation;
                        item.toggle.navigation = navigation2;
                    }

                    toggle = item.toggle;
                }
            }

            Vector2 sizeDelta = rectTransform2.sizeDelta;
            sizeDelta.y = size.y * ( float )m_Items.Count + vector.y - vector2.y;
            rectTransform2.sizeDelta = sizeDelta;
            float num = rectTransform.rect.height - rectTransform2.rect.height;
            if( num > 0f )
            {
                rectTransform.sizeDelta = new Vector2( rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - num );
            }

            Vector3[] array = new Vector3[4];
            rectTransform.GetWorldCorners( array );
            RectTransform rectTransform3 = canvas.transform as RectTransform;
            Rect rect3 = rectTransform3.rect;
            for( int k = 0; k < 2; k++ )
            {
                bool flag = false;
                for( int l = 0; l < 4; l++ )
                {
                    Vector3 vector3 = rectTransform3.InverseTransformPoint( array[l] );
                    if( ( vector3[k] < rect3.min[k] && !Mathf.Approximately( vector3[k], rect3.min[k] ) ) || ( vector3[k] > rect3.max[k] && !Mathf.Approximately( vector3[k], rect3.max[k] ) ) )
                    {
                        flag = true;
                        break;
                    }
                }

                if( flag )
                {
                    RectTransformUtility.FlipLayoutOnAxis( rectTransform, k, keepPositioning: false, recursive: false );
                }
            }

            for( int m = 0; m < m_Items.Count; m++ )
            {
                RectTransform rectTransform4 = m_Items[m].rectTransform;
                rectTransform4.anchorMin = new Vector2( rectTransform4.anchorMin.x, 0f );
                rectTransform4.anchorMax = new Vector2( rectTransform4.anchorMax.x, 0f );
                rectTransform4.anchoredPosition = new Vector2( rectTransform4.anchoredPosition.x, vector.y + size.y * ( float )( m_Items.Count - 1 - m ) + size.y * rectTransform4.pivot.y );
                rectTransform4.sizeDelta = new Vector2( rectTransform4.sizeDelta.x, size.y );
            }

            AlphaFadeList( m_AlphaFadeSpeed, 0f, 1f );
            m_Template.gameObject.SetActive( value: false );
            componentInChildren.gameObject.SetActive( value: false );
            m_Blocker = CreateBlocker( canvas );
        }

        protected virtual GameObject CreateBlocker( Canvas rootCanvas )
        {
            GameObject gameObject = new GameObject( "Blocker" );
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetParent( rootCanvas.transform, worldPositionStays: false );
            rectTransform.anchorMin = Vector3.zero;
            rectTransform.anchorMax = Vector3.one;
            rectTransform.sizeDelta = Vector2.zero;
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            Canvas component = m_Dropdown.GetComponent<Canvas>();
            canvas.sortingLayerID = component.sortingLayerID;
            canvas.sortingOrder = component.sortingOrder - 1;
            Canvas canvas2 = null;
            Transform parent = m_Template.parent;
            while( parent != null )
            {
                canvas2 = parent.GetComponent<Canvas>();
                if( canvas2 != null )
                {
                    break;
                }

                parent = parent.parent;
            }

            if( canvas2 != null )
            {
                Component[] components = canvas2.GetComponents<BaseRaycaster>();
                Component[] array = components;
                for( int i = 0; i < array.Length; i++ )
                {
                    Type type = array[i].GetType();
                    if( gameObject.GetComponent( type ) == null )
                    {
                        gameObject.AddComponent( type );
                    }
                }
            }
            else
            {
                GetOrAddComponent<GraphicRaycaster>( gameObject );
            }

            Image image = gameObject.AddComponent<Image>();
            image.color = Color.clear;
            Button button = gameObject.AddComponent<Button>();
            button.onClick.AddListener( Hide );
            return gameObject;
        }

        protected virtual void DestroyBlocker( GameObject blocker )
        {
            UnityEngine.Object.Destroy( blocker );
        }

        protected virtual GameObject CreateDropdownList( GameObject template )
        {
            return UnityEngine.Object.Instantiate( template );
        }

        protected virtual void DestroyDropdownList( GameObject dropdownList )
        {
            UnityEngine.Object.Destroy( dropdownList );
        }

        protected virtual DropdownItem CreateItem( DropdownItem itemTemplate )
        {
            return UnityEngine.Object.Instantiate( itemTemplate );
        }

        protected virtual void DestroyItem( DropdownItem item )
        {
        }

        private DropdownItem AddItem( OptionData data, bool selected, DropdownItem itemTemplate, List<DropdownItem> items )
        {
            DropdownItem dropdownItem = CreateItem( itemTemplate );
            dropdownItem.rectTransform.SetParent( itemTemplate.rectTransform.parent, worldPositionStays: false );
            dropdownItem.gameObject.SetActive( value: true );
            dropdownItem.gameObject.name = "Item " + items.Count + ( ( data.text != null ) ? ( ": " + data.text ) : "" );
            if( dropdownItem.toggle != null )
            {
                dropdownItem.toggle.isOn = false;
            }

            if( ( bool )dropdownItem.text )
            {
                dropdownItem.text.text = data.text;
            }

            if( ( bool )dropdownItem.image )
            {
                dropdownItem.image.sprite = data.image;
                dropdownItem.image.enabled = ( dropdownItem.image.sprite != null );
            }

            items.Add( dropdownItem );
            return dropdownItem;
        }

        private void AlphaFadeList( float duration, float alpha )
        {
            CanvasGroup component = m_Dropdown.GetComponent<CanvasGroup>();
            AlphaFadeList( duration, component.alpha, alpha );
        }

        private void AlphaFadeList( float duration, float start, float end )
        {
            if( !end.Equals( start ) )
            {
                FloatTween floatTween = default( FloatTween );
                floatTween.duration = duration;
                floatTween.startValue = start;
                floatTween.targetValue = end;
                FloatTween info = floatTween;
                info.AddOnChangedCallback( SetAlpha );
                info.ignoreTimeScale = true;
                m_AlphaTweenRunner.StartTween( info );
            }
        }

        private void SetAlpha( float alpha )
        {
            if( ( bool )m_Dropdown )
            {
                CanvasGroup component = m_Dropdown.GetComponent<CanvasGroup>();
                component.alpha = alpha;
            }
        }

        public void Hide()
        {
            if( m_Coroutine != null )
            {
                return;
            }

            if( m_Dropdown != null )
            {
                AlphaFadeList( m_AlphaFadeSpeed, 0f );
                if( IsActive() )
                {
                    m_Coroutine = StartCoroutine( DelayedDestroyDropdownList( m_AlphaFadeSpeed ) );
                }
            }

            if( m_Blocker != null )
            {
                DestroyBlocker( m_Blocker );
            }

            m_Blocker = null;
            Select();
        }

        private IEnumerator DelayedDestroyDropdownList( float delay )
        {
            yield return new WaitForSecondsRealtime( delay );
            ImmediateDestroyDropdownList();
        }

        private void ImmediateDestroyDropdownList()
        {
            for( int i = 0; i < m_Items.Count; i++ )
            {
                if( m_Items[i] != null )
                {
                    DestroyItem( m_Items[i] );
                }
            }

            m_Items.Clear();
            if( m_Dropdown != null )
            {
                DestroyDropdownList( m_Dropdown );
            }

            if( m_AlphaTweenRunner != null )
            {
                m_AlphaTweenRunner.StopTween();
            }

            m_Dropdown = null;
            m_Coroutine = null;
        }

        private void OnToggleItem( Toggle toggle )
        {
            int num = -1;
            Transform transform = toggle.transform;
            Transform parent = transform.parent;
            for( int i = 0; i < parent.childCount; i++ )
            {
                if( parent.GetChild( i ) == transform )
                {
                    num = i - 1;
                    break;
                }
            }

            if( num == -1 )
            {
                m_Values.Clear();
                return;
            }

            if( toggle.isOn )
            {
                m_Values.Add( num );
            }
            else
            {
                m_Values.Remove( num );
            }

            m_OnValuesChanged.Invoke( m_Values );
            RefreshShownValue();
        }
    }
}
