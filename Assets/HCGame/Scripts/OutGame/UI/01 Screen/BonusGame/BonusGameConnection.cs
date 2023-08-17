using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using Google.Protobuf;
using BonusGamePlay;

namespace BonusGame
{
    public class BonusGameConnection : ManualSingletonMono<BonusGameConnection>
    {
        public Action<Response> OnRouletteResultReceive;
        public Action<ItemResponse> OnRouletteItemsReceive;

        public Action<ListRewardRandomBox> OnListRewardRandomBox;
        public Action<ListRewardRandomBox> OnReceiveInitRandomBox;

        public Action<ListResponseScratch> OnListResponseScratch;
        public Action<ItemResponseScratch> OnItemResponseScratch;

        #region WebSocket Event Handlers

        // Called when we received a byte data message from the server
        public void OnByteDataReceived(byte[] data)
        {
            Debug.Log("WebSocket OnByteDataReceived!");
            var packageData = PackageData.Parser.ParseFrom(data);
            switch (packageData.Header)
            {
                case 5001:
                    var response = Response.Parser.ParseFrom(packageData.Data);
                    OnRouletteResultReceive?.Invoke(response);
                    break;
                case 5003:
                    var itemResponse = ItemResponse.Parser.ParseFrom(packageData.Data);
                    OnRouletteItemsReceive?.Invoke(itemResponse);
                    break;

                //Cho randomox bắt đầu có phản hồi sau khi ấn mở
                case 5002:
                    var listRewardRandomBox = ListRewardRandomBox.Parser.ParseFrom(packageData.Data);
                    OnListRewardRandomBox?.Invoke(listRewardRandomBox);
                    break;
                case 5005:
                    var feeRandomBox = ListRewardRandomBox.Parser.ParseFrom(packageData.Data);
                    OnReceiveInitRandomBox?.Invoke(feeRandomBox);
                    break;

                //Cho việc lỗi 
                case 5000:
                    var itemResponseScratch = ItemResponseScratch.Parser.ParseFrom(packageData.Data);
                    OnItemResponseScratch?.Invoke(itemResponseScratch);
                    break;
                case 5004:
                    var scratchResponse = ListResponseScratch.Parser.ParseFrom(packageData.Data);
                    OnListResponseScratch?.Invoke(scratchResponse);
                    break;


                default:
                    break;
            }
        }

        #endregion
    }
}