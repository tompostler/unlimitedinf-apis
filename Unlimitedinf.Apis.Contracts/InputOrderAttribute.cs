using System;

namespace Unlimitedinf.Apis.Contracts
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class InputOrderAttribute : Attribute
    {
        public int Order { get; }

        public InputOrderAttribute(int inputOrder)
        {
            this.Order = inputOrder;
        }
    }
}
