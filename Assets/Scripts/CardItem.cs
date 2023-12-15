using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public int type = 0;
    public GameObject source;
    public Action<CardItem> ChooseAction;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate ()
        {
            Choose();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Choose()
    {
        if (ChooseAction != null)
        {
            ChooseAction(this);
        }
        if (type == 0)
        {
            gameObject.SetActive(false);
        }
        if (type == 1)
        {
            source.gameObject.SetActive(true);
            Destroy(gameObject);
        }
        if (type == 2)
        {
            Destroy(gameObject);
        }
    }
}
