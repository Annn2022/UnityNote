using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace YGame.Scripts.UI
{
    [Serializable]
    public class Margins
    {
        public float Left, Right, Top, Bottom;

        public Margins(float m)
        {
            Left = Right = Top = Bottom = m;
        }
        public Margins(float x, float y)
        {
            Left = Right = x;
            Top = Bottom = y;
        }
        public Margins(float l, float r, float t, float b)
        {
            Left = l;
            Right = r;
            Top = t;
            Bottom = b;
        }
    }

    public static class UnityEventUtility
    {
        public static void AddListenerOnce(this UnityEvent unityEvent, UnityAction call)
        {
            unityEvent.RemoveListener(call);
            unityEvent.AddListener(call);
        }
        public static void AddListenerOnce<T>(this UnityEvent<T> unityEvent, UnityAction<T> call)
        {
            unityEvent.RemoveListener(call);
            unityEvent.AddListener(call);
        }
    }

    [AddComponentMenu("UI/PageScrollView")]
    [RequireComponent(typeof(ScrollRect))]
    public class PageScrollView : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Vector2 size = new Vector2(400, 250);
        [SerializeField] private float snapSpeed = 10f;
        [SerializeField] private float spacing = 0f;
        [SerializeField] private float thresholdSpeedToSnap = -1f;
        [SerializeField] private ToggleGroup pagination = null;
        [SerializeField] private GameObject togglePrefab = null;
        [SerializeField] private int startingPanel = 0;
        [SerializeField] private UnityEvent<GameObject, float> onTransitionEffects = new UnityEvent<GameObject, float>();
        [SerializeField] private UnityEvent<int> onPanelSelecting = new UnityEvent<int>();
        public UnityEvent<int> onPanelSelected = new UnityEvent<int>();
        [SerializeField] private UnityEvent<int, int> onPanelCentering = new UnityEvent<int, int>();
        [SerializeField] private UnityEvent<int, int> onPanelCentered = new UnityEvent<int, int>();

        [SerializeField] private Margins automaticLayoutMargins = new Margins(0);

        private Vector2 contentSize, prevAnchoredPosition, velocity;
        private bool isDragging, isPressing, isSelected;
        private ScrollRect scrollRect;
        private float releaseSpeed;
        private int accurateCount;

        public int InitPageIndex { get; set; }

        public ScrollRect ScrollRect
        {
            get
            {
                if (scrollRect == null)
                {
                    scrollRect = GetComponent<ScrollRect>();
                }
                return scrollRect;
            }
        }

        public RectTransform Viewport => ScrollRect.viewport;

        public RectTransform[] Panels
        {
            get;
            private set;
        }

        public List<Toggle> Toggles
        {
            get;
            private set;
        }

        public int SelectedPanel
        {
            get;
            private set;
        }
        public int CenteredPanel
        {
            get;
            private set;
        }

        public Vector2 Velocity
        {
            get => velocity;
            set
            {
                ScrollRect.velocity = velocity = value;
                isSelected = false;
            }
        }

        public RectTransform Content => ScrollRect.content;

        public int NumberOfPanels => Content.childCount;

        public void OnBeginDrag(PointerEventData eventData)
        {
            isSelected = false;
            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && onPanelSelecting.GetPersistentEventCount() > 0)
            {
                onPanelSelecting.Invoke(GetNearestPanel());
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressing = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressing = false;
        }

        private void Start()
        {
            if (accurateCount != 0)
                return;

            accurateCount = NumberOfPanels;
            Setup();
            if(InitPageIndex != 0)
            {
                GoToPanel(InitPageIndex);
            }
        }

        private void Update()
        {
            if (accurateCount == 0) return;

            HandleSelectingAndSnapping();
            HandleTransitionEffects();

            GetVelocity();
        }

        private void HandleSelectingAndSnapping()
        {
            if (isSelected)
            {
                if (!(isDragging || isPressing))
                {
                    SnapToPanel();
                }
            }
            else if (!isDragging && (ScrollRect.velocity.magnitude <= thresholdSpeedToSnap || thresholdSpeedToSnap == -1f))
            {
                SelectPanel();
            }
        }

        private void HandleTransitionEffects()
        {
            if (onTransitionEffects.GetPersistentEventCount() == 0) return;

            for (int i = 0; i < accurateCount; i++)
            {
                Vector2 displacement = GetDisplacementFromCenter(i);
                float d = ScrollRect.horizontal ? displacement.x : displacement.y;
                onTransitionEffects.Invoke(Panels[i].gameObject, d);
            }
        }

        private void Setup()
        {
            if (accurateCount == 0 || NumberOfPanels < accurateCount) return;
            Panels = new RectTransform[accurateCount];
            size = Viewport.rect.size;

            for (int i = 0; i < accurateCount; i++)
            {
                Panels[i] = Content.GetChild(i) as RectTransform;
                Panels[i].anchorMin = new Vector2(ScrollRect.horizontal ? 0f : 0.5f, ScrollRect.vertical ? 0f : 0.5f);
                Panels[i].anchorMax = new Vector2(ScrollRect.horizontal ? 0f : 0.5f, ScrollRect.vertical ? 0f : 0.5f);

                float x = (automaticLayoutMargins.Right + automaticLayoutMargins.Left) / 2f - automaticLayoutMargins.Left;
                float y = (automaticLayoutMargins.Top + automaticLayoutMargins.Bottom) / 2f - automaticLayoutMargins.Bottom;
                Vector2 marginOffset = new Vector2(x / size.x, y / size.y);
                Panels[i].pivot = new Vector2(0.5f, 0.5f) + marginOffset;
                Panels[i].sizeDelta = size - new Vector2(automaticLayoutMargins.Left + automaticLayoutMargins.Right, automaticLayoutMargins.Top + automaticLayoutMargins.Bottom);

                float panelPosX = ScrollRect.horizontal ? i * (spacing + 1f) * size.x + (size.x / 2f) : 0f;
                float panelPosY = ScrollRect.vertical ? i * (spacing + 1f) * size.y + (size.y / 2f) : 0f;
                Panels[i].anchoredPosition = new Vector2(panelPosX, panelPosY);
            }

            Content.anchorMin = new Vector2(ScrollRect.horizontal ? 0f : 0.5f, ScrollRect.vertical ? 0f : 0.5f);
            Content.anchorMax = new Vector2(ScrollRect.horizontal ? 0f : 0.5f, ScrollRect.vertical ? 0f : 0.5f);
            Content.pivot = new Vector2(ScrollRect.horizontal ? 0f : 0.5f, ScrollRect.vertical ? 0f : 0.5f);

            float contentWidth = ScrollRect.horizontal ? (accurateCount * (spacing + 1f) * size.x) : size.x;
            float contentHeight = ScrollRect.horizontal ? (accurateCount * (spacing + 1f) * size.y) : size.y;
            Content.sizeDelta = new Vector2(contentWidth, contentHeight);
            float xOffset = ScrollRect.horizontal ? Viewport.rect.width / 2f : 0f;
            float yOffset = ScrollRect.vertical ? Viewport.rect.height / 2f : 0f;
            Vector2 offset = new Vector2(xOffset, yOffset);
            prevAnchoredPosition = Content.anchoredPosition = -Panels[startingPanel].anchoredPosition + offset;
            SelectedPanel = CenteredPanel = startingPanel;

            // Pagination
            if (pagination != null && accurateCount != 0)
            {
                Toggles = pagination.GetComponentsInChildren<Toggle>().ToList();
                Toggles ??= new();

                for (int i = 0; i < accurateCount; i++)
                {
                    if (i >= Toggles.Count)
                    {
                        Toggles.Add(Instantiate(togglePrefab, pagination.transform).GetComponent<Toggle>());
                    }
                    Toggles[i].group = pagination;
                    Toggles[i].gameObject.SetActive(true);
                }

                if (accurateCount < Toggles.Count)
                {
                    for (int i = accurateCount; i < Toggles.Count; i++)
                    {
                        Toggles[i].gameObject.SetActive(false);
                    }
                }

                Toggles[startingPanel].SetIsOnWithoutNotify(true);
                for (int i = 0; i < Toggles.Count; i++)
                {
                    int panelNumber = i;
                    Toggles[i].onValueChanged.AddListenerOnce(delegate (bool isOn)
                    {
                        if (isOn)
                        {
                            GoToPanel(panelNumber);
                        }
                    });
                }
            }
        }

        private void SelectPanel()
        {
            int nearestPanel = GetNearestPanel();
            GoToPanel(nearestPanel);
        }

        private void SnapToPanel()
        {
            float xOffset = ScrollRect.horizontal ? Viewport.rect.width / 2f : 0f;
            float yOffset = ScrollRect.vertical ? Viewport.rect.height / 2f : 0f;
            Vector2 offset = new Vector2(xOffset, yOffset);

            Vector2 targetPosition = -Panels[CenteredPanel].anchoredPosition + offset;
            if (Content.anchoredPosition == targetPosition)
                return;

            Content.anchoredPosition = Vector2.Lerp(Content.anchoredPosition, targetPosition, Time.deltaTime * snapSpeed);

            if (SelectedPanel != CenteredPanel)
            {
                if (GetDisplacementFromCenter(CenteredPanel).magnitude < (Viewport.rect.width / 10f))
                {
                    onPanelCentered.Invoke(CenteredPanel, SelectedPanel);
                    SelectedPanel = CenteredPanel;
                }
            }
            else
            {
                onPanelCentering.Invoke(CenteredPanel, SelectedPanel);
            }
        }

        public void GoToPanel(int panelNumber, bool notify = true)
        {
            CenteredPanel = panelNumber;
            isSelected = true;
            if (notify)
            {
                onPanelSelected.Invoke(SelectedPanel);
            }

            if (pagination != null && Toggles.Count > 0)
            {
                if (notify)
                {
                    Toggles[panelNumber].isOn = true;
                }
                else
                {
                    Toggles[panelNumber].SetIsOnWithoutNotify(true);
                }
            }

            ScrollRect.inertia = false;
        }

        public void GoToPreviousPanel()
        {
            int nearestPanel = GetNearestPanel();
            if (nearestPanel != 0)
            {
                GoToPanel(nearestPanel - 1);
            }
            else
            {
                GoToPanel(nearestPanel);
            }
        }

        public void GoToNextPanel()
        {
            int nearestPanel = GetNearestPanel();
            if (nearestPanel != (NumberOfPanels - 1))
            {
                GoToPanel(nearestPanel + 1);
            }
            else
            {
                GoToPanel(nearestPanel);
            }
        }

        public void AddToFront(GameObject panel)
        {
            Add(panel, 0);
        }

        public void AddToBack(GameObject panel)
        {
            Add(panel, NumberOfPanels);
        }

        public void Add(GameObject panel, int index)
        {
            if (NumberOfPanels != 0 && (index < 0 || index > NumberOfPanels))
            {
                return;
            }

            panel = Instantiate(panel, Content, false);
            panel.transform.SetSiblingIndex(index);

            if (CenteredPanel <= index)
            {
                startingPanel = CenteredPanel;
            }
            else
            {
                startingPanel = CenteredPanel + 1;
            }
            Setup();
        }

        public void RemoveFromFront()
        {
            Remove(0);
        }

        public void RemoveFromBack()
        {
            if (NumberOfPanels > 0)
            {
                Remove(NumberOfPanels - 1);
            }
            else
            {
                Remove(0);
            }
        }

        public void Remove(int index)
        {
            if (NumberOfPanels == 0)
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> There are no panels to remove.", gameObject);
                return;
            }

            if (index < 0 || index > (NumberOfPanels - 1))
            {
                Debug.LogError("<b>[SimpleScrollSnap]</b> Index must be an integer from 0 to " + (NumberOfPanels - 1) + ".", gameObject);
                return;
            }

            DestroyImmediate(Panels[index].gameObject);
            if (CenteredPanel == index)
            {
                if (index == NumberOfPanels)
                {
                    startingPanel = CenteredPanel - 1;
                }
                else
                {
                    startingPanel = CenteredPanel;
                }
            }
            else if (CenteredPanel < index)
            {
                startingPanel = CenteredPanel;
            }
            else
            {
                startingPanel = CenteredPanel - 1;
            }
            Setup();
        }

        private Vector2 GetDisplacementFromCenter(int index)
        {
            return Panels[index].anchoredPosition + Content.anchoredPosition - new Vector2(Viewport.rect.width * (0.5f - Content.anchorMin.x), Viewport.rect.height * (0.5f - Content.anchorMin.y));
        }

        private int GetNearestPanel()
        {
            float[] distances = new float[accurateCount];
            for (int i = 0; i < accurateCount; i++)
            {
                distances[i] = GetDisplacementFromCenter(i).magnitude;
            }

            int nearestPanel = 0;
            float minDistance = Mathf.Min(distances);
            for (int i = 0; i < accurateCount; i++)
            {
                if (minDistance == distances[i])
                {
                    nearestPanel = i;
                    break;
                }
            }
            return nearestPanel;
        }

        private void GetVelocity()
        {
            Vector2 displacement = Content.anchoredPosition - prevAnchoredPosition;
            velocity = displacement / Time.deltaTime;
            prevAnchoredPosition = Content.anchoredPosition;
        }

        public void SetPage(int page)
        {
            accurateCount = page;
            Setup();
        }
    }
}
