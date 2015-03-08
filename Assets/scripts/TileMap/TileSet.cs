using UnityEngine;
using System.Collections;

public class TileSet : ScriptableObject {
	public Transform[] prefabs = new Transform[0];

	public Transform GetTileByName(string _name){
		for (int i=0; i < prefabs.Length; i ++) {
			if( prefabs[i].gameObject.name == _name )
				return prefabs[i];
		}
		return null;
	}
}
