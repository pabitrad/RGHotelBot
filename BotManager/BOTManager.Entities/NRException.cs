using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities
{
    public class NRException : Exception 
    {
        public NRException(string message)
        : base(message) { }

    }
}
