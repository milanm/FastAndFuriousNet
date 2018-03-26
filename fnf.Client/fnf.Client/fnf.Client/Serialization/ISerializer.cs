using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client.Serialization
{
    interface ISerializer
    {
        string Serialize<T>(T message);

        T Deserialize<T>(string messageBody);
    }
}
