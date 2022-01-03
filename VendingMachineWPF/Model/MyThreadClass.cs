using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VendingMachineWPF.Model
{
    public class MyThreadClass
    {
        public Action<Product> MyAction { get; set; }

        public List<Product> Cars { get; set; }
        public MyThreadClass(List<Product> cars)
        {
            Cars = cars;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public void Do(object o)
        {
            var Cars = o as List<Product>;

            for (int k = 0; k < Cars.Count; k++)
            {
                Thread.Sleep(2000);
                MyAction.Invoke(Cars[k]);
            }
        }
        ~MyThreadClass()
        {
            Dispose();
        }
    }
}
