using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour {

	public Sprite[] m_Sprites;
	public float timeDuration = 0.1f;

	// Use this for initialization
	void OnEnable () {
		StartCoroutine(play());
	}

	IEnumerator play()
	{
		int i = 0;

		while(i < m_Sprites.Length)
		{
			transform.GetComponent<Image>().sprite = m_Sprites[i];
			i++;
			yield return new WaitForSeconds(timeDuration);

			if (i >= m_Sprites.Length)
				i = 0;
		}
	}

}
