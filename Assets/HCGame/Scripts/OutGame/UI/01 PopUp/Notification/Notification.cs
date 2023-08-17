using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : UIPopupView<Notification>
{
    public Button btClose;
    public ItemNotification itemNotificationDefault;
    public List<ItemNotification> notifications;
    private void Awake()
    {
        btClose.onClick.AddListener(CloseView);
    }

    private void OnEnable()
    {
        UISignals.OnUpdateNotification.AddListener(UpdateShowView);
    }

    private void OnDisable()
    {
        UISignals.OnUpdateNotification.RemoveListener(UpdateShowView);
    }

    private void UpdateShowView()
    {
        Show(HCAppController.Instance.listHcNotification);
    }

    public void Show(Dictionary<long, HcNotificationProto> listHcNotification)
    {
        if (notifications == null) notifications = new List<ItemNotification>();
        if (listHcNotification == null)
        {
            for (int i = 0; i < notifications.Count; i++)
            {
                Destroy(notifications[i]);
            }
            return;
        }
        int count = 0;
        foreach (var item in listHcNotification)
        {
            if (notifications.Count > count)
            {
                notifications[count].Show(item.Value);
            }
            else
            {
                var itemNew = Instantiate(itemNotificationDefault, itemNotificationDefault.transform.parent);
                notifications.Add(itemNew);
                itemNew.Show(item.Value);
            }
            count += 1;
        }
        if (notifications.Count > count)
        {
            for (int i = count; i < notifications.Count; i++)
            {
                Destroy(notifications[i]);
            }
        }
    }
}