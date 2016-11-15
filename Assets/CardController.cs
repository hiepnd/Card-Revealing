using UnityEngine;
using System.Collections;

public class CardController : MonoBehaviour {
	public Card card;

	Vector3 dir;
	float magnitude;
	Vector3 firstPos;
	bool tracking;

	void Update () {
		if (tracking && Input.GetMouseButtonUp(0)) {
			tracking = false;

			StartCoroutine (AnimateOut((Input.mousePosition - firstPos) * 0.01f, 0.2f));
		}	

		if (!tracking && Input.GetMouseButtonDown(0)) {
			tracking = true;
			firstPos = Input.mousePosition;
		}

		if (tracking) {
			var p = Input.mousePosition;
			card.UpdateMesh ((p - firstPos) * 0.01f);
		}
	}

	IEnumerator AnimateOut (Vector3 intrude, float duration) {
		if (intrude.magnitude < Mathf.Epsilon) {
			card.UpdateMesh (Vector3.zero);
			yield break;
		}

		float elapsed = 0;
		while (elapsed < duration) {
			float d = Mathf.Lerp (intrude.magnitude, 0, elapsed/duration);
			card.UpdateMesh (intrude.normalized * d);
			elapsed += Time.deltaTime;
			yield return null;
		}
		card.UpdateMesh (Vector3.zero);
	}
}
