using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class CsvReader
{
    public static List<T> ReadCsvFromResources<T>(string path, int skipRows)
    {
        List<T> result = new List<T>();
        TextAsset textAsset = Resources.Load(path) as TextAsset;

        if (textAsset == null)
        {
            Debug.LogWarning($"{path} == NULL");
            return result;
        }

        Type type = typeof(T);
        foreach (var s in textAsset.text.Split("\n").Skip(skipRows))
        {
            T t = ReadLine<T>(s, type);
            result.Add(t);
        }
        return result;
    }

    private static T ReadLine<T>(string columnLine, Type type)
    {
        string[] columns = columnLine.Split(",");

        ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
        T t = (T)constructorInfo.Invoke(Array.Empty<object>());
        FieldInfo[] fields = t.GetType().GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            object parsedValue = Parse(fields[i].FieldType, columns[i]);
            fields[i].SetValue(t, parsedValue);
        }
        
        return t;
    }

    private static object Parse(Type type, string value)
    {
        TypeCode code = Type.GetTypeCode(type);

        return code switch
        {
            TypeCode.Boolean => bool.Parse(value),
            TypeCode.Byte => byte.Parse(value),
            TypeCode.Char => char.Parse(value),
            TypeCode.DateTime => DateTime.Parse(value),
            TypeCode.Decimal => Decimal.Parse(value),
            TypeCode.Double => double.Parse(value),
            TypeCode.Empty => null,
            TypeCode.Int16 => short.Parse(value),
            TypeCode.Int32 => int.Parse(value),
            TypeCode.Int64 => long.Parse(value),
            TypeCode.Object => null,
            TypeCode.SByte => sbyte.Parse(value),
            TypeCode.Single => float.Parse(value),
            TypeCode.String => value,
            TypeCode.UInt16 => ushort.Parse(value),
            TypeCode.UInt32 => uint.Parse(value),
            TypeCode.UInt64 => ulong.Parse(value),
            _ => null
        };
    }
}