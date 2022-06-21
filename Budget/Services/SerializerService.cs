﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Budget.Services
{
    class SerializerService
    {
        #region XML

        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
                writer.Flush();
            }
            finally
            {
                writer?.Close();
            }
        }

        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
                return default(T);

            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                reader?.Close();
            }
        }

        #endregion
    }
}
