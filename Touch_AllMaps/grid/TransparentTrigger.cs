using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentTrigger : MonoBehaviour {
	public GameObject[] TargetToFade;//should be gameobject that all child have renderer;
	public bool tiggered = false;
	private Collider col;
	public Material highlight;
	public Material defalse;
	public GameObject dontFateTrigger;
	public List<Collider> playerInside;
	bool dontFate;
	public List<Transform> faded = new List<Transform>();

	public int count = 0;

	// Use this for initialization
	void Start () {
		col = GetComponent<Collider>();
		col.isTrigger = true;
		if (dontFateTrigger != null)
		{
			dontFate = dontFateTrigger.activeInHierarchy;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !dontFate)
		{
			if (!playerInside.Contains(other))
			{
				playerInside.Add(other);
			}
			if (!tiggered)
			{
				StopAllCoroutines();
				tiggered = true;
				//fadeOut
				foreach (GameObject a in TargetToFade)
				{
					if (a.activeSelf)
					{
						foreach (Transform atransform in a.GetComponent<Transform>())
						{
							foreach (Transform achildren in atransform.GetComponentsInChildren<Transform>())
							{
								faded.Add(achildren);
								var arend = achildren.GetComponent<Renderer>();
								if (arend != null)
								{
									StartCoroutine(fadeOut(arend, achildren));
								}
							}
						}
					}
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player" && !dontFate)
		{
			playerInside.Remove(other);
			if (playerInside.Count == 0 && tiggered)
			{
				tiggered = false;
				//fadeIn
				StopAllCoroutines();
				foreach (Transform a in faded)
				{
					a.gameObject.SetActive(true);
					var arend = a.GetComponent<Renderer>();
					if (arend != null)
					{
						StartCoroutine(fadeIn(arend, a));
					}
				}
				//foreach (GameObject a in TargetToFade)
				//{
				//	foreach (Transform atransform in a.GetComponent<Transform>())
				//	{
				//		foreach (Transform achildren in atransform.GetComponentsInChildren<Transform>())
				//		{
				//			var arend = achildren.GetComponent<Renderer>();
				//			if (arend != null)
				//			{
				//				StartCoroutine(fadeIn(arend, a));
				//			}
				//		}
				//	}
				//}
			}
			
		}
	}
	


	IEnumerator fadeIn(Renderer rend,Transform achild)
	{
		Color c = rend.material.color;
		if (c.a <= 0.5)
		{
			for (float f = 0f; f <= 1; f += 0.01f)
			{
				c.a = f;
				rend.material.color = c;
				if (f > 0.99) rend.material = defalse;
				yield return new WaitForSeconds(0.01f);
			}
		}
		else
		{
			rend.material = defalse;
		}

	}

	IEnumerator fadeOut(Renderer rend,Transform achild)
	{
		rend.material = highlight;
		Color c = rend.material.color;
		if (c.a == 1)
		{
			for (float f = 1f; f > 0; f -= 0.01f)
			{
				c.a = f;
				rend.material.color = c;
				if (f < 0.05)
				{
					achild.gameObject.SetActive(false);
				}
				yield return new WaitForSeconds(0.01f);
			}
		}
		else
		{
			c.a = 0;
		}
	}
}
