using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Respawn Component")]
    public GameObject respawnText;

    [Header("Win Condition Component")]
    public GameObject winBox;

    [Header("Score Component")]
    public GameObject scoreText;

    [Header("Kill History UI Components")]
    public GameObject entryPrefab;
    public GameObject entryParent;
    public int entryOffset;

    // Start is called before the first frame update
    void Start()
    {
        winBox.SetActive(false);
        entryOffset = 50;
    }

    public void UpdateScoreText(int score)
    {
        scoreText.GetComponent<Text>().text = "Score: " + score.ToString();
    }

    public void UpdateRespawnText(string text)
    {
        respawnText.GetComponent<Text>().text = text;
    }

    public void UpdateWinText(string text, bool isActive)
    {
        if (isActive)
        {
            winBox.SetActive(true);
            Text winText = winBox.GetComponentInChildren<Text>();
            winText.GetComponent<Text>().text = text;
        }
        else
        {
            winBox.SetActive(false);
        }
    }

    public void UpdateKillUI()
    {
        int index = 0;

        foreach (Transform item in entryParent.transform)
        {
            Destroy(item.gameObject);
        }

        foreach (string entry in GameManager.instance.killHistory)
        {
            GameObject entryItem = Instantiate(entryPrefab);
            entryItem.transform.SetParent(entryParent.transform);
            entryItem.transform.localScale = Vector3.one;
            entryItem.transform.position = entryParent.transform.position - new Vector3(0,index*entryOffset,0);
            entryItem.GetComponentInChildren<Text>().text = entry;
            index++;
        }
    }
}
