using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FPS : MonoBehaviour {
	Text text;

	float tCounter = 0;
	float minFps;
	float maxFps;

	void Start () {
		text = GetComponent<Text>();
	}

	void LateUpdate() {
		text.text = (1/Time.unscaledDeltaTime).ToString("f2");
	}
}
