using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VendingMachineWPF.Model
{
	public class Product 
	{
        public string Model { get; set; }
        public string Vendor { get; set; }
        public int Year { get; set; }
        public string ImgLink { get; set; }
    }
}
