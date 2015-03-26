using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Row wrapper. Defines a row so that the TileMap grid can be Serialized
/// </summary>

[System.Serializable]
public class RowWrapper  {
	[SerializeField] List<Tile> list = new List<Tile>();
	[SerializeField] protected GameObject m_rowObject;
	[SerializeField] private int m_index = 0;


	public void Add(Tile t){
		list.Add (t);
	}

	public void Add( int _count ){
		for (int i=0; i < _count; i++) {
			list.Add(null);
		}
	}

	public void Insert(int index,Tile t){
		list.Insert (index,t);
	}

	public void RefreshNames(){
		m_rowObject.name = "Row" + m_index.ToString ("000");
		for (int i=0; i < list.Count; i++) {
			Tile t = null;
			try{
				t = list[i];
			}catch(System.Exception e){
				e.ToString(); //Avoid Warning in Console
			}
			if( t != null )
				t.gameObject.name = "Tile_" + t.Column.ToString("000") + "_" + m_index.ToString("000");
		}
	}

	public void CopyTiles(RowWrapper _rowWrapperSource){
		for (int i=0; i < _rowWrapperSource.Count; i++) {
			if( i < list.Count )
				list[i] = _rowWrapperSource [i];
			else
				list.Add(_rowWrapperSource[i]);
		}
	}

	public void Print(){
		for(int i=0; i < list.Count; i++){
			try{
				Debug.Log (list[i].gameObject.name + "at column " + i);
			}catch(System.Exception e){
				Debug.Log( "null at column " + i);
				e.ToString(); //Avoid Warning in Console
			}
		}
	}

	#region GETTERS=SETTERS

	
	public Tile this[int i]
	{
		get { return list[i]; }
		set { list[i] = value; }
	}

	public int Count{
		get{
			return list.Count;
		}
	}

	public GameObject RowObject {
		get {
			return m_rowObject;
		}
		set {
			m_rowObject = value;
		}
	}

	public int Index {
		get {
			return m_index;
		}
		set {
			m_index = value;
			if( m_rowObject != null ){
				//rename all
				RefreshNames();
			}
		}
	}

	#endregion
}
