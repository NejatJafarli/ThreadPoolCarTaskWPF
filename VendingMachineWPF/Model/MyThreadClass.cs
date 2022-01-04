using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VendingMachineWPF.Model
{
    public class MyThreadClass
    {
        public Action<Product> MyAction { get; set; }

        public List<Product> Cars { get; set; }
        public string Path { get; set; }
        public MyThreadClass(List<Product> cars)
        {
            Cars = cars;
        }
        public MyThreadClass(string Path)
        {
            this.Path = Path;
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public void DoForMultiCore(object o)
        {
            var Cars = o as List<Product>;

            for (int k = 0; k < Cars.Count; k++)
            {
                Thread.Sleep(300);
                MyAction.Invoke(Cars[k]);
            }
        }

        public void DoForSingleCore(object o)
        {
            var Size = Directory.GetFiles(Path).Length;
            for (int i = 1; i <= Size; i++)
            {
                var text = File.ReadAllText($@"{Path}\{i}.json");
                var Temp = JsonSerializer.Deserialize<List<Product>>(text);
                for (int k = 0; k < Temp.Count; k++)
                {
                    Thread.Sleep(300);
                    MyAction.Invoke(Temp[k]);
                }
            }
        }
        ~MyThreadClass()
        {
            Dispose();
        }
    }
}
