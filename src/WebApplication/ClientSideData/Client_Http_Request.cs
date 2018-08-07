using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer; 

namespace WebApplication.ClientSideData
{
    public class Client_Http_Request : Client_Side_Data
    {
        public string Method { get; set; }
        public string Path { get; set; }

        // Constructor that will take in values that are taken from Http_Request to make
        // a Detailed_Http_Request object.
        public Client_Http_Request(Http_Request h)
        {
            this.Method = h.method;
            this.Path = h.path;

            this.StartTimestamp = h.timestamp; 
        }
    }
}
