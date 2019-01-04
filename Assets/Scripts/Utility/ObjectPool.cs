using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {
	GameObject container;
	List<GameObject> pool;
	int maxNumOfObjects;
	int currentObject;

	public ObjectPool(GameObject obj, int max, Transform parent){
		container = new GameObject();
		container.name = "Container";
		container.transform.SetParent(parent);
		currentObject = 0;
		maxNumOfObjects = max;
		pool = new List<GameObject>();
		for(int i = 0; i < maxNumOfObjects; i++){
			GameObject newObj = GameObject.Instantiate(obj, container.transform);
			pool.Add(newObj);
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
