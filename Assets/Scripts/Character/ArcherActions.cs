using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ArcherActions : UnitActions {

	public override void Awake() {
		base.Awake();
	}

	public override IEnumerator Attack(){
		yield return null;
	}
}