using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moxy.Events
{
    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs(T data)
        {
            Data = data;
        }

        public readonly T Data;


        public static implicit operator T(GenericEventArgs<T> genericEventArgsInstance)
        {
            return genericEventArgsInstance.Data;
        }

        public static implicit operator GenericEventArgs<T>(T data)
        {
            return new GenericEventArgs<T>(data);
        }
    }
}
