using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CardTest))]
public class SomethingEditor : Editor {
	

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();

		CardTest card = (CardTest)target;

		if (GUILayout.Button("Apply")) {
			card.Apply ();
		}
	}
}
