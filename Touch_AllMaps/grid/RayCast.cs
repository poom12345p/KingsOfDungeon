using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour {
	public Material highlight;
	public Material defalse;

	private string selectableTag = "selectable";

	public Camera cam;

	private Transform _selection;

	public float fadeSpeed = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;

		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
		{
			var selection = hit.transform;
			if (_selection != selection)
			{
				if (_selection != null)
				{
					foreach (Transform s in _selection.GetComponentInChildren<Transform>())
					{
						var selectionRenderer = s.GetComponent<Renderer>();
						selectionRenderer.material = highlight;
						StartCoroutine("fadeIn", selectionRenderer);
					}
					_selection = null;
				}

			}
			if (selection.CompareTag(selectableTag))
			{
				foreach (Transform s in selection.GetComponentInChildren<Transform>())
				{
					if (s.GetComponent<Renderer>() == null)
					{
						foreach (Transform sChildren in s.GetComponentInChildren<Transform>())
						{
							var sChildRenderer = sChildren.GetComponent<Renderer>();
							sChildRenderer.material = highlight;
							StartCoroutine("fadeOut", sChildRenderer);
						}
						continue;
					}
					var selectionRenderer = s.GetComponent<Renderer>();
					selectionRenderer.material = highlight;
					StartCoroutine("fadeOut", selectionRenderer);
				}
				_selection = selection;
			}
		}
	}
	IEnumerator fadeIn(Renderer rend)
	{
		Color c = rend.material.color;
		for (float f = 0.05f; f <= 1; f += 0.05f)
		{
			c.a = f;
			rend.material.color = c;
			yield return new WaitForSeconds(0.05f);
		}

	}
	IEnumerator fadeOut(Renderer rend)
	{
		Color c = rend.material.color;
		for (float f = 1; f >= 0; f -= 0.05f)
		{
			c.a = f;
			rend.material.color = c;
			yield return new WaitForSeconds(0.01f);
		}
	}
}
