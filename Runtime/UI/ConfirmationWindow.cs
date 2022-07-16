using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfirmationWindow : MonoBehaviour
{
	[SerializeField]
	private Selectable focusSelectable;
	[SerializeField]
	private TMP_Text descriptionText;

	private Action onSubmit;
	private Action onCancel;
	
	public event Action<Request> OnInitialize = delegate {  };

	private void OnEnable()
	{
		UIFunctions.AddFocus(this);
	}

	private void OnDisable()
	{
		UIFunctions.RemoveFocus(this);
	}

	public void Submit()
	{
		onSubmit.Invoke();
		Destroy(gameObject);
	}

	public void Cancel()
	{
		onCancel.Invoke();
		Destroy(gameObject);
	}

	internal void Initialize(Request request)
	{
		onSubmit = request.onSubmit;
		onCancel = request.onCancel;
		EventSystem.current.SetSelectedGameObject(focusSelectable.gameObject);
		descriptionText.text = request.description;
		OnInitialize.Invoke(request);
	}

	public class Request
	{
		public Action onSubmit;
		public Action onCancel;
		public Canvas canvas;
		public string description;
		public string submitText;
		public string cancelText;
	}
}