using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum ModelEnum
{
    Random,
    Optional,
    Choosen
}

public class CardController : MonoBehaviour
{
    public Color[] colors;
    public Transform father;
    public Transform choosefather;
    public Transform tempfather;
    public GameObject sampleitem;
    public Dictionary<string, CardItem> cards;
    public List<CardItem> choosecards;
    public List<CardItem> tempcards;
    public Transform dialog;

    public string chooses;
    public List<string> mychooses;

    public bool finish = true;

    public ModelEnum currentModel;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        StartCoroutine(LoadSetting());
        Init();
        Suiji();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Suiji();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SubmitRecord();
        }
    }

    private void Init()
    {
        currentModel = ModelEnum.Optional;
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
        tempcards.Add(InitItem(na, CardTypeEnum.ChooseCard, tempfather));
    }

    CardItem InitItem(string na, CardTypeEnum type, Transform _father)
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
        if (cardItem.type == CardTypeEnum.PlloCard && (currentModel == ModelEnum.Optional || currentModel == ModelEnum.Choosen))
        {
            currentModel = ModelEnum.Choosen;
            CardItem _cardItem = InitItem(cardItem.name, CardTypeEnum.ChooseCard, tempfather);
            tempcards.Add(_cardItem);
            _cardItem.source = cardItem.gameObject;
            cardItem.gameObject.SetActive(false);
        }
        if (cardItem.type == CardTypeEnum.ChooseCard)
        {
            if (cardItem.source)
            {
                cardItem.source.gameObject.SetActive(true);
            }
            tempcards.Remove(cardItem);
            Destroy(cardItem.gameObject);
            if (tempcards.Count == 0)
            {
                currentModel = ModelEnum.Optional;
            }
        }
        if (cardItem.type == CardTypeEnum.ChoosenCard)
        {
            //Destroy(cardItem.gameObject);
        }
    }

    private IEnumerator Show()
    {
        if (finish == true)
        {
            finish = false;
            List<string> _chooses = String2List(chooses);
            int count = 0;
            foreach (var item in chooses.Split("|"))
            {
                string[] values = item.Split(",");
                foreach (var val in values)
                {
                    yield return new WaitForSeconds(0.2f);

                    CardItem _cardItem = InitItem(val, CardTypeEnum.ChoosenCard, choosefather);
                    if (cards.ContainsKey(val))
                    {
                        GameObject _source = cards[val].gameObject;
                        _cardItem.source = _source;
                        _source.SetActive(false);
                    }
                    _cardItem.transform.Find("Image").GetComponent<Image>().color = colors[count % 3];
                    choosecards.Add(_cardItem);
                }
                count++;
            }
            
            finish = true;
        }
    }

    public void BeginRecord()
    {
        transform.Find("Center/Button_Record").GetComponent<Button>().interactable = false;
        Clean();
    }

    public void Reset()
    {
        Clean();
        transform.Find("Center/Button_Record").GetComponent<Button>().interactable = true;
        Suiji();
    }

    private void Clean()
    {
        currentModel = ModelEnum.Optional;

        int count = choosecards.Count;

        for (int i = 0; i < count; i++)
        {
            if (choosecards[i].GetComponent<CardItem>().source)
            {
                choosecards[i].GetComponent<CardItem>().source.gameObject.SetActive(true);
            }
            Destroy(choosecards[i].gameObject);
        }
        choosecards.Clear();

        count = tempcards.Count;
        for (int i = 0; i < count; i++)
        {
            if (tempcards[i].GetComponent<CardItem>().source)
            {
                tempcards[i].GetComponent<CardItem>().source.gameObject.SetActive(true);
            }
            Destroy(tempcards[i].gameObject);
        }
        tempcards.Clear();

        mychooses.Clear();
    }

    public void Record()
    {
        Color _color = colors[mychooses.Count % 3];
        string _mychoose = "";
        int count = tempfather.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform _transform = tempfather.GetChild(0);
            _mychoose += _transform.name + ',';
            _transform.GetComponent<CardItem>().type = CardTypeEnum.ChoosenCard;
            _transform.SetParent(choosefather, true);
            _transform.Find("Image").GetComponent<Image>().color = _color;
        }
        _mychoose = _mychoose.Trim(',');
        mychooses.Add(_mychoose);
    }

    public void Computer()
    {
        string _mychoose = "";
        foreach (var item in mychooses)
        {
            _mychoose += item + "|";
        }
        _mychoose = _mychoose.Trim('|');

        List<string> source = String2List(chooses);
        List<string> cho = String2List(_mychoose);

        bool choosetrue = source.Count == cho.Count;
        for (int i = 0; choosetrue && i < cho.Count; i++)
        {
            choosetrue &= (int.Parse(source[i]) / 10 == int.Parse(cho[i]) / 10);
        }

        Debug.Log(choosetrue ? "正确" : "错误");

        ShowDialog(choosetrue);
    }

    void ShowDialog(bool value)
    {
        dialog.Find("Message").GetComponent<Text>().text = value ? "正确" : "错误";
        dialog.gameObject.SetActive(true);
    }

    public void Repeat()
    {
        dialog.gameObject.SetActive(false);
        Reset();
    }

    public void Next()
    {
        dialog.gameObject.SetActive(false);
        chooses = RandomString(8);
        Reset();
    }

    string RandomString(int count)
    {
        string[] strings = new string[cards.Count];
        cards.Keys.CopyTo(strings, 0);
        
        List<string> selectedStrings = new List<string>();

        while (selectedStrings.Count < count)
        {
            int index = Random.Range(0, strings.Length);
            string selectedString = strings[index];

            if (!selectedStrings.Contains(selectedString))
            {
                selectedStrings.Add(selectedString);
            }
        }
        string strdata = "";
        foreach (string str in selectedStrings)
        {
            strdata += str + ",";
        }
        return strdata.Trim(',');
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

    public void Suiji()
    {
        Clean();

        if (currentModel == ModelEnum.Optional)
        {
            currentModel = ModelEnum.Random;
            StartCoroutine(Show());
        }
    }

    public void SubmitRecord()
    {
        string str = "";
        for (int i = 0; i < choosefather.childCount; i++)
        {
            str += choosefather.GetChild(i).name + ",";
        }
        Debug.Log(str);
    }
}
