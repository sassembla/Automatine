using UnityEngine;


/**
	convert JSON to RuntimeAutoData using Unity's JsonUtility.
*/
public class AutomatineJsonConverter
{
    public static string AutoDataToJson(RuntimeAutoData autoData)
    {
        var serialized = JsonUtility.ToJson(autoData);
        return serialized;
    }

    public static RuntimeAutoData JsonToAutoData(string json)
    {
        var deserializedData = JsonUtility.FromJson<RuntimeAutoData>(json);
        return deserializedData;
    }
}