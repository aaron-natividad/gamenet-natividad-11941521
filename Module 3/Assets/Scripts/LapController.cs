using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class LapController : MonoBehaviourPunCallbacks
{
    public List<GameObject> lapTriggers = new List<GameObject>();

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0
    }

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoFinishedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string) data[0];
            int finishOrder = (int) data[1];
            int viewID = (int) data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUIText = GameManager.instance.finisherTextUI[finishOrder - 1];
            orderUIText.SetActive(true);

            if(viewID == photonView.ViewID)
            {
                orderUIText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer + " (YOU)";
                orderUIText.GetComponent<Text>().color = Color.red;
            }
            else
            {
                orderUIText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
            }
        }      
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject go in GameManager.instance.lapTriggers)
        {
            lapTriggers.Add(go);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(lapTriggers.Contains(col.gameObject))
        {
            if (col.gameObject.tag == "FinishTrigger")
            {
                GameFinish();
            }
            else
            {
                int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);
                lapTriggers[indexOfTrigger].SetActive(false);
            }  
        }
    }

    public void GameFinish()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;

        GameManager.instance.finishOrder++;
        string nickName = photonView.Owner.NickName;
        int viewID = photonView.ViewID;

        // event data
        object[] data = new object[] { nickName, GameManager.instance.finishOrder, viewID };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = false
        };
        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions);
    }
}
