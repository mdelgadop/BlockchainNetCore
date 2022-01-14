using System.Text.Json;

namespace JumaCoin.Business.classes.Helpers
{
    public class SerializeHelper<T>
        where T: class
    {
        public string Serialize(T data)
        {
            string jsonString = JsonSerializer.Serialize<T>(data);
            return jsonString;
        }

        public virtual T Deserialize(string data)
        {
            T jsonString = JsonSerializer.Deserialize<T>(data);
            return jsonString;
        }

    }
}