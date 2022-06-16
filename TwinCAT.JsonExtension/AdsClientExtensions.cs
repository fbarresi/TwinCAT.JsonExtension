using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.PlcOpen;
using TwinCAT.TypeSystem;

namespace TwinCAT.JsonExtension
{
    public static class AdsClientExtensions
    {
        public static Task WriteAsync<T>(this IAdsSymbolicAccess client, string variablePath, T value)
        {
            return WriteAsync(client, variablePath, value, CancellationToken.None);
        }
        
        public static Task WriteAsync<T>(this IAdsSymbolicAccess client, string variablePath, T value, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var symbolInfo = client.ReadSymbol(variablePath);
                var targetType = (symbolInfo.DataType as IManagedMappableType)?.ManagedType;
                object targetValue;

                if (symbolInfo.DataType.Category == DataTypeCategory.Enum && value is JToken j && j.Type == JTokenType.String)
                {
                    var enumType = symbolInfo.DataType as IEnumType;
                    targetValue = Convert.ChangeType(enumType.Parse(value.ToString()), targetType);
                }
                else if (targetType.Namespace == "TwinCAT.PlcOpen")
                {
                    targetValue = value.TryConvertToPlcOpenType(targetType);
                }
                else
                {
                    targetValue = targetType != null
                        ? (value is JToken jToken) ? jToken.ToObject(targetType) : Convert.ChangeType(value, targetType)
                        : value;
                }
                client.WriteValue(symbolInfo, targetValue);
            }, token);
        }

        public static Task<T> ReadAsync<T>(this IAdsSymbolicAccess client, string variablePath)
        {
            return ReadAsync<T>(client, variablePath, CancellationToken.None);
        }
        
        public static Task<T> ReadAsync<T>(this IAdsSymbolicAccess client, string variablePath, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var symbolInfo = client.ReadSymbol(variablePath);
                var obj = client.ReadValue(symbolInfo);
                return (T) Convert.ChangeType(obj, typeof(T));
            }, token);
        }

        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JArray array, CancellationToken token)
        {
            return WriteJsonAsync(client, variablePath, array, false, token);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JArray array)
        {
            return WriteJsonAsync(client, variablePath, array, false, CancellationToken.None);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JArray array, bool force)
        {
            return WriteArrayAsync(client, variablePath, array, force, CancellationToken.None);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JArray array, bool force, CancellationToken token)
        {
            return WriteArrayAsync(client, variablePath, array, force, token);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JObject obj)
        {
            return WriteJsonAsync(client, variablePath, obj, false, CancellationToken.None);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JObject obj, CancellationToken token)
        {
            return WriteJsonAsync(client, variablePath, obj, false, token);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JObject obj, bool force)
        {
            return WriteRecursiveAsync(client, variablePath, obj, string.Empty, force, CancellationToken.None);
        }
        
        public static Task WriteJsonAsync(this IAdsSymbolicAccess client, string variablePath, JObject obj, bool force, CancellationToken token)
        {
            return WriteRecursiveAsync(client, variablePath, obj, string.Empty, force, token);
        }

        private static async Task WriteArrayAsync(this IAdsSymbolicAccess client, string variablePath, JArray array, bool force, CancellationToken token)
        {
            var symbolInfo = client.ReadSymbol(variablePath);
            var dataType = symbolInfo.DataType as IArrayType;

            if (dataType.Category != DataTypeCategory.Array)
            {
                throw new InvalidOperationException($"Type of plc variable {variablePath} must be an array");
            }
        
            var elementCount = array.Count < dataType.Dimensions.ElementCount ? array.Count : dataType.Dimensions.ElementCount;
            for (int i = 0; i < elementCount; i++)
            {
                if ((dataType.ElementType as IManagedMappableType)?.ManagedType != null)
                {
                    await WriteAsync(client, variablePath + $"[{i + dataType.Dimensions.LowerBounds.First()}]", array[i], token).ConfigureAwait(false);
                }
                else if (dataType.ElementType?.Category == DataTypeCategory.Array)
                {
                    await WriteArrayAsync(client, variablePath + $"[{i + dataType.Dimensions.LowerBounds.First()}]", array[i] as JArray, force, token).ConfigureAwait(false);
                }
                else
                {
                    await WriteRecursiveAsync(client, variablePath + $"[{i + dataType.Dimensions.LowerBounds.First()}]", array[i] as JObject, string.Empty, force, token).ConfigureAwait(false);
                }
            }
        }
        
        private static async Task WriteRecursiveAsync(this IAdsSymbolicAccess client, string variablePath, JObject parent, string jsonName, bool force, CancellationToken token)
        {
            var symbolInfo = client.ReadSymbol(variablePath);
            var dataType = symbolInfo.DataType;
            {
                if (dataType.Category == DataTypeCategory.Array)
                {
                    var arrayType = symbolInfo.DataType as IArrayType;
                    var array = parent.SelectToken(jsonName) as JArray;
                    var elementCount = array.Count < arrayType.Dimensions.ElementCount ? array.Count : arrayType.Dimensions.ElementCount;
                    for (int i = 0; i < elementCount; i++)
                    {
                        if ((arrayType.ElementType as IManagedMappableType)?.ManagedType != null)
                        {
                            await WriteAsync(client, variablePath + $"[{i + arrayType.Dimensions.LowerBounds.First()}]", array[i], token).ConfigureAwait(false);
                        }
                        else
                        {
                            await WriteRecursiveAsync(client, variablePath + $"[{i + arrayType.Dimensions.LowerBounds.First()}]", parent, jsonName + $"[{i}]", force, token).ConfigureAwait(false);
                        }
                    }
                }
                else if ((dataType as IManagedMappableType)?.ManagedType == null)
                {
                    var structType = symbolInfo.DataType as IStructType;
                    if ((bool) structType?.Members.Any())
                    {
                        foreach (var subItem in structType.Members)
                        {
                            if (HasJsonName(subItem, force))
                            {
                                await WriteRecursiveAsync(client, variablePath + "." + subItem.InstanceName, parent, string.IsNullOrEmpty(jsonName) ? GetJsonName(subItem) : jsonName + "." + GetJsonName(subItem), force, token).ConfigureAwait(false); ;
                            }
                        }
                    }
                }
                else
                {
                    var value = parent.SelectToken(jsonName);
                    if (value != null)
                    {
                        await WriteAsync(client, symbolInfo.InstancePath, value, token).ConfigureAwait(false);
                    }
                }
            }
        }
        
        public static Task<JObject> ReadJsonAsync(this IAdsSymbolicAccess client, string variablePath)
        {
            return ReadJsonAsync(client, variablePath, false, false, CancellationToken.None);
        }
        
        public static Task<JObject> ReadJsonAsync(this IAdsSymbolicAccess client, string variablePath, CancellationToken token)
        {
            return ReadJsonAsync(client, variablePath, false, false, token);
        }
        
        public static Task<JObject> ReadJsonAsync(this IAdsSymbolicAccess client, string variablePath, bool force)
        {
            return ReadJsonAsync(client, variablePath, force, false, CancellationToken.None);
        }

        public static Task<JObject> ReadJsonAsync(this IAdsSymbolicAccess client, string variablePath, bool force, bool stringifyEnums)
        {
            return ReadJsonAsync(client, variablePath, force, stringifyEnums, CancellationToken.None);
        }

        public static Task<JObject> ReadJsonAsync(this IAdsSymbolicAccess client, string variablePath, bool force, bool stringifyEnums, CancellationToken token)
        {
            return Task.Run(() => ReadRecursive(client, variablePath, new JObject(), GetVaribleNameFromFullPath(variablePath), force:force, stringifyEnums:stringifyEnums), token);
        }

        private static JObject ReadRecursive(IAdsSymbolicAccess client, string variablePath, JObject parent, string jsonName, bool isChild = false, bool force = false, bool stringifyEnums = false)
        {
            var symbolInfo = client.ReadSymbol(variablePath);
            var dataType = symbolInfo.DataType;
            {
                if (dataType.Category == DataTypeCategory.Array)
                {
                    var arrayType = symbolInfo.DataType as IArrayType;
                    if ((arrayType.ElementType as IManagedMappableType)?.ManagedType != null)
                    {
                        var obj = client.ReadValue(symbolInfo);
                        parent.Add(jsonName, JArray.FromObject(obj));
                    }
                    else
                    {
                        var array = new JArray();
                        for (int i = arrayType.Dimensions.LowerBounds.First(); i <= arrayType.Dimensions.UpperBounds.First(); i++)
                        {
                            var child = new JObject();
                            ReadRecursive(client, variablePath + $"[{i}]", child, jsonName, isChild:false, force:force, stringifyEnums:stringifyEnums);
                            if (child[jsonName] != null && dataType.Category == DataTypeCategory.Array)
                            {
                                array.Add(child[jsonName]);
                            }
                            else
                            {
                                array.Add(child);
                            }
                        }
                        parent.Add(jsonName, array);
                    }
                }
                else if ((dataType as IManagedMappableType)?.ManagedType == null)
                {
                    var structType = symbolInfo.DataType as IStructType;
                    if ((bool) structType?.Members.Any())
                    {
                        var child = new JObject();
                        foreach (var subItem in structType?.Members)
                        {
                            if (HasJsonName(subItem, force))
                            {
                                ReadRecursive(client, variablePath + "." + subItem.InstanceName, isChild ? child : parent, GetJsonName(subItem), isChild:true, force:force, stringifyEnums:stringifyEnums);
                            }
                        }
                        if (isChild)
                        {
                            parent.Add(jsonName, child);
                        }
                    }
                }
                else if(dataType.Category == DataTypeCategory.Enum)
                {
                    var obj = (Int16)client.ReadValue(symbolInfo);
                    if (stringifyEnums)
                    {
                        var enumType = symbolInfo.DataType as IEnumType;
                        parent.Add(jsonName, new JValue(enumType.ToString(obj)));
                    }
                    else
                    {
                        parent.Add(jsonName, new JValue(obj.TryConvertToDotNetManagedType()));
                    }
                }
                else
                {
                    var obj = client.ReadValue(symbolInfo);
                    parent.Add(jsonName, new JValue(obj.TryConvertToDotNetManagedType()));
                }
            }

            return parent;
        }

        public static string GetVaribleNameFromFullPath(this string variablePath)
        {
            return variablePath.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries).Last();
        }

        public static string GetJsonName(this IAttributedInstance dataType)
        {
            var jsonName = dataType.Attributes.FirstOrDefault(attribute => attribute.Name.Equals("json", StringComparison.InvariantCultureIgnoreCase))?.Value;
            return string.IsNullOrEmpty(jsonName) ? GetVaribleNameFromFullPath(dataType.InstanceName) : jsonName;
        }

        public static bool HasJsonName(this IAttributedInstance subItem, bool force)
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

        public static object TryConvertDateTime(this object obj)
        {
            bool conversion;
            object newObject;
            switch (obj)
            {
                case DT dt:
                {
                    return dt.Value;
                }

                case DATE date:
                {
                    conversion = PlcOpenDateConverter.TryConvert(date, typeof(DateTime), out newObject);
                    if (conversion) 
                    {
                        return newObject;
                    }

                    break;
                }
            }

            return obj;
        }
        public static object TryConvertTimeSpan(this object obj)
        {
            bool conversion;
            object newObject;
            switch (obj)
            {
                case TIME time:
                {
                    conversion = PlcOpenTimeConverter.TryConvert(time, typeof(TimeSpan), out newObject);
                    if (conversion)
                    {
                        return newObject;
                    }

                    break;
                }

                case LTIME ltime:
                {
                    conversion = PlcOpenTimeConverter.TryConvert(ltime, typeof(TimeSpan), out newObject);
                    if (conversion)
                    {
                        return newObject;
                    }

                    break;
                }
            }

            return obj;
        }

        public static object TryConvertToPlcOpenType(this object obj, Type targetType)
        {
            if (targetType == typeof(DT) || targetType == typeof(DATE))
            {
                var dateTimeOffset = (DateTimeOffset) ((obj is JToken jToken)
                    ? jToken.ToObject(typeof(DateTimeOffset))
                    : JToken.FromObject(obj).ToObject(typeof(DateTimeOffset)));
                return dateTimeOffset.TryConvertToPlcOpenType(targetType);
            }
            if (targetType == typeof(TIME) || targetType == typeof(LTIME))
            {
                var timeSpan = (TimeSpan) ((obj is JToken jToken)
                    ? jToken.ToObject(typeof(TimeSpan))
                    : JToken.FromObject(obj).ToObject(typeof(TimeSpan)));
                return timeSpan.TryConvertToPlcOpenType(targetType);
            }

            return obj;
        }

        public static object TryConvertToPlcOpenType(this DateTimeOffset dateTimeOffset, Type targetType)
        {
            if (targetType == typeof(DT))
            {
                return new DT(dateTimeOffset);
            }

            if (targetType == typeof(DATE))
            {
                return new DATE(dateTimeOffset);
            }
            
            return dateTimeOffset;
        }
        
        public static object TryConvertToPlcOpenType(this TimeSpan timeSpan, Type targetType)
        {
            if (targetType == typeof(TIME))
            {
                return new TIME(timeSpan);
            }

            if (targetType == typeof(LTIME))
            {
                return new LTIME(timeSpan);
            }
            
            return timeSpan;
        }
    }
}
