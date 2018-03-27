namespace fnf.Client.Serialization
{
    interface ISerializer
    {
        string Serialize<T>(T message);

        T Deserialize<T>(string messageBody);
    }
}
