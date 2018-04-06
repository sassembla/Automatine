using System;
using System.Collections.Generic;

public static class ConditionGateway
{
    private static Dictionary<string, Type> nameTypeDict = new Dictionary<string, Type>();
    private static Dictionary<string, Type> bareNameTypeDict = new Dictionary<string, Type>();
    private static Dictionary<Type, string[]> typeNamesDict = new Dictionary<Type, string[]>();
    private static Dictionary<Type, object[]> typeValueDict = new Dictionary<Type, object[]>();
    private static Dictionary<Type, int> typeIndexDict = new Dictionary<Type, int>();
    /**
        Generate cache.
     */
    static ConditionGateway()
    {
        var collectedTypes = CollectedConditions.conditions;
        for (var i = 0; i < collectedTypes.Length; i++)
        {
            var type = collectedTypes[i];
            typeNamesDict[type] = Enum.GetNames(type);

            var enumValues = Enum.GetValues(type);
            typeValueDict[type] = new object[enumValues.Length];

            for (var j = 0; j < enumValues.Length; j++)
            {
                typeValueDict[type][j] = enumValues.GetValue(j);
            }

            nameTypeDict[type.Name] = type;
            bareNameTypeDict[type.Name.Replace("AutoConditions+", string.Empty)] = type;

            typeIndexDict[type] = i;
        }
    }

    /**
		return index of condition.
	*/
    public static int GetIndexOfConditionType(object condition)
    {
        var t = GetConditionType(condition);

        var index = Array.IndexOf(CollectedConditions.conditions, t);
        if (index != -1)
        {
            return index;
        }

        var types = new string[CollectedConditions.conditions.Length];
        for (var i = 0; i < types.Length; i++)
        {
            var type = CollectedConditions.conditions[i];
            types[i] = type.ToString();
        }
        throw new Exception(
            "failed to detect condition:" + condition
             + " is not contained to runtime. please update CollectedConditions.cs to latest."
             + " current defined types are:" + string.Join(", ", types));
    }

    public static Type GetConditionType(object condition)
    {
        return condition.GetType();
    }

    public static Type ConditionTypeFromString(string conditionTypeNameStr)
    {
        return bareNameTypeDict[conditionTypeNameStr];
    }

    public static T EnumByString<T>(string valueStr)
    {
        return (T)Enum.Parse(typeof(T), valueStr, true);
    }

    public static string[] TypeValuesFromTypeConditionStr(string typeConditionStr)
    {
        var targetType = nameTypeDict[typeConditionStr];
        return typeNamesDict[targetType];
    }

    public static object ConditionFromString(string conditionTypeStr, string conditionValueStr)
    {
        // type -> string[]
        // stringからindexを出す
        // type -> object[]
        // indexを適応する。
        var targetType = nameTypeDict[conditionTypeStr];
        var names = typeNamesDict[targetType];
        var values = typeValueDict[targetType];

        var enumIndex = Array.IndexOf(names, conditionValueStr);
        if (enumIndex != -1)
        {
            return values[enumIndex];
        }

        throw new Exception("searching conditionValue:" + conditionValueStr + " is not found in conditionType:" + conditionTypeStr);
    }

    public static string ConditionTypeString(int typeIndex)
    {
        return CollectedConditions.conditions[typeIndex].Name;
    }

    public static int IndexOfConditionType(Type conditionType)
    {
        // Type, int
        return typeIndexDict[conditionType];
    }

    // public static Type[] ConditionTypesFromValues(Type[] conditionValues)
    // {
    //     return conditionValues.Select(val => GetConditionType(val)).ToArray();
    // }

    public static string ConditionValueString(int typeIndex, int valueIndex)
    {
        var type = CollectedConditions.conditions[typeIndex];

        // type, value string[]
        var condSource = typeValueDict[type];
        var conditionValueStr = condSource[valueIndex].ToString();

        return conditionValueStr;
    }



    /*
		extension methods for conditions(int[]).
	*/

    /**
		check if the part of the condition is containd or not containd.
	*/
    public static bool ContainsCondition<T>(this int[] currentConditionSource, T condition) where T : IConvertible
    {
        var targetConditon = currentConditionSource[GetIndexOfConditionType(condition)];
        var val = (int)((object)condition);
        if (targetConditon == val)
        {
            return true;
        }
        return false;
    }

    /**
		check if the part of the condition is not containd or containd.
	*/
    public static bool NotContainsCondition<T>(this int[] currentConditionSource, T condition) where T : IConvertible
    {
        var targetConditon = currentConditionSource[GetIndexOfConditionType(condition)];

        var val = (int)((object)condition);
        if (targetConditon != val)
        {
            return true;
        }
        return false;
    }

    /**
		check if every part of conditions are containd or not containd.
	*/
    public static bool ContainsAllConditions(this int[] currentConditionSource, params object[] conditions)
    {
        for (var i = 0; i < conditions.Length; i++)
        {
            var condition = conditions[i];
            var targetConditon = currentConditionSource[GetIndexOfConditionType(condition)];
            var val = (int)((object)condition);
            if (targetConditon != val)
            {
                return false;
            }
        }
        return true;
    }

    /**
		check if every part of conditions are not containd or containd.
	*/
    public static bool NotContainsAllConditions(this int[] currentConditionSource, params object[] conditions)
    {
        for (var i = 0; i < conditions.Length; i++)
        {
            var condition = conditions[i];
            var targetConditon = currentConditionSource[GetIndexOfConditionType(condition)];
            var val = (int)((object)condition);
            if (targetConditon == val)
            {
                return false;
            }
        }
        return true;
    }

    /**
		check if all conditions are same or not.
	*/
    public static bool SameConditions(this int[] currentConditionSource, int[] targetConditionSource)
    {
        if (currentConditionSource.Length != targetConditionSource.Length)
        {
            return false;
        }
        for (int i = 0; i < currentConditionSource.Length; i++)
        {
            if (currentConditionSource[i] != targetConditionSource[i])
            {
                return false;
            }
        }
        return true;
    }


    public static bool Contains<T>(int[] currentConditionSource, T condition) where T : IConvertible
    {
        return currentConditionSource.ContainsCondition(condition);
    }

    public static bool NotContains<T>(int[] currentConditionSource, T condition) where T : IConvertible
    {
        return currentConditionSource.NotContainsCondition(condition);
    }

    public static bool ContainsAll(int[] currentConditionSource, params object[] conditions)
    {
        return currentConditionSource.ContainsAllConditions(conditions);
    }

    public static bool NotContainsAll(int[] currentConditionSource, params object[] conditions)
    {
        return currentConditionSource.NotContainsAllConditions(conditions);
    }

    public static bool Same(int[] currentConditionSource, int[] targetConditionSource)
    {
        return currentConditionSource.SameConditions(targetConditionSource);
    }

}