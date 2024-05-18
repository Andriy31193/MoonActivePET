using Network.Enums;

namespace Network.Request.Data
{
    public class RandomUserData : UserData
    {
        public override Endpoint NetworkEndpoint => Endpoint.GetRandomUser;

        public RandomUserData(string username, params UserField[] userFields) : base(username, userFields) {}
    }
}