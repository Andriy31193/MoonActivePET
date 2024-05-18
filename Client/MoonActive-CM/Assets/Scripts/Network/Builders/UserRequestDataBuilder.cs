using System.Collections.Generic;
using Network.Enums;
using Network.Request.Data;

namespace Network.Builders
{
    public sealed class UserRequestDataBuilder
    {
        private readonly string _username;
        private readonly List<UserField> _fields = new List<UserField>();

        public UserRequestDataBuilder(string username, params UserField[] fields)
        {
            _username = username;
            _fields.AddRange(fields);
        }

        public void AddField(UserField field) => _fields.Add(field);
        public void AddField(string key, OperationType operationType, string value = "") => AddField(new UserField(key, operationType, value));

        public UserData Build() => new UserData(_username, _fields.ToArray());
    }
}