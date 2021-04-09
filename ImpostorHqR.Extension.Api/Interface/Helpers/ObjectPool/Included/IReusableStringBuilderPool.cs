using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included
{
    public interface IReusableStringBuilderPool
    {
        IReusableStringBuilder Get();
        void Return(IReusableStringBuilder obj);
    }
}
