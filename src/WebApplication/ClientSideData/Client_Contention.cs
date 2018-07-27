using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataTransfer;

namespace WebApplication.ClientSideData
{
    public class Client_Contention : Client_Side_Data
    {
        // Constructor that will take in values that are taken from Http_Request to make
        // a Detailed_Http_Request object.
        public Client_Contention(Contention c)
        {
            this.StartTimestamp = c.timestamp; //will always be the start time when using this constructor
        }
    }
}