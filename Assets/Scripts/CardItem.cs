using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardTypeEnum
{
    PlloCard,
    ChooseCard,
    ChoosenCard
}

public class CardItem : MonoBehaviour
{
    public CardTypeEnum type = 0;
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
    }
}
