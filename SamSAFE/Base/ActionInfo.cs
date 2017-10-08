using System;
using System.Collections.Generic;
using System.Text;

namespace SamSAFE.Base
{
    public class ActionInfo
    {
        public ActionInfo(string name, object payload)
        {
            Name = name;
            Data = payload;
        }

        public string __actionId { get; set; }
        public string Name { get; }
        public object Data { get; }
    }
}
