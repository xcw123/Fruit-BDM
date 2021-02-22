using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace FruitSortingVtest1
{
    public class MyWebClient : WebClient
    {
        IPEndPoint m_OutIPEndPoint;
        public MyWebClient(IPEndPoint outIp)
        {
            if (outIp == null)
                throw new ArgumentNullException("outIp");

            m_OutIPEndPoint = outIp;
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            Proxy = null;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
            {
                return m_OutIPEndPoint;
            };
            return request;
        }
    }

    public class TimeoutWebClient : WebClient
    {
        IPEndPoint m_OutIPEndPoint;
        int Timeout = 0;
        public TimeoutWebClient(int timeout, IPEndPoint ip)
            : base()
        {
            m_OutIPEndPoint = ip;
            Timeout = timeout;
            CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            Encoding = System.Text.Encoding.UTF8;
            Proxy = null;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            if (m_OutIPEndPoint != null)
            {
                request.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
                {
                    return m_OutIPEndPoint;
                };
            }
            return request;
        }
    }
}
