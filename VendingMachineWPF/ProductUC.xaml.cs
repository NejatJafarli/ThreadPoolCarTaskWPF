using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VendingMachineWPF.Model;

namespace VendingMachineWPF
{
    /// <summary>
    /// Interaction logic for ProductUC.xaml
    /// </summary>
    public partial class ProductUC : UserControl, INotifyPropertyChanged
    {
        private Product product;
        public Product Product
        {
            get { return product; }
            set { product = value; OnPropertyRaised(); }
        }
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private string botText;

        public string BotText
        {
            get { return botText; }
            set { botText = value; OnPropertyRaised(null); }
        }

        private string img;

        public string Img
        {
            get { return img; }
            set { img = value; OnPropertyRaised(null); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public ProductUC(Product myProduct)
        {
            InitializeComponent();
            DataContext = this;
            Product = myProduct;
            BotText = $"{Product.Vendor},{Product.Model},{Product.Year}";
            Img = Product.ImgLink;
        }
    }
}
