using System;
using System.Net;

namespace IctBaden.Stonehenge2.Test.Hosting
{
    public class RedirectableWebClient : WebClient
    {
        public new string DownloadString(string address)
        {
            for (var redirect = 0; redirect < 10; redirect++)
            {
                var request = (HttpWebRequest)WebRequest.Create(address);
                request.AllowAutoRedirect = false;
                var response = GetWebResponse(request);
                if (response == null) return null;

                var redirUrl = response.Headers["Location"];
                response.Close();

                var newAddress = new Uri(request.RequestUri, redirUrl).AbsoluteUri;
                if (newAddress == address)
                    break;

                address = newAddress;
            }

            return base.DownloadString(address);
        }
    }
}