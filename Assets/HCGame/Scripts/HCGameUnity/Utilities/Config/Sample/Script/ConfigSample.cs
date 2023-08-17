using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileHelpers;
using NBCore;

[FileHelpers.DelimitedRecord("\t")]
[FileHelpers.IgnoreFirst(1)]
[FileHelpers.IgnoreCommentedLines("//")]

public class ConfigSampleRecord {
    [FieldNullValue(typeof(int), "0")]
    public int value1;

    [FieldNullValue(typeof(string), "")]
    public string value2;
} // ConfigSampleRecord

public class ConfigSample : GConfigDataTable<ConfigSampleRecord> {
    public ConfigSample() : base("ConfigSample", typeof(BaseItemData)) {
        
    }


    protected override void OnDataLoaded () {
        RebuildIndexField<int>("value1");
    }


    public ConfigSampleRecord GetSample (int id) {
        return FindRecordByIndex<int>("value1", id);
    } // GetSample ()


    public List<ConfigSampleRecord> GetAllSamples() {
        return records;
    } // GetAllSamples()

}
