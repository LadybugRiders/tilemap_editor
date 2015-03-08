using UnityEngine;
using System.Collections;

public class TileSet : ScriptableObject {
	public Transform[] tilesPrefabs = new Transform[0];

	public Transform GetTileByName(string _name){
		for (int i=0; i < tilesPrefabs.Length; i ++) {
			if( tilesPrefabs[i].gameObject.name == _name )
				return tilesPrefabs[i];
		}
		return null;
	}
}
