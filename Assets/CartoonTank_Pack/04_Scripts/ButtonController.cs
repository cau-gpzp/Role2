using UnityEngine;
using System.Collections;

public enum BUTTON_TYPE
{
	NEXT,
	PREV,
	COLOR,
    MARK,
    PARTS,
    CANNON
}

public class ButtonController : MonoBehaviour {

	public BUTTON_TYPE _buttonType;
    public TankPackManager _tankPackManager;
	private Vector3 _scale;

	void Start()
	{
		_scale = transform.localScale;
	}

	void OnMouseDown()
	{
		transform.localScale = _scale * 1.1f;
	}

	void OnMouseUp()
	{
		transform.localScale = _scale;
	}

	void OnMouseUpAsButton()
	{
		transform.localScale = _scale;
		CheckButton ();
	}

	void CheckButton()
	{
		switch(_buttonType)
        {
            case BUTTON_TYPE.NEXT:
                _tankPackManager.NextCannon();
                break;
            case BUTTON_TYPE.PREV:
                _tankPackManager.PrevCannon();
                break;
            case BUTTON_TYPE.COLOR:
                _tankPackManager.NextColor();
                break;
            case BUTTON_TYPE.MARK:
                _tankPackManager.NextMarker();
                break;
            case BUTTON_TYPE.PARTS:
                _tankPackManager.ToggleParts();
                break;
        }
	}
}
