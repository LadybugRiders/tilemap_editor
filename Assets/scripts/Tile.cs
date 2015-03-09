using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	[SerializeField] protected int m_type = 0;

	[HideInInspector][SerializeField]
	int m_row = 0;
	[HideInInspector][SerializeField]
	int m_column = 0;

	SpriteRenderer m_renderer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Resize(float cellSize){
		//Compute scale to apply
		float sclModifier = ( GetPPU() / 100 );
		float newScaleX = sclModifier* cellSize / m_renderer.sprite.texture.width;
		float newScaleY = sclModifier* cellSize / m_renderer.sprite.texture.height;

		this.transform.localScale = new Vector3 (newScaleX,newScaleY,1);
	}

	public bool Compare(Tile tile){
		bool eq = true;
		eq = eq && (tile.Type == this.m_type);
		//eq = eq && (tile.Row == this.m_row) && (tile.Column == this.m_column);
		eq = eq && (tile.GetComponent<SpriteRenderer> ().sprite.texture.name == gameObject.GetComponent<SpriteRenderer> ().sprite.texture.name);
		return eq;
	}

	#region GETTERS-SETTERS

	public int Row {
		get {
			return m_row;
		}
		set {
			m_row = value;
		}
	}

	public int Column {
		get {
			return m_column;
		}
		set {
			m_column = value;
		}
	}

	public float GetPPU(){
		if( m_renderer == null )
			m_renderer = this.gameObject.GetComponent<SpriteRenderer> ();
		//Pixel Per Unit
		return  m_renderer.sprite.rect.width / m_renderer.sprite.bounds.size.x;
	}


	public int Type {
		get {
			return m_type;
		}
	}
	#endregion
}
