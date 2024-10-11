using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingUpByExp : MonoBehaviour
{
    public GameObject _char;
    public float _scaleX;
    public float _scaleY;
    public float _scaleZ;


    public GameObject inputTextField;
    public float _exp;

    void Start()
    {
        _char = this.GetComponent<GrowingUpByExp>().gameObject;
        _scaleX = _char.transform.localScale.x;
        _scaleY = _char.transform.localScale.y;
        _scaleZ = _char.transform.localScale.z;
    }


    void Update()
    {
        _char = this.GetComponent<GrowingUpByExp>().gameObject;
        _scaleX = _char.transform.localScale.x;
        _scaleY = _char.transform.localScale.y;
        _scaleZ = _char.transform.localScale.z;

        _exp = inputTextField.GetComponent<InputCommands>().exp;

        //if (_exp == 2) _char.transform.localScale = new Vector3((float)(_scaleX + 0.1), (float)(_scaleY + 0.1), (float)(_scaleZ + 0.1));
        //if (_exp == 5) _char.transform.localScale = new Vector3((float)(_scaleX + 0.1), (float)(_scaleY + 0.1), (float)(_scaleZ + 0.1));
        //if (_exp == 8) _char.transform.localScale = new Vector3((float)(_scaleX + 0.1), (float)(_scaleY + 0.1), (float)(_scaleZ + 0.1));
    }
}
