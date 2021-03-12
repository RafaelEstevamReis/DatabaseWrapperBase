using System;
using System.Collections.Generic;

namespace Simple.DatabaseWrapper.TypeReader
{
    public class ReaderCachedCollection
    {
        Dictionary<string, TypeInfo> dicTypes;
        public ReaderCachedCollection()
        {
            dicTypes = new Dictionary<string, TypeInfo>();
        }

        public TypeInfo GetInfo<T>()=> getType(typeof(T));
        public TypeInfo GetInfo(Type type)=> getType(type);

        private TypeInfo getType(Type type)
        {
            var key = getTypeKey(type);

            if (dicTypes.ContainsKey(key)) return dicTypes[key];

            var info = TypeInfo.FromType(type);
            dicTypes[key] = info;

            return info;
        }

        public bool HasCacheOf<T>() => HasCacheOf(typeof(T));
        public bool HasCacheOf(Type type)
        {
            var key = getTypeKey(type);
            return dicTypes.ContainsKey(key);
        }

        public bool Remove<T>() => Remove(typeof(T));
        public bool Remove(Type type)
        {
            var key = getTypeKey(type);
            return dicTypes.Remove(key);
        }

        private static string getTypeKey(Type type) => type.FullName;

    }
}
