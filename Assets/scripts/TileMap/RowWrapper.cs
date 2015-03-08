using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Row wrapper. Defines a row so that the TileMap grid can be Serialized
/// </summary>

[System.Serializable]
public class RowWrapper  {
	[SerializeField] List<Tile> list = new List<Tile>();

	public Tile this[int i]
	{
		get { return list[i]; }
		set { list[i] = value; }
	}

	public void Add(Tile t){
		list.Add (t);
	}

	public void Insert(int index,Tile t){
		list.Insert (index,t);
	}
}
