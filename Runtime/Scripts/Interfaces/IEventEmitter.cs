using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinaX.UIKit.MVVM.Interfaces
{
    public interface IEventEmitter
    {
        Action OnEvent { get; set; }
    }
}
