using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TwinCAT.Ads;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension
{
    public static class AdsClientExtensions
    {
        public static Task WriteAsync<T>(this IAdsSymbolicAccess client, string variablePath, T value)
        {
            return Task.Run(() =>
            {
                var symbolInfo = (ITcAdsSymbol5)client.ReadSymbolInfo(variablePath);
                var targetType = symbolInfo.DataType.ManagedType;
                var targetValue = targetType != null ? Convert.ChangeType(value, targetType) : value;
                client.WriteSymbol(symbolInfo, targetValue);
            });
        }

        public static Task<T> ReadAsync<T>(this IAdsSymbolicAccess client, string variablePath)
        {
            return Task.Run(() =>
            {
                var symbolInfo = (ITcAdsSymbol5)client.ReadSymbolInfo(variablePath);
                var obj = client.ReadSymbol(symbolInfo);
                return (T) Convert.ChangeType(obj, typeof(T));
            });
        }

        public static Task WriteJson(this IAdsSymbolicAccess client, string variablePath, JObject obj)
        {
            return WriteJson(client, variablePath, obj, false);
        }
        
        public static Task WriteJson(this IAdsSymbolicAccess client, string variablePath, JObject obj, bool force)
        {
            return WriteRecursive(client, variablePath, obj, string.Empty, force);
        }

        private static async Task WriteRecursive(this IAdsSymbolicAccess client, string variablePath, JObject parent, string jsonName, bool force = false)
        {
            var symbolInfo = (ITcAdsSymbol5)client.ReadSymbolInfo(variablePath);
            var dataType = symbolInfo.DataType;
            {
                if (dataType.Category == DataTypeCategory.Array)
                {
                    var array = parent.SelectToken(jsonName) as JArray;
                    var elementCount = array.Count < dataType.Dimensions.ElementCount ? array.Count : dataType.Dimensions.ElementCount;
                    for (int i = 0; i < elementCount; i++)
                    {
                        if (dataType.BaseType.ManagedType != null)
                        {
                            await WriteAsync(client, variablePath + $"[{i + dataType.Dimensions.LowerBounds.First()}]", array[i]).ConfigureAwait(false);
                        }
                        else
                        {
                            await WriteRecursive(client, variablePath + $"[{i + dataType.Dimensions.LowerBounds.First()}]", parent, jsonName + $"[{i}]").ConfigureAwait(false);
                        }
                    }
                }
                else if (dataType.ManagedType == null)
                {
                    if (dataType.SubItems.Any())
                    {
                        foreach (var subItem in dataType.SubItems)
                        {
                            if (HasJsonName(subItem, force))
                            {
                                await WriteRecursive(client, variablePath + "." + subItem.SubItemName, parent, string.IsNullOrEmpty(jsonName) ? GetJsonName(subItem) : jsonName + "." + GetJsonName(subItem)).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    await WriteAsync(client, symbolInfo.Name, parent.SelectToken(jsonName)).ConfigureAwait(false);
                }
            }

        }
        
        public static Task<JObject> ReadJson(this IAdsSymbolicAccess client, string variablePath)
        {
            return ReadJson(client, variablePath, false);
        }
        
        public static Task<JObject> ReadJson(this IAdsSymbolicAccess client, string variablePath, bool force)
        {
            return Task.Run(() => ReadRecursive(client, variablePath, new JObject(), GetVaribleNameFromFullPath(variablePath), force:force));
        }

        private static JObject ReadRecursive(IAdsSymbolicAccess client, string variablePath, JObject parent, string jsonName, bool isChild = false, bool force = false)
        {
            var symbolInfo = (ITcAdsSymbol5)client.ReadSymbolInfo(variablePath);
            var dataType = symbolInfo.DataType;
            {
                if (dataType.Category == DataTypeCategory.Array)
                {
                    if (dataType.BaseType.ManagedType != null)
                    {
                        var obj = client.ReadSymbol(symbolInfo);
                        parent.Add(jsonName, new JArray(obj));
                    }
                    else
                    {
                        var array = new JArray();
                        for (int i = dataType.Dimensions.LowerBounds.First(); i <= dataType.Dimensions.UpperBounds.First(); i++)
                        {
                            var child = new JObject();
                            ReadRecursive(client, variablePath + $"[{i}]", child, jsonName, force:force);
                            array.Add(child);
                        }
                        parent.Add(jsonName, array);
                    }
                }
                else if (dataType.ManagedType == null)
                {
                    if (dataType.SubItems.Any())
                    {
                        var child = new JObject();
                        foreach (var subItem in dataType.SubItems)
                        {
                            if (HasJsonName(subItem, force))
                            {
                                ReadRecursive(client, variablePath + "." + subItem.SubItemName, isChild ? child : parent, GetJsonName(subItem), true, force);
                            }
                        }
                        if (isChild)
                        {
                            parent.Add(jsonName, child);
                        }
                    }
                }
                else
                {
                    var obj = client.ReadSymbol(symbolInfo);
                    parent.Add(jsonName, new JValue(obj.TryConvertToDotNetManagedType()));
                }
            }

            return parent;
        }

        public static string GetVaribleNameFromFullPath(this string variablePath)
        {
            return variablePath.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        private static string GetJsonName(ITcAdsSubItem dataType)
        {
            var jsonName = dataType.Attributes.FirstOrDefault(attribute => attribute.Name.Equals("json", StringComparison.InvariantCultureIgnoreCase))?.Value;
            return string.IsNullOrEmpty(jsonName) ? GetVaribleNameFromFullPath(dataType.SubItemName) : jsonName;
        }

        private static bool HasJsonName(this ITcAdsSubItem subItem, bool force = false)
        {
            if (force)
            {
                return true;
            }
            return subItem.Attributes.Any(attribute => attribute.Name.Equals("json", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// try to convert TwinCAT ManagedType to .Net ManagedType (e.g. DT => DateTime, TIME => TimeSpan, etc.)
        /// If no conversion is possible return the unmodified object
        /// </summary>
        /// <param name="obj">object to convert</param>
        /// <returns></returns>
        public static object TryConvertToDotNetManagedType(this object obj)
        {
            return obj.TryConvertDateTime().TryConvertTimeSpan();
        }

        private static object TryConvertDateTime(this object obj)
        {
            bool conversion;
            object newObject;
            if (obj.GetType() == typeof(PlcOpen.DT))
            {
                conversion = PlcOpen.PlcOpenDTConverter.TryConvert(obj as PlcOpen.DT, typeof(DateTime), out newObject);
                if (conversion)
                {
                    return newObject;
                }
            }
            if (obj.GetType() == typeof(PlcOpen.DATE))
            {
                conversion = PlcOpen.PlcOpenDateConverter.TryConvert(obj as PlcOpen.DATE, typeof(DateTime), out newObject);
                if (conversion) 
                {
                    return newObject;
                }
            }
            return obj;
        }
        private static object TryConvertTimeSpan(this object obj)
        {
            bool conversion;
            object newObject;
            if (obj.GetType() == typeof(PlcOpen.TIME))
            {
                conversion = PlcOpen.PlcOpenTimeConverter.TryConvert(obj as PlcOpen.TIME, typeof(TimeSpan), out newObject);
                if (conversion)
                {
                    return newObject;
                }
            }
            if (obj.GetType() == typeof(PlcOpen.LTIME))
            {
                conversion = PlcOpen.PlcOpenTimeConverter.TryConvert(obj as PlcOpen.LTIME, typeof(TimeSpan), out newObject);
                if (conversion)
                {
                    return newObject;
                }
            }

            return obj;
        }
    }
}
