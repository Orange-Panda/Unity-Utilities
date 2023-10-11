using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LMirman.Utilities
{
	/// <summary>
	/// By default a <see cref="Selectable"/> does not cause the <see cref="ScrollRect"/> to update it's value. 
	/// This script, when attached to a <see cref="ScrollRect"/>, will update the <see cref="scrollRect"/>'s <see cref="ScrollRect.verticalNormalizedPosition"/> to include the rect of
	/// a <see cref="Selectable"/> that is nested within <see cref="ScrollRect.content"/> when a new <see cref="Selectable"/> is selected.
	/// </summary>
	[RequireComponent(typeof(ScrollRect)), PublicAPI]
	public class AutoScroll : MonoBehaviour
	{
		[SerializeField, Tooltip("Should the auto scroll component control the horizontal (left-right) scroll position?")]
		private bool affectHorizontal = true;
		[SerializeField, Tooltip("Should the auto scroll component control the vertical (up-down) scroll position?")]
		private bool affectVertical = true;
		
		private List<Selectable> candidates;
		private ScrollRect scrollRect;
		private GameObject lastSelection;
		private int refreshCount;

		private readonly Vector3[] selectableWorld = new Vector3[4];
		private readonly Vector3[] scrollWorld = new Vector3[4];
		
		public bool AffectHorizontal { get => affectHorizontal; set => affectHorizontal = value; }
		public bool AffectVertical { get => affectVertical; set => affectVertical = value; }

		private void Awake()
		{
			scrollRect = GetComponent<ScrollRect>();
		}

		private void Start()
		{
			RefreshList();
		}

		private void OnEnable()
		{
			SetDirty(2);
		}

		private void Update()
		{
			if (refreshCount > 0)
			{
				refreshCount = Mathf.Clamp(refreshCount - 1, 0, 10);
				RefreshList();
			}
		}

		private void LateUpdate()
		{
			if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject == lastSelection)
			{
				return;
			}

			lastSelection = EventSystem.current.currentSelectedGameObject;
			Selectable selectable = lastSelection.GetComponentInChildren<Selectable>();
			FocusSelectable(selectable);
		}

		/// <summary>
		/// Adjust the <see cref="scrollRect"/> position to show the <paramref name="selectable"/> if it is a child of the scroll rect and outside of its viewport. 
		/// </summary>
		/// <remarks>
		/// This is automatically invoked when a new selectable in the <see cref="scrollRect"/> is selected so you shouldn't need to call this in most cases.
		/// </remarks>
		/// <param name="selectable">The selectable to focus the <see cref="scrollRect"/> ensure is visible within its viewport.</param>
		public void FocusSelectable(Selectable selectable)
		{
			if (selectable == null || !candidates.Contains(selectable) || (!affectHorizontal && !affectVertical))
			{
				return;
			}

			// Store the world positions of the target selectable and its scroll view in Vector3 arrays.
			// Array index reference: 0 - bottom left, 1 - top left, 2 - top right, 3 - bottom right
			scrollRect.content.GetWorldCorners(scrollWorld);
			selectable.GetComponent<RectTransform>().GetWorldCorners(selectableWorld);
			SetVertical();
			SetHorizontal();
		}

		private void SetVertical()
		{
			if (!affectVertical || !scrollRect.vertical)
			{
				return;
			}
			
			// Determine the range in which the current content is visible
			float scrollSize = Mathf.Clamp01(scrollRect.viewport.rect.height / scrollRect.content.rect.height);
			float t = scrollRect.verticalNormalizedPosition;
			Vector2 visibleRange = new Vector2(Mathf.Lerp(0, 1 - scrollSize, t), Mathf.Lerp(scrollSize, 1, t));
			
			// Padding that will add a little extra space forward the selectable when it is made visible.
			float padding = scrollSize / 40f;
			
			// Calculate the height of the scroll content and the top/bottom offsets of the selectable within the scroll rect.
			float scrollHeight = Mathf.Abs(scrollWorld[1].y - scrollWorld[0].y);
			float topDistance = Mathf.Abs(scrollWorld[1].y - selectableWorld[1].y);
			float topNormalizedPosition = 1 - (topDistance / scrollHeight);
			float botDistance = Mathf.Abs(scrollWorld[1].y - selectableWorld[0].y);
			float botNormalizedPosition = 1 - (botDistance / scrollHeight);
			
			// Check if top is within range
			if (topNormalizedPosition > visibleRange.y)
			{
				scrollRect.verticalNormalizedPosition = Mathf.InverseLerp(scrollSize, 1, topNormalizedPosition + padding);
			}

			// Check if bottom is within range
			if (botNormalizedPosition < visibleRange.x)
			{
				scrollRect.verticalNormalizedPosition = Mathf.InverseLerp(0, 1 - scrollSize, botNormalizedPosition - padding);
			}
		}

		private void SetHorizontal()
		{
			if (!affectHorizontal || !scrollRect.horizontal)
			{
				return;
			}
			
			// Determine the range in which the current content is visible
			float scrollSize = Mathf.Clamp01(scrollRect.viewport.rect.width / scrollRect.content.rect.width);
			float t = scrollRect.horizontalNormalizedPosition;
			Vector2 visibleRange = new Vector2(Mathf.Lerp(0, 1 - scrollSize, t), Mathf.Lerp(scrollSize, 1, t));

			// Padding that will add a little extra space forward the selectable when it is made visible.
			float padding = scrollSize / 40f;
			
			// Calculate the width of the scroll content and the left/right offsets of the selectable within the scroll rect.
			float scrollWidth = Mathf.Abs(scrollWorld[2].x - scrollWorld[1].x);
			float leftDistance = Mathf.Abs(scrollWorld[1].x - selectableWorld[1].x);
			float leftNormalizedPosition = leftDistance / scrollWidth;
			float rightDistance = Mathf.Abs(scrollWorld[1].x - selectableWorld[2].x);
			float rightNormalizedPosition = rightDistance / scrollWidth;

			// Check if right is within range
			if (rightNormalizedPosition > visibleRange.y)
			{
				scrollRect.horizontalNormalizedPosition = Mathf.InverseLerp(scrollSize, 1, rightNormalizedPosition + padding);
			}

			// Check if left is within range
			if (leftNormalizedPosition < visibleRange.x)
			{
				scrollRect.horizontalNormalizedPosition = Mathf.InverseLerp(0, 1 - scrollSize, leftNormalizedPosition - padding);
			}
		}

		/// <summary>
		/// Immediately refresh the <see cref="candidates"/> list.
		/// </summary>
		/// <remarks>
		/// In most cases you should just use <see cref="SetDirty"/>.
		/// However, if your application requires an immediate refresh (such as components being added and immediately being selected) then this function gives you the ability to immediately refresh the list.
		/// </remarks>
		public void RefreshList()
		{
			candidates = new List<Selectable>(scrollRect.content.GetComponentsInChildren<Selectable>(true));
		}

		/// <summary>
		/// Mark the list as dirty, letting the autoscroll know that the list of children selectables will be changed in the next <see cref="count"/> frames.
		/// </summary>
		/// <param name="count">The number of frames to refresh the list for, this should practically only have a value from 1-3</param>
		public void SetDirty(int count = 1)
		{
			count = Mathf.Clamp(count, 1, 10);
			refreshCount = Mathf.Max(count, refreshCount);
		}
	}
}