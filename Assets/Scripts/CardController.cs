using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Transform father;
    public Transform choosefather;
    public Transform tempfather;
    public GameObject sampleitem;
    public Dictionary<string, CardItem> cards;
    public List<CardItem> choosecards;
    public List<CardItem> tempcards;

    public string chooses;

    public bool finish = true;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        StartCoroutine(LoadSetting());
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init()
    {
        cards = new Dictionary<string, CardItem>();
        for (int i = 1; i < 14; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                string na = i + "" + j;
                cards.Add(na, InitItem(na, 0, father));
            }
        }

        for (int i = 14; i < 16; i++)
        {
            string na = i + "0";
            cards.Add(na, InitItem(na, 0, father));
        }
    }

    public void Pass()
    {
        string na = "00";
        tempcards.Add(InitItem(na, 2, tempfather));
    }

    CardItem InitItem(string na, int type, Transform _father)
    {
        GameObject gc = Instantiate(sampleitem, _father);
        gc.name = na;
        Texture2D imageicon = Resources.Load("image/" + na) as Texture2D;
        Sprite iconsprite = Sprite.Create(imageicon, new Rect(0, 0, imageicon.width, imageicon.height), new Vector2(0.5f, 0.5f));
        gc.transform.Find("Image").GetComponent<Image>().sprite = iconsprite;
        CardItem cardItem = gc.GetComponent<CardItem>();
        cardItem.ChooseAction = CardChoose;
        cardItem.type = type;
        gc.gameObject.SetActive(true);
        return cardItem;
    }

    public void CardChoose(CardItem cardItem)
    {
        if (cardItem.type == 0)
        {
            CardItem _cardItem = InitItem(cardItem.name, 1, tempfather);
            tempcards.Add(_cardItem);
            _cardItem.source = cardItem.gameObject;
        }
    }

    public void Suiji()
    {
        StartCoroutine(Show());
    }

    private IEnumerator Show()
    {
        if (finish == true)
        {
            finish = false;
            List<string> _chooses = String2List(chooses);
            for (int i = 0; i < _chooses.Count; i++)
            {
                yield return new WaitForSeconds(0.2f);
                
                CardItem _cardItem = InitItem(_chooses[i], 3, choosefather);
                if (cards.ContainsKey(_chooses[i]))
                {
                    GameObject _source = cards[_chooses[i]].gameObject;
                    _cardItem.source = _source;
                    _source.SetActive(false);
                }

                choosecards.Add(_cardItem);
            }
            finish = true;
        }
    }

    public void Clean()
    {
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        for (int i = 0; i < choosecards.Count; i++)
        {
            if (choosecards[i].GetComponent<CardItem>().source)
            {
                choosecards[i].GetComponent<CardItem>().source.gameObject.SetActive(true);
            }
            Destroy(choosecards[i].gameObject);
            yield return new WaitForSeconds(0.1f);
        }
        choosecards.Clear();
        for (int i = 0; i < tempcards.Count; i++)
        {
            if (tempcards[i].GetComponent<CardItem>().source)
            {
                tempcards[i].GetComponent<CardItem>().source.gameObject.SetActive(true);
            }
            Destroy(tempcards[i].gameObject);
            yield return new WaitForSeconds(0.1f);
        }
        tempcards.Clear();
    }


    public void Jisuan()
    {
        List<string> _chooses = String2List(chooses);
        bool choosetrue = true;
        for (int i = 0; i < choosefather.childCount; i++)
        {
            choosetrue &= (int.Parse(_chooses[i]) / 10 == int.Parse(choosefather.GetChild(i).name) / 10);
        }

        Debug.Log(choosetrue ? "正确" : "错误");
    }

    [System.Obsolete]
    IEnumerator LoadSetting()
    {
        // 创建一个UnityWebRequest对象
        UnityWebRequest request = UnityWebRequest.Get("https://github.com/ChunkTian/KK-Game/releases/download/v.1.0.0/setting.json");

        // 设置请求超时时间为10秒
        request.timeout = 60;

        // 发送网络请求
        yield return request.SendWebRequest();

        // 如果请求成功
        if (!request.isNetworkError)
        {
            // 处理请求结果
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            // 如果请求超时，输出错误信息
            if (request.error == "Request timeout")
            {
                Debug.Log("Request timeout");
            }
            else
            {
                // 如果请求发生其他错误，输出错误信息
                Debug.Log(request.error);
            }
        }
    }

    private List<string> String2List(string str)
    {
        List<string> data = new List<string>();
        foreach (var item in str.Split("|"))
        {
            string[] values = item.Split(",");
            foreach (var val in values)
            {
                data.Add(val);
            }
        }
        return data;
    }

    [ContextMenu("Record")]
    public void Record()
    {
        string str = "";
        for (int i = 0; i < tempfather.childCount; i++)
        {
            str += tempfather.GetChild(i).name + ",";
        }
        Debug.Log(str);
    }
}
