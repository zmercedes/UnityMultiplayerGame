using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {
	List<GameObject> pool;
	int maxNumOfObjects;
	int currentObject;

	public ObjectPool(GameObject obj, int max){
		currentObject = 0;
		maxNumOfObjects = max;
		pool = new List<GameObject>();
		for(int i = 0; i < maxNumOfObjects; i++){
			GameObject tmp = GameObject.Instantiate(obj);
			pool.Add(tmp);
		}
	}

	public GameObject Next(){
		GameObject current = pool[currentObject];
		currentObject++;
		if(currentObject >= maxNumOfObjects)
			currentObject = 0;

		return current;
	}
}
