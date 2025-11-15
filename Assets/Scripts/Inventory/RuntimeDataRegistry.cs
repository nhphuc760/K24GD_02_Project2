using System;
using System.Collections.Generic;
using UnityEngine;

public static class RuntimeDataRegistry
{
   
        private static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        static RuntimeDataRegistry()
        {
            Register<ToolRunTimeData>();
        }

        public static void Register<T>() where T : DataRunTimeItem
        {
            typeMap[typeof(T).Name] = typeof(T);
        }

        public static Type GetType(string name)
        {
            typeMap.TryGetValue(name, out var t);
            return t;
        }
 

}
