using System;

namespace Network.Responses
{
    [Serializable]
    public class UserDataResponse
    {
        public bool Success;
        public UserData UserData;
    }
}