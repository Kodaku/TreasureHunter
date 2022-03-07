using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using UnityEngine;

using State = System.Tuple<UnityEngine.Vector3, CellInfo, System.Tuple<CellInfo, CellInfo, CellInfo, CellInfo>>;
using ActionQ = System.Collections.Generic.Dictionary<Action, float>;
using ActionProbability = System.Collections.Generic.Dictionary<Action, float>;

public class QItem
{
    [XmlAttribute]
    public State state;
    [XmlAttribute]
    public ActionQ value;
}

public class ActionQItem
{
    [XmlAttribute]
    public Action action;
    [XmlAttribute]
    public float qValue;
}

public class PolicyItem
{
    [XmlAttribute]
    public State state;
    [XmlAttribute]
    public ActionProbability value;
}

public class ActionPolicyItem
{
    [XmlAttribute]
    public Action action;
    [XmlAttribute]
    public float probability;
}

public class Serializer
{
    /// <summary>
    /// Writes the given object instance to a Json file.
    /// <para>Object type must have a parameterless constructor.</para>
    /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
    /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
    /// </summary>
    /// <typeparam name="T">The type of object being written to the file.</typeparam>
    /// <param name="filePath">The file path to write the object instance to.</param>
    /// <param name="objectToWrite">The object instance to write to the file.</param>
    /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
    public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
    {
        TextWriter writer = null;
        try
        {
            var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
            writer = new StreamWriter(filePath, append);
            writer.Write(contentsToWriteToFile);
        }
        finally
        {
            if (writer != null)
                writer.Close();
        }
    }

    /// <summary>
    /// Reads an object instance from an Json file.
    /// <para>Object type must have a parameterless constructor.</para>
    /// </summary>
    /// <typeparam name="T">The type of object to read from the file.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the Json file.</returns>
    public static T ReadFromJsonFile<T>(string filePath) where T : new()
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(filePath);
            string fileContents = reader.ReadToEnd();
            // Debug.Log(fileContents);
            var content = JsonConvert.DeserializeObject<T>(fileContents);
            // Debug.Log(content);
            // var obj = JsonConvert.DeserializeObject(content);
            Debug.Log(content);
            return (T)content;
        }
        finally
        {
            if (reader != null)
                reader.Close();
        }
    }

    public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
    {
        using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, objectToWrite);
        }
    }

    /// <summary>
    /// Reads an object instance from a binary file.
    /// </summary>
    /// <typeparam name="T">The type of object to read from the binary file.</typeparam>
    /// <param name="filePath">The file path to read the object instance from.</param>
    /// <returns>Returns a new instance of the object read from the binary file.</returns>
    public static T ReadFromBinaryFile<T>(string filePath)
    {
        using (Stream stream = File.Open(filePath, FileMode.Open))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)binaryFormatter.Deserialize(stream);
        }
    }

    public static void WriteQToFile(string filename, Dictionary<State, ActionQ> Q)
    {
        var writer = new StreamWriter(filename);
        XmlSerializer serializer = new XmlSerializer(typeof(QItem[]), new XmlRootAttribute() { ElementName = "items" });
        serializer.Serialize(writer, Q.Select(kv=>new QItem(){state = kv.Key,value=kv.Value}).ToArray() );
    }

    public static void WritePolicyToFile(string filename, Dictionary<State, ActionProbability> policy)
    {
        var writer = new StreamWriter(filename);
        XmlSerializer serializer = new XmlSerializer(typeof(QItem[]), new XmlRootAttribute() { ElementName = "items" });
        serializer.Serialize(writer, policy.Select(kv=>new QItem(){state = kv.Key, value=kv.Value}).ToArray() );
    }
}
