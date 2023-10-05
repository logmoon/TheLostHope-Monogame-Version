﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheLostHope.Engine.ContentManagement;
using TheLostHopeEngine.EngineCode.Assets.Core;

namespace TheLostHopeEngine.EngineCode.ContentManagement.Json
{
    public class JsonAssetManager
    {
        private JsonSerializer serializer;

        public JsonAssetManager()
        {
            serializer = new JsonSerializer();
            serializer.TypeNameHandling = TypeNameHandling.Auto;
        }

        public T LoadAsset<T>(string filePath) where T : ScriptableObject
        {
            using (StreamReader fileReader = new StreamReader(filePath))
            using (JsonTextReader jsonReader = new JsonTextReader(fileReader))
            {
                return serializer.Deserialize<T>(jsonReader);
            }
        }

        public void SaveAsset<T>(string filePath, T asset) where T : ScriptableObject
        {
            using (StreamWriter fileWriter = new StreamWriter(filePath))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(fileWriter))
            {
                serializer.Serialize(jsonWriter, asset);
            }
        }
    }
}
