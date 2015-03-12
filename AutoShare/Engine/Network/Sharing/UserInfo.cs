using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Common;
using System.Net;
using System.Globalization;

namespace AutoShare.Engine.Network.Sharing
{
    
    [Serializable()]
    public class UserInfo
    {
        #region Helper Classes
        [Serializable()]
        public struct IPHistoryEntry:ISerializable
        {
            public System.Net.IPEndPoint EndPoint;
            public DateTime TimeStart;
            public DateTime TimeEnd;

            #region Constructors
            public IPHistoryEntry(System.Net.IPEndPoint Addr){
                this.EndPoint = Addr;
                this.TimeStart = DateTime.Now;
                this.TimeEnd = DateTime.MinValue;
            }

            public IPHistoryEntry(System.Net.IPEndPoint Addr, DateTime TimeStart){
                this.EndPoint = Addr;
                this.TimeStart = TimeStart;
                this.TimeEnd = DateTime.MinValue;
            }

            public IPHistoryEntry(System.Net.IPEndPoint Addr, DateTime TimeStart, DateTime TimeEnd)
            {
                this.EndPoint = Addr;
                this.TimeStart = TimeStart;
                this.TimeEnd = TimeEnd;
            }
            public IPHistoryEntry(SerializationInfo info, StreamingContext context)
            {
                this.EndPoint = (IPEndPoint)info.GetValue("endp", typeof(IPEndPoint));
                this.TimeEnd = info.GetDateTime("tend");
                this.TimeStart = info.GetDateTime("tstart");
            }

            #endregion
            #region Public Static Methods
            // Handles IPv4 and IPv6 notation.
            public static IPEndPoint CreateIPEndPoint(string endPoint)
            {
                string[] ep = endPoint.Split(':');
                if (ep.Length < 2)
                    throw new FormatException("Invalid endpoint format");
                IPAddress ip;
                if (ep.Length > 2)
                {
                    if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                    {
                        throw new FormatException("Invalid ip address");
                    }
                }
                else
                {
                    if (!IPAddress.TryParse(ep[0], out ip))
                    {
                        throw new FormatException("Invalid ip address");
                    }
                }
                int port;
                if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
                {
                    throw new FormatException("Invalid port");
                }
                return new IPEndPoint(ip, port);
            }
            #endregion

            
            #region APICalls
            public override string ToString()
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes('{' + EndPoint.ToString() + '|' + TimeStart.Ticks + '|' + TimeEnd.Ticks + '}');
                return Convert.ToBase64String(bytes);
            }


            public static IPHistoryEntry FromString(string Base64String)
            {
                FormatException excep = new FormatException("Provided string is corrupt. Use the string from ToString() result");
                string original = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(Base64String));
                if (original.Length < 2 || original[0] != '{' || original[original.Length - 1] != '}')
                    throw excep;
                System.Net.IPEndPoint p = null;
                DateTime st = DateTime.MinValue, end = DateTime.MinValue;
                int mode = 0, last_pos = 0; //mode to grab data
                string copy;
                long longres;
                for (int i = 1, s = original.Length; i < s; ++i)
                {
                    if (original[i] == '|')
                    {
                        //copy
                        copy = original.Substring(last_pos + 1, i - last_pos - 1);
                        last_pos = i;
                        switch (mode)
                        {
                            case 0://IPEndPoint
                                IPEndPoint ip = IPHistoryEntry.CreateIPEndPoint(copy);
                                break;
                            case 1://TimeStart
                                if (!long.TryParse(copy, out longres))
                                    throw new FormatException("TimeStart value has invalid format");
                                else
                                    st = new DateTime(longres);
                                break;
                            default:
                                throw excep;
                        }
                        ++mode;
                    }
                    else if (original[i] == '}')
                    {
                        if (mode == 2) //TimeEnd
                        {
                            copy = original.Substring(last_pos + 1, i - last_pos - 1);
                            if (!long.TryParse(copy, out longres))
                                throw new FormatException("TimeEnd value has invalid format");
                            else
                                end = new DateTime(longres);
                            ++mode;
                        }
                        else
                            throw excep;
                    }
                }
                if(mode != 3) 
                    throw new FormatException("Not all data present in the passed string.");
                return new IPHistoryEntry(p, st, end);
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("endp", this.EndPoint, typeof(IPEndPoint));
                info.AddValue("tend", this.TimeEnd);
                info.AddValue("tstart", TimeStart);
            }
            
        #endregion
            
        }
        #endregion
        #region Private Members
        System.Net.IPEndPoint _addr;
        bool _strict_nat, _log_ips; //this parameter is passed by the user itself
        string _eval_key, _name;
        List<IPHistoryEntry> _addr_history;

        #endregion
        #region Properties
        //Changes frequently
        System.Net.IPEndPoint LastKnownAddress { get { return _addr; } }
        bool StrictNATDetected { get { return _strict_nat; } }
        //Almost Hardcoded
        string EvaluationKey { get { return _eval_key; } }
        string UserName { get { return _name; } }
        //Historical
        bool LogChangeIPs { get { return this._log_ips; } }
        List<IPHistoryEntry> AddressHistory { get { return this._addr_history; } }

        #endregion
        #region Private methods
        string SerializeHistory()
        {
            string history = "";
            foreach (IPHistoryEntry ip in this._addr_history)
            {
                history += ip.ToString() + ',';
            }
            if(history.Length>1)
                history = history.Remove(history.Length - 1); //remove trailing ','
            return history;
        }
        void DeserializeHistory(string str)
        {
            string[] arr = str.Split(',');

            foreach (string s in arr)
                this._addr_history.Add(IPHistoryEntry.FromString(s));
        }
        #endregion
        #region Constructors
        public UserInfo(string UserName, string EvaluationKey = "", System.Net.IPEndPoint Addreess = null, bool LogIpHistory = true)
        {
            this._name = UserName;
            this._eval_key = EvaluationKey;
            this._addr = Addreess;
            this._log_ips = LogIpHistory;
            this._addr_history = new List<IPHistoryEntry>();
        }

        // The special constructor is used to deserialize values.
        public UserInfo(SerializationInfo info, StreamingContext context)
        {
            this._addr_history = new List<IPHistoryEntry>();
            DeserializeHistory(info.GetString("ip_history"));
            this._log_ips = info.GetBoolean("log_ips");
            this._name = info.GetString("name");
            this._eval_key = info.GetString("eval_key");
            this._strict_nat = info.GetBoolean("strict_nat");
        }
        #endregion
        #region APICalls
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ip_history", this.SerializeHistory(), typeof(string));
            info.AddValue("name", this._name, typeof(string));
            info.AddValue("eval_key", this._eval_key, typeof(string) );
            info.AddValue("log_ips", this._log_ips);
            info.AddValue("strict_nat", this._strict_nat);
        }
        #endregion
    }
}
