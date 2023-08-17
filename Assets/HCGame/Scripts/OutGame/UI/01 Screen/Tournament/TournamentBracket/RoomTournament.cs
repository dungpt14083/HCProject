using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTournament : MonoBehaviour
{
    [SerializeField] List<UserRoomItem> listUserRoomItem;
    [SerializeField] Transform centerPosition;


    public Vector3 CenterPosition => centerPosition.position;

    public void ShowView(RoomTournamentProto data)
    {
        if (data != null)
        {
            var tmpIndex = 0;
            for (int i = 0; i < data.User.Count && i < listUserRoomItem.Count; i++)
            {
                listUserRoomItem[i].ShowView(data.User[i],data.Status);
                tmpIndex++;
            }

            if (tmpIndex < listUserRoomItem.Count)
            {
                for (int j = tmpIndex; j < listUserRoomItem.Count; j++)
                {
                    listUserRoomItem[j].SetDefault();
                }
            }
        }
        else
        {
            for (int i = 0; i < listUserRoomItem.Count; i++)
            {
                listUserRoomItem[i].SetDefault();
            }
        }
    }
}