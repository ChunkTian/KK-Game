using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{
    public Transform father;
    GameObject source;
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

    public void Choose() {
        gameObject.SetActive(false);
        if (source == null)
        {
            InitItem();
        }
        else
        {
            source.gameObject.SetActive(true);
            Destroy(gameObject); 
        }
    }

    void InitItem()
    {
        GameObject gc = Instantiate(gameObject, father.transform);
        gc.name = name;
        gc.gameObject.SetActive(true);
        gc.GetComponent<CardItem>().source = gameObject;
    }
}
