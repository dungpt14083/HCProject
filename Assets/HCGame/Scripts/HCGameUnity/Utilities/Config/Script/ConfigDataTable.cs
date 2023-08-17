using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace NBCore {

    #region SonTT add more
    [Serializable]
    public class BaseItemData {
        public BaseItemData () {

        }
        public virtual void SetupData (IConfigDataTable data) {
            throw new System.NotImplementedException();
        }
    }
    #endregion

    public interface IConfigDataTable {
        /// <summary>
        /// Get name
        /// </summary>
        string GetName ();

        /// <summary>
        /// Begin append load
        /// </summary>
        void BeginLoadAppend ();

        /// <summary>
        /// Begin append load
        /// </summary>
        void EndLoadAppend ();

        /// <summary>
        /// Load data from string from memory
        /// </summary>
        void LoadFromString (string content);

        /// <summary>
        /// Load data from a text asset
        /// </summary>
        void LoadFromAsset (TextAsset asset);

        /// <summary>
        /// Load data from a text asset path
        /// </summary>
        void LoadFromAssetPath (string assetPath);

        /// <summary>
        /// Clear all data
        /// </summary>
        void Clear ();

        /// <summary>
        /// Parse config data to Binary 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="destinationFolder"></param>
        /// <param name="destinationPath"></param>
        /// <returns></returns>
        bool ToBinary<T>(string destinationPath) where T : BaseItemData, new();

        // KSH : get 'BaseItemData class' that using this class
        Type GetBaseItemDataType();

    } // IConfigDataTable
    

    [Serializable]
    public class GConfigDataTable<TDataRecord> : IConfigDataTable where TDataRecord : class {
        // Index fields
        public class IndexField<TIndex> : Dictionary<TIndex, object> {

        };
            
        // Indices lookup
        
        public virtual Dictionary<string, object> indices{ get; set; }

        // Record list
        public List<TDataRecord> records { get; private set; }

        // Name
        public string name { get; private set; }

        // Is loaded
        public bool isLoaded { get; private set; }

        // Is empty
        public bool isEmpty { get { return records.Count == 0; } }
		
        // Get num records
        public int count { get { return records.Count; } }

        // Flag to mark append loading
        private bool isAppendLoading = false;

        // Variables hold info of generic methods
        private MethodInfo methodRebuildIndexField, methodFindRecordByIndex, methodFindRecordsByIndex;

        // KSH : BaseItemDataType
        public Type m_baseItemDataType { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public GConfigDataTable () {
            this.name = GetType().Name;
            records = new List<TDataRecord>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GConfigDataTable (string name, Type bidType) {
            this.name = name;
            m_baseItemDataType = bidType;
            records = new List<TDataRecord>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitGenericMethodRefs () {
            Type thisType = typeof(GConfigDataTable<TDataRecord>);

            methodRebuildIndexField = thisType.GetMethods(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(m => m.Name == "RebuildIndexField" && m.GetParameters().Count() == 1);
            methodFindRecordByIndex = thisType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(m => m.Name == "FindRecordByIndex" && m.GetParameters().Count() == 2);
            methodFindRecordsByIndex = thisType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(m => m.Name == "FindRecordsByIndex" && m.GetParameters().Count() == 2);
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnDataLoaded () {
        }

        /// <summary>
        /// Get name
        /// </summary>
        public string GetName () {
            return name;
        }

        public void BeginLoadAppend () {
            isAppendLoading = true;
        }

        public void EndLoadAppend () {
            if (isAppendLoading) {
                isLoaded = true;
                InitGenericMethodRefs();
                OnDataLoaded();
                isAppendLoading = false;
            }
        }

        /// <summary>
        /// Load data from string from memory
        /// </summary>
        public void LoadFromString (string content) {
            if (string.IsNullOrEmpty(content))
                throw new ArgumentException("Content is null or empty");

            if (!isAppendLoading && isLoaded)
                Clear();

            FileHelpers.FileHelperEngine fileEngine = new FileHelpers.FileHelperEngine(typeof(TDataRecord));
            records.AddRange(fileEngine.ReadString(content).Select(r => r as TDataRecord));

            if (!isAppendLoading) {
                isLoaded = true;
                InitGenericMethodRefs();
                OnDataLoaded();
            }
        }

        /// <summary>
        /// Load data from a text asset
        /// </summary>
        public void LoadFromAsset (TextAsset asset) {
            if (asset == null)
                throw new ArgumentNullException("Asset data is invalid");
            LoadFromString(asset.text);
        }

        /// <summary>
        /// Load data from a text asset path
        /// </summary>
        public void LoadFromAssetPath (string assetPath) {
            if (string.IsNullOrEmpty(assetPath))
                throw new ArgumentException("Asset path is null or empty");
            LoadFromAsset(Resources.Load<TextAsset>(assetPath));
        }

        /// <summary>
        /// Clear all data
        /// </summary>
        public void Clear () {
            records.Clear();
            isLoaded = false;
        }


        /// <summary>
        /// Build index from a table field
        /// </summary>
        public void RebuildIndexField (Type indexType,
                                   string fieldName) {
            MethodInfo generic = methodRebuildIndexField.MakeGenericMethod(indexType);
            generic.Invoke(this, new object[] { fieldName });		
        }

        /// <summary>
        /// Build index from a table field
        /// </summary>
        public void RebuildIndexField<TIndex> (string fieldName) {
            if (isEmpty)
                return;

            // Search field name by reflection
            Type recordType = typeof(TDataRecord);

            // Check the record type to find the field you need to indexed
            // TSON : Change field to Properties to using ZeroFormatter
//            FieldInfo fieldInfo = recordType.GetField(fieldName);
            PropertyInfo fieldInfo = recordType.GetProperty(fieldName);
            if (fieldInfo == null)
                throw new Exception("Field [" + fieldName + "] not found");

            if (indices == null)
                indices = new Dictionary<string, object>();

            // Add new index column object
            IndexField<TIndex> indexField = new IndexField<TIndex>();
            indices[fieldName] = indexField;

            // Build index column field from records
            for (int i = 0; i < records.Count; i++) {
                // Get field value
                var fieldValue = (TIndex)fieldInfo.GetValue(records[i],null);

                // the value of the index maybe a single record or a list of records that have the same key field
                object indexedValue;
                if (!indexField.TryGetValue(fieldValue, out indexedValue))
                    indexField.Add(fieldValue, records[i]);
                else {
                    // If indexedValue is a list, append data
                    if (indexedValue is List<TDataRecord>)
                        (indexedValue as List<TDataRecord>).Add(records[i]);
                    else {
                        var listRecords = new List<TDataRecord>();
                        listRecords.Add(indexedValue as TDataRecord);
                        listRecords.Add(records[i]);
                        indexField[fieldValue] = listRecords;
                    }
                }
            }
        } // RebuildInDexField ()

        /// <summary>
        /// Check if the field is indexed
        /// </summary>
        public bool IsFieldIndexed (string fieldName) {
            if (indices == null)
                return false;

            return indices.ContainsKey(fieldName);
        } // IsFieldIndexed ()

        /// <summary>
        /// 
        /// </summary>
        private TDataRecord FindRecordByIndex<TIndex> (object _indexField,
                                                   TIndex value) {
            var indexField = _indexField as IndexField<TIndex>;
            if (indexField == null)
                throw new InvalidOperationException("Index type and search key mismatch");

            // Find
            object indexedValue;
            if (!indexField.TryGetValue(value, out indexedValue))
                return null;

            // Get first item in the list
            if (indexedValue is List<TDataRecord>)
                return (indexedValue as List<TDataRecord>).FirstOrDefault() as TDataRecord;

            return indexedValue as TDataRecord;
        } // FindRecordByIndex ()

        /// <summary>
        /// 
        /// </summary>
        private List<TDataRecord> FindRecordsByIndex<TIndex> (object _indexField,
                                                          TIndex value) {
            var indexField = _indexField as IndexField<TIndex>;
            if (indexField == null)
                throw new InvalidOperationException("Index type mismatch");

            // Find
            object indexedValue;
            if (!indexField.TryGetValue(value, out indexedValue))
                return null;

            // Get first item in the list
            if (indexedValue is List<TDataRecord>)
                return indexedValue as List<TDataRecord>;

            return new List<TDataRecord>() { indexedValue as TDataRecord };
        } // FindRecordByIndex ()


        /// <summary>
        /// Find single record of a index
        /// </summary>
        public TDataRecord FindRecordByIndex<TIndex> (string indexName, TIndex value) {
            // Do not have index
            object indexField;
            if (indices == null || !indices.TryGetValue(indexName, out indexField)) {
                if (null != indexName && indexName.Length > 0)
                    throw new InvalidOperationException("Index not found 1: " + indexName);
                else
                    throw new InvalidOperationException("Index not found 1.");
            } // if
            return FindRecordByIndex<TIndex>(indexField, value);
        } // FindRecordByIndex ()


        /// <summary>
        /// Find single record of a index (non-generic)
        /// </summary>
        public TDataRecord FindRecordByIndex (Type indexType, string indexName, object value) {
            // Do not have index
            object indexField;
            if (indices == null || !indices.TryGetValue(indexName, out indexField)) {
                if (null != indexName && indexName.Length > 0)
                    throw new InvalidOperationException("Index not found 1: " + indexName);
                else
                    throw new InvalidOperationException("Index not found 1.");
            } // if

            MethodInfo generic = methodFindRecordByIndex.MakeGenericMethod(indexType);
            return (TDataRecord)generic.Invoke(this, new object[] { indexField, value });
        } // FindremordByIndex ()


        /// <summary>
        /// Find many records of a index
        /// </summary>
        public List<TDataRecord> FindRecordsByIndex<TIndex> (string indexName, TIndex value) {
            // Do not have index
            object indexField;
            if (indices == null || !indices.TryGetValue(indexName, out indexField)) {
                if (null != indexName && indexName.Length > 0) {
                    throw new InvalidOperationException("Index not found 2" + indexName);
                } else {
                    throw new InvalidOperationException("Index not found 2.");
                } // else
            } // if
            return FindRecordsByIndex<TIndex>(indexField, value);
        } // FindRecordsByIndex ()


        //=========== SonTT add more

        public bool ToBinary<T>(string destinationPath)where T : BaseItemData, new() {
            //try {
                T desItem = new T();
                desItem.SetupData(this);

                byte[] bytes = null;
                bytes = Utility.Serialize<T>(desItem);
#if UNITY_EDITOR
                //create File and Directory
                if (destinationPath.Contains("/"))
                {
                    string destinationFolder = destinationPath.Remove(destinationPath.LastIndexOf("/"));
                    if (!FileUtility.CreateDirectory(destinationFolder))
                    {
                        Debug.LogError(" can not create destination config folder : " + destinationFolder);
                        return false;
                    } // if
                    
                } // if
                File.WriteAllBytes(destinationPath, bytes);
#endif
                return true;
            //}
            //catch (Exception ex) {
            //    throw ex;

            //    Debug.LogError("To Binary Error : " + ex.ToString());
            //    return false;
            //}
        }

        // KSH
        public Type GetBaseItemDataType()
        {
            return m_baseItemDataType;
        }

    } // GConfigDataTable

} // NBCore
