using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IctBaden.Stonehenge
{
    using System.Linq;

    public class AppSessionCache
    {
        public static string InstanceId = Guid.NewGuid().ToString("N");

        public static readonly Dictionary<Guid, AppSession> Cache = new Dictionary<Guid, AppSession>();

        [Flags]
        public enum ReuseSessionStrategy
        {
            None = 0,
            ClientAddress = 0x0001,
            Cookie = 0x0002
        }

        public static ReuseSessionStrategy ReuseSessions = ReuseSessionStrategy.None;

        public static AppSession NewSession()
        {
            lock (Cache)
            {
                var session = new AppSession();
                Cache.Add(session.Id, session);
                Trace.TraceInformation("New AppSession created: {0}, {1} sessions", session.Id, Cache.Count);
                return session;
            }
        }

        public static AppSession GetSession(Guid id)
        {
            lock (Cache)
            {
                return Cache.ContainsKey(id) ? Cache[id] : null;
            }
        }

        public static AppSession GetSessionByStackId(string id)
        {
            lock (Cache)
            {
                return Cache.Select(ce => ce.Value).FirstOrDefault(session => session.StackId == id);
            }
        }

        public static AppSession GetSessionByIpAddress(string address)
        {
            lock (Cache)
            {
                return Cache.Where(s => s.Value.ClientAddress == address).Select(s => s.Value).FirstOrDefault();
            }
        }

        public static void RemoveSession(Guid id)
        {
            lock (Cache)
            {
                if (Cache.ContainsKey(id))
                {
                    Trace.TraceInformation("Remove AppSession: {0}, {1} sessions", id, Cache.Count - 1);
                    Cache.Remove(id);
                }
            }
        }

        public static void Terminate()
        {
            lock (Cache)
            {
                foreach (var cachedSession in Cache)
                {
                    cachedSession.Value.Terminate();
                }
                Cache.Clear();
            }
        }
    }
}