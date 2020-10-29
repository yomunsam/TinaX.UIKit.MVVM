using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit.MVVM.Exceptions
{
    public class MvvmException : UIKitException
    {
        public MvvmException(string msg, int errorCode) : base($"[TinaX.UIKit.MVVM]{msg}", errorCode)
        {

        }

        public MvvmException(string msg, MvvmErrorCode error) : base(msg, (int)error) { }
    }
}
