using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter322
{
    public class Variables
    {
        private Dictionary<string, KeyValuePair<DType, object>> variables;
        public Variables()
        {
            variables = new Dictionary<string, KeyValuePair<DType, object>>();
        }

        public void AddVariable(string identifier, DType DType_type, object val)
        {
            variables.Add(identifier, new KeyValuePair<DType, object>(DType_type, val));
        }

        public void AddIdentifier(string identifier, DType DType_type)
        {
            variables.Add(identifier, new KeyValuePair<DType, object>(DType_type, null));
        }

        public void AddValue(string identifier, object val)
        {
            variables[identifier] = new KeyValuePair<DType, object>(variables[identifier].Key, val);
        }

        public DType GetType(string identifier)
        {
            return variables[identifier].Key;
        }

        public object GetValue(string identifier)
        {
            return variables[identifier].Value;
        }

        public bool Exist(string identifier)
        {
            return variables.ContainsKey(identifier);
        }

    }
}
