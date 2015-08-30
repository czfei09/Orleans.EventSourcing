﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Orleans.EventSourcing
{
    public class EventNameCodeMapping
    {
        private static readonly IDictionary<int, Type> EventTypeCodeMappings = new Dictionary<int, Type>();

        private static readonly object Locker = new object();
        private static readonly bool Inited = false;

        public static bool TryGetEventType(int typeCode, out Type eventType)
        {
            return EventTypeCodeMappings.TryGetValue(typeCode, out eventType);
        }

        public static bool TryGetEventTypeCode(Type eventType, out int typeCode)
        {
            typeCode = (from kv in EventTypeCodeMappings where eventType == kv.Value select kv.Key).FirstOrDefault();

            return typeCode > 0;
        }

        internal static void RegisterEventType(int typeCode, Type type)
        {
            if (IsEventType(type) && !EventTypeCodeMappings.ContainsKey(typeCode))
                EventTypeCodeMappings.Add(typeCode, type);
        }

        private static bool IsEventType(Type grainType)
        {
            return grainType.IsClass && !grainType.IsAbstract && typeof(GrainEvent).IsAssignableFrom(grainType);
        }

        public static void Initialize(Dictionary<string, int> typeCodeDic)
        {
            lock (Locker)
            {
                if (typeCodeDic.Any())
                {
                    var types = Assembly.LoadFrom("FullCoin.GrainInterface.Dll").ExportedTypes;
                    foreach (var kv in typeCodeDic)
                    {
                        var type = types.SingleOrDefault(t => t.FullName == kv.Key);

                        RegisterEventType(kv.Value, type);
                    }
                }
            }
        }
    }

}
