using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardcController : MonoBehaviour
{
    public Transform chooseparent;
    public GameObject sampleitem;
    public GameObject father;
    public List<string> names;
    public Text result;
    public List<string> chooses;
    public bool finish = true;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        names = new List<string>();
        for (int i = 1; i < 14; i++) {
            for (int j = 0; j < 4; j++) {
                string na = i + "" + j;
                InitItem(na);
            }
        }

        for (int i = 14; i < 16; i++)
        {
            string na = i + "0";
            InitItem(na);
        }
    }

    void InitItem(string na) {
        names.Add(na);
        GameObject gc = Instantiate(sampleitem, father.transform);
        gc.name = na;
        Texture2D imageicon = Resources.Load("image/" + na) as Texture2D;
        Sprite iconsprite = Sprite.Create(imageicon, new Rect(0, 0, imageicon.width, imageicon.height), new Vector2(0.5f, 0.5f));
        gc.transform.GetComponent<Image>().sprite = iconsprite;
        gc.gameObject.SetActive(true);
    }

    public void Suiji()
    {
        chooses = Shuffle(names);
        StartCoroutine(Show());
    }

    public List<string> Shuffle(List<string> array)
    {
        int n = array.Count;
        for (int i = 0; i < n; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, n);
            string temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
        return array;
    }
    
    private IEnumerator Show()
    {
        if (finish == true)
        {
            finish = false;
            foreach (var item in chooses)
            {
                yield return new WaitForSeconds(0.2f);
                father.transform.Find(item).GetComponent<CardItem>().Choose();
            }
            finish = true;
        }
    }

    public void Clean()
    {
        for (int i = 0; i < chooseparent.childCount; i++)
        {
            chooseparent.GetChild(i).GetComponent<CardItem>().Choose();
        }
    }

    public void Jisuan()
    {
        bool choosetrue = true;
        for (int i = 0; i < chooseparent.childCount; i++)
        {
            choosetrue &= (int.Parse(chooses[i]) / 10 == int.Parse(chooseparent.GetChild(i).name) / 10);
        }

        result.text = choosetrue ? "正确" : "错误";
    }
}
