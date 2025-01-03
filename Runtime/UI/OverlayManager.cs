using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LMirman.Utilities.UI
{
	/// <summary>
	/// Handles an overlay system which is designed to display user input ui during the application.
	/// Useful for interfaces such as inventory, pause menu, game overs, etc.
	/// </summary>
	[PublicAPI]
	public class OverlayManager : MonoBehaviour
	{
		[Tooltip("The transform to use as the parent for created overlays. Will use this transform if not specified.")]
		[SerializeField]
		protected Transform desiredOverlayParent;
		[SerializeField]
		protected OverlayEntry[] overlays = Array.Empty<OverlayEntry>();

		protected Dictionary<string, OverlayEntry> lookup = new Dictionary<string, OverlayEntry>();
		protected Transform FallbackTransform => desiredOverlayParent ? desiredOverlayParent : transform;

		/// <summary>
		/// The current interface set active via <see cref="SetActiveInterface(OverlayInterface)"/>.
		/// Null if there is no active interface.
		/// </summary>
		public OverlayInterface ActiveInterface { get; set; }

		/// <summary>
		/// Invoked when an interface is set active via <see cref="SetActiveInterface(OverlayInterface)"/>.
		/// </summary>
		public event Action InterfaceOpened = delegate { };

		protected virtual void Awake()
		{
			lookup = new Dictionary<string, OverlayEntry>();
			foreach (OverlayEntry entry in overlays)
			{
				lookup.Add(entry.key, entry);
			}
		}

		protected virtual void Start()
		{
			ClearInterfaces();

			// Load some overlays immediately if their functionality needs to exist in the scene before its first open request.
			foreach (OverlayEntry overlay in overlays)
			{
				if (overlay.loadImmediately && !overlay.Loaded)
				{
					overlay.LoadOverlay(overlay.DetermineParent(FallbackTransform));
				}
			}
		}

		//Avoid renaming, used by UI buttons.
		// ReSharper disable once UnusedMember.Global
		/// <summary>
		/// Ask the current <see cref="ActiveInterface"/> to close using <see cref="OverlayInterface.RequestClose"/>.
		/// </summary>
		/// <remarks>
		/// This is useful for close buttons in UI that are independent of a particular overlay.
		/// </remarks>
		public void SendCloseRequest()
		{
			if (ActiveInterface)
			{
				ActiveInterface.RequestClose();
			}
		}

		/// <summary>
		/// Get an interface reference from it's assigned key in the <see cref="lookup"/>.
		/// </summary>
		/// <param name="key">The id defined at the <see cref="OverlayEntry.key"/></param>
		/// <param name="validate">When true, throw an error if no interface was found with that key.</param>
		/// <returns>The <see cref="OverlayInterface"/> that was found. Null if no entry existed for that key.</returns>
		public OverlayInterface GetInterface(string key, bool validate = false)
		{
			if (!lookup.TryGetValue(key, out OverlayEntry entry))
			{
				if (validate)
				{
					Debug.LogError($"Tried to get overlay \"{key}\" but no such overlay was present.");
				}

				return null;
			}

			if (!entry.Loaded)
			{
				entry.LoadOverlay(entry.DetermineParent(FallbackTransform));
			}

			return entry.Overlay;
		}

		/// <summary>
		/// Set the active interface directly from its component reference.
		/// </summary>
		/// <param name="value">The interface to set active. Should not be null, use <see cref="ClearInterfaces"/> if you intend to not open any interface.</param>
		public void SetActiveInterface(OverlayInterface value)
		{
			CloseOpenInterfaces();
			ActiveInterface = value;
			value.Open();
			InterfaceOpened.Invoke();
		}

		/// <summary>
		/// Set the active interface based on it's assigned key in the <see cref="lookup"/>.
		/// </summary>
		/// <param name="key">The id defined at the <see cref="OverlayEntry.key"/></param>
		public void SetActiveInterface(string key)
		{
			if (lookup.TryGetValue(key, out OverlayEntry entry))
			{
				if (!entry.Loaded)
				{
					entry.LoadOverlay(entry.DetermineParent(FallbackTransform));
				}

				SetActiveInterface(entry.Overlay);
			}
			else
			{
				Debug.LogError($"Tried to set overlay \"{key}\" but no such overlay was present.");
			}
		}

		/// <summary>
		/// Close all open interfaces.
		/// </summary>
		/// <remarks>
		/// For internal use. If you need to close all interfaces use <see cref="ClearInterfaces"/> instead.
		/// </remarks>
		private void CloseOpenInterfaces()
		{
			foreach (OverlayEntry entry in lookup.Values)
			{
				if (entry.Loaded && entry.Overlay.IsOpen)
				{
					entry.Overlay.Close();
				}
			}
		}

		/// <summary>
		/// <b>Immediately</b> close all interfaces that are open.
		/// </summary>
		/// <remarks>
		/// Beware: This will skip the process of <see cref="OverlayInterface.RequestClose"/> and immediately close the interfaces.
		/// </remarks>
		public void ClearInterfaces()
		{
			CloseOpenInterfaces();
			ActiveInterface = null;
		}

		/// <summary>
		/// A particular overlay that the <see cref="OverlayManager"/> is responsible for.
		/// </summary>
		[Serializable]
		public class OverlayEntry
		{
			[Tooltip("The id for this overlay. Used to set it active when there is no direct reference to the interface component.")]
			public string key;
			[Tooltip("The prefab to instantiate for the overlay at runtime.")]
			public GameObject prefab;
			[Tooltip("Usually overlays are instantiated when they are first opened. Enable this to instantiate them when the overlay manager is first started.")]
			public bool loadImmediately;
			[Tooltip("The parent this overlay would prefer to be instantiated as a child of. Only used through DetermineParent() functionality.")]
			[SerializeField]
			private Transform desiredParent;

			/// <summary>
			/// When true signifies that the <see cref="prefab"/> has been instantiated and is ready to be used.
			/// </summary>
			public bool Loaded { get; private set; }

			/// <summary>
			/// The <see cref="prefab"/> instance that has been instantiated.
			/// </summary>
			public GameObject InstanceObject { get; private set; }

			public OverlayInterface Overlay { get; private set; }

			public Transform DetermineParent(Transform fallbackParent)
			{
				return desiredParent != null ? desiredParent : fallbackParent;
			}

			/// <summary>
			/// Spawn the overlay instance if it has not already been loaded.
			/// </summary>
			/// <param name="parent">The transform of the ui element to make the parent of the created overlay prefab.</param>
			public void LoadOverlay(Transform parent)
			{
				if (Loaded)
				{
					return;
				}

				InstanceObject = Instantiate(prefab, parent);
				Overlay = InstanceObject.GetComponent<OverlayInterface>();
				Loaded = true;

				if (Overlay == null)
				{
					Debug.LogError($"Overlay \"{InstanceObject.name}\" does not have a OverlayInterface component.");
					return;
				}

				Overlay.SetVisualActive(false);
			}
		}
	}
}