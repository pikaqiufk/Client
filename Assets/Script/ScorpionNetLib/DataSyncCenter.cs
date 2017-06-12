
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using DataContract;
using ProtoBuf;
using ServiceBase;

namespace ScorpionNetLib
{
    public class DataSyncCenter
    {
        private Dictionary<int, Dictionary<ulong, Dictionary<uint, Action<byte[]>>>> mTargetBindings = new Dictionary<int, Dictionary<ulong, Dictionary<uint, Action<byte[]>>>>();

        public void RequestSyncData<T>(ServiceType type, ulong characterId, uint id, Action<T> setter) where T : class, IExtensible
        {
            RequestSyncDataImpl(type, characterId, id, ms => { setter(Serializer.Deserialize<T>(ms)); });
        }

        private byte[] mBuffer = new byte[8];

        public void RequestSyncData(ServiceType type, ulong characterId, uint id, Action<int> setter)
        {
            RequestSyncDataImpl(type, characterId, id, ms =>
            {
                ms.Read(mBuffer, 0, 4);
                setter(SerializerUtility.ReadInt(mBuffer));
            });
        }
        public void RequestSyncData(ServiceType type, ulong characterId, uint id, Action<uint> setter)
        {
            RequestSyncDataImpl(type, characterId, id, ms =>
            {
                ms.Read(mBuffer, 0, 4);
                setter((uint)SerializerUtility.ReadInt(this.mBuffer));
            });
        }

        public void RequestSyncData(ServiceType type, ulong characterId, uint id, Action<ulong> setter)
        {
            RequestSyncDataImpl(type, characterId, id, ms =>
            {
                ms.Read(mBuffer, 0, 8);
                setter((ulong)SerializerUtility.ReadLong(this.mBuffer));
            });
        }
        public void RequestSyncData(ServiceType type, ulong characterId, uint id, Action<long> setter)
        {
            RequestSyncDataImpl(type, characterId, id, ms =>
            {
                ms.Read(mBuffer, 0, 8);
                setter(SerializerUtility.ReadLong(this.mBuffer));
            });
        }

        public void RequestSyncData(ServiceType type, ulong characterId, uint id, Action<float> setter)
        {
            RequestSyncDataImpl(type, characterId, id, ms =>
            {
                ms.Read(mBuffer, 0, 4);
                setter(SerializerUtility.ReadInt(this.mBuffer) / 10000.0f);
            });
        }

        public void RequestSyncData(ServiceType type, ulong characterId, uint id, Action<string> setter)
        {
            RequestSyncDataImpl(type, characterId, id, ms => { setter(Serializer.Deserialize<TString>(ms).Data); });
        }

        private void RequestSyncDataImpl(ServiceType type, ulong characterId, uint id, Action<MemoryStream> setter)
        {
            Dictionary<ulong, Dictionary<uint, Action<byte[]>>> serviceDict;
            if (!mTargetBindings.TryGetValue((int) type, out serviceDict))
            {
                serviceDict = new Dictionary<ulong, Dictionary<uint, Action<byte[]>>>();
                mTargetBindings.Add((int)type, serviceDict);
            }

            Dictionary<uint, Action<byte[]>> dict;
            if (!serviceDict.TryGetValue(characterId, out dict))
            {
                dict = new Dictionary<uint, Action<byte[]>>();
                serviceDict.Add(characterId, dict);
            }

            dict[id] = (data) =>
            {
                using (var ms = new MemoryStream(data, false))
                {
                    setter(ms);
                }
            };
        }

        public void StopSyncData(ServiceType type, ulong characterId, uint id)
        {
            Dictionary<ulong, Dictionary<uint, Action<byte[]>>> serviceDict;
            if (!mTargetBindings.TryGetValue((int)type, out serviceDict))
            {
                serviceDict = new Dictionary<ulong, Dictionary<uint, Action<byte[]>>>();
                mTargetBindings.Add((int)type, serviceDict);
            }

            Dictionary<uint, Action<byte[]>> dict;
            if (serviceDict.TryGetValue(characterId, out dict))
            {
                dict.Remove(id);
                if (dict.Count == 0)
                {
                    serviceDict.Remove(characterId);
                }
            }
        }

        public void ApplySync(ServiceType serviceType, SyncData syncData)
        {
            Dictionary<ulong, Dictionary<uint, Action<byte[]>>> serviceDict;
            if (mTargetBindings.TryGetValue((int) serviceType, out serviceDict))
            {
                var __list4 = syncData.Datas;
                var __listCount4 = __list4.Count;
                for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var data = __list4[__i4];
                    {
                        Dictionary<uint, Action<byte[]>> dict;
                        if (serviceDict.TryGetValue(data.CharacterId, out dict))
                        {
                            Action<byte[]> action;
                            if (dict.TryGetValue(data.Id, out action))
                            {
                                action(data.Data);
                            }
                        }
                    }
                }
            }
        }

        public void ApplySync(SceneSyncData syncData)
        {
            Dictionary<ulong, Dictionary<uint, Action<byte[]>>> serviceDict;
            if (mTargetBindings.TryGetValue((int) ServiceType.Scene, out serviceDict))
            {
                var __list4 = syncData.Datas;
                var __listCount4 = __list4.Count;
                for (int __i4 = 0; __i4 < __listCount4; ++__i4)
                {
                    var data = __list4[__i4];
                    {
                        Dictionary<uint, Action<byte[]>> dict;
                        if (serviceDict.TryGetValue(data.CharacterId, out dict))
                        {
                            Action<byte[]> action;
                            if (dict.TryGetValue(data.Id, out action))
                            {
                                action(data.Data);
                            }
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            foreach (var item in mTargetBindings)
            {
                item.Value.Clear();
            }

            mTargetBindings.Clear();
        }

    }
}