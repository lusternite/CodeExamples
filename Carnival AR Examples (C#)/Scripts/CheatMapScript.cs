using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

public class CheatMapScript : MonoBehaviour {

    public Sprite[] _sprites;
    int i = 1;
    float _f = 0.0f;
    public float _fff = 0.2f;

	// Use this for initialization
	void Start () {

        gameObject.GetComponent<Image>().sprite = _sprites[0];
    }
	
	// Update is called once per frame
	void Update () {
        _f += Time.deltaTime;
        Debug.Log(_f);
        if (_f >= _fff)
        {
            _f = 0;
            ++i;
            if (i == 17)
            {
                i = 0;
            }
            gameObject.GetComponent<Image>().sprite = _sprites[i];
        }
    }
}
