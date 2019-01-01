using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SnakeHead : MonoBehaviour {

    public List<Transform> bodyList = new List<Transform>();
    public float velocity = 0.35f;
    public int step;
    private int x;
    private int y;
    private Vector3 headPos;
    public bool isDie = false;

    public GameObject bodyPrefab;
    public Sprite[] bodySprites = new Sprite[2];
    private Transform canvas;
    public GameObject dieEffect;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").transform;
    }

    void Start()
    {
        InvokeRepeating("Move", 0, velocity);
        x = 0;y = step;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && MainUIController.Instance.isPause == false&&isDie == false)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity - 0.2f);
        }
        if (Input.GetKeyUp(KeyCode.Space) && MainUIController.Instance.isPause == false && isDie == false)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity);
        }
        if (Input.GetKey(KeyCode.W) && y != -step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            x = 0;y = step;
        }
        if (Input.GetKey(KeyCode.A)&& x!= step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
            x = -step; y = 0;
        }
        if (Input.GetKey(KeyCode.S)&& y != step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            x = 0; y = -step;
        }
        if (Input.GetKey(KeyCode.D) && x != -step && MainUIController.Instance.isPause == false && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
            x = step; y = 0;
        }
    }

    void Move()
    {
        headPos = gameObject.transform.localPosition;
        gameObject.transform.localPosition = new Vector3(headPos.x + x, headPos.y + y, headPos.z);
        if (bodyList.Count > 0)
        {
            for (int i = bodyList.Count - 2; i >= 0; i--)
            {
                bodyList[i + 1].localPosition = bodyList[i].localPosition;
            }
            bodyList[0].localPosition = headPos;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Food"))
        {
            //Debug.Log("hlx");
            Destroy(collision.gameObject);
            MainUIController.Instance.UpdateUI();
            Grow();
            FoodMaker.Instance.MakeFood((Random.Range(0,100)<20)?true:false);
            
        }
        else if (collision.gameObject.CompareTag("Body"))
        {
            Die();
        }
        else if (collision.gameObject.CompareTag("Reward"))
        {
            Destroy(collision.gameObject);
            MainUIController.Instance.UpdateUI(Random.Range(5,15)*10);
            Grow();
            
        }
        else 
        {
            switch (collision.gameObject.name)
            {
                case "Up":
                    Debug.Log("up");
                    transform.localPosition = new Vector3(transform.localPosition.x,-transform.localPosition.y+30,transform.localPosition.z);
                    break;
                case "Down":
                    Debug.Log("down");
                    transform.localPosition= new Vector3(transform.localPosition.x, -transform.localPosition.y - 30,transform.localPosition.z);
                    break;
                case "Right":
                    transform.localPosition = new Vector3(-transform.localPosition.x+210, transform.localPosition.y, transform.localPosition.z);
                    break;
                case "Left":
                    transform.localPosition = new Vector3(-transform.localPosition.x+150, transform.localPosition.y, transform.localPosition.z);
                    break;
            }
        }
    }

    void Grow()
    {
        int index = (bodyList.Count % 2 == 0) ? 0 : 1;
        GameObject body = Instantiate(bodyPrefab, new Vector3(2000, 2000, 0), Quaternion.identity);
        body.GetComponent<Image>().sprite = bodySprites[index];
        body.transform.SetParent(canvas, false);
        bodyList.Add(body.transform);
    }

    void Die()
    {
        CancelInvoke();
        isDie = true;
        Instantiate(dieEffect);
        PlayerPrefs.SetInt("lastl", MainUIController.Instance.length);
        PlayerPrefs.SetInt("lasts", MainUIController.Instance.score);
        if (PlayerPrefs.GetInt("bests", 0) < MainUIController.Instance.score)
        {
            PlayerPrefs.SetInt("bestl", MainUIController.Instance.length);
            PlayerPrefs.SetInt("bests", MainUIController.Instance.score);
        }
        StartCoroutine(GameOver(1.5f));
    }

    IEnumerator GameOver(float t)
    {
        yield return new WaitForSeconds(t);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
