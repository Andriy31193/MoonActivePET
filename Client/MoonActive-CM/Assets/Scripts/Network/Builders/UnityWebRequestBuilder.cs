using UnityEngine.Networking;

namespace Network.Builders
{
    public sealed class UnityWebRequestBuilder
    {
        private readonly UnityWebRequest _www;

        public UnityWebRequestBuilder(string url, string method)
        {
            _www = UnityWebRequest.Post(url, method);
        }

        public UnityWebRequestBuilder SetHeader(string key, string value)
        {
            _www.SetRequestHeader(key, value);
            return this;
        }

        public UnityWebRequestBuilder SetBody(string body)
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(body);
            _www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            return this;
        }

        public UnityWebRequest Build()
        {
            return _www;
        }
    }
}