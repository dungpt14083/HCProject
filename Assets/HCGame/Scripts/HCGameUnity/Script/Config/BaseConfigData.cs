using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class BaseConfigData<T> where T : BaseRecord, new()
{
    protected IDictionary<int, T> indexDicts = new Dictionary<int, T>();

    public virtual void SetupData(IList<string[]> datas) 
    {
        indexDicts.Clear();
        //IDictionary<int, T> result = new Dictionary<int, T>();
        foreach (var data in datas)
        {
            T record = new T();
            var props = record.GetType().GetProperties();
            //parse ID
            record.ID = int.Parse(data[0]);

            for (int i = 0; i < props.Length; i++)
            {
                var p = props[i];
                var dataIndex = i + 1;
                if (dataIndex < data.Length)
                {

                    if (string.IsNullOrEmpty(data[dataIndex]))
                    {
                        p.SetValue(record, null);
                        continue;
                    }
                    
                    object newData = data[dataIndex];
                    if (p.PropertyType.IsEnum)
                    {
                        newData = Enum.Parse(p.PropertyType, data[dataIndex]);
                    }
                    else
                    {
                        // Debug.Log("Parse data: " + p.Name + "==" + p.PropertyType + "===" + newData);
                        if (p.PropertyType != typeof(string) && !p.PropertyType.IsPrimitive)
                        {
                            Debug.Log("not primitive type : " + p.PropertyType);
                            var stringData = (string)newData;
                            // Debug.Log("string data : " + stringData);
                            newData = JsonConvert.DeserializeObject(stringData, p.PropertyType);
                        }
                    }

                    // else
                    // {
                    //     var value = Convert.ChangeType(newData, p.PropertyType);
                    //     p.SetValue(record, value);
                    // }

                    if (null == newData || true == string.IsNullOrEmpty(newData.ToString()))
                    {
                        p.SetValue(record, default);
                    }
                    else
                    {
                        var value = Convert.ChangeType(newData, p.PropertyType);
                        p.SetValue(record, value);
                    }
                    
                    // var stringData = (string)newData;
                    // var test = JsonConvert.DeserializeObject(stringData, p.PropertyType);
                    //
                    // var value = Convert.ChangeType(test, p.PropertyType);
                    // p.SetValue(record, value);
                }
            }

            indexDicts.Add(record.ID, record);
        }
    }
}

[Serializable]
public class BaseRecord
{
    [JsonProperty("id", Order = -2)]
    public int ID { get; set; }
}