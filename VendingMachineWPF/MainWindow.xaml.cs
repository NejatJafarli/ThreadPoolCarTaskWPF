using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
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
using System.Windows.Threading;
using VendingMachineWPF.Model;

namespace VendingMachineWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public ObservableCollection<ProductUC> ProductsUC { get; set; } = new ObservableCollection<ProductUC>();

        public MyThreadClass MyTC { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;


        }
        private void EscHandle(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Application.Current.Shutdown();

        public void MyActionMethod(Product Prod)
        {
            this.Dispatcher.Invoke(() =>
            {
                var temp = new ProductUC(Prod);
                temp.Window.Width = 220;
                temp.Window.Height = 280;
                ProductsUC.Add(temp);
                for (int i = 0; i < 5; i++)
                    Scroll.LineDown();
            });
        }
        public Stopwatch Watch { get; set; } = new Stopwatch();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductsUC.Clear();

            Watch.Start();

            if (TGbtn.IsChecked.Value)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
                timer.Start();

                var path = $@"C:\Users\{Environment.UserName}\Source\Repos\ThreadPoolCarTaskWPF\VendingMachineWPF\JsonFiles";

                var size = Directory.GetFiles(path).Length;

                for (int i = 1; i <= size; i++)
                {
                    TimerTxt.Text = $"{Watch.Elapsed.Hours}:{Watch.Elapsed.Minutes}:{Watch.Elapsed.Seconds}";

                    var text = File.ReadAllText($@"{path}\{i}.json");
                    var Temp = JsonSerializer.Deserialize<List<Product>>(text);

                    MyTC = new MyThreadClass(Temp);
                    MyTC.MyAction = MyActionMethod;

                    ThreadPool.QueueUserWorkItem(MyTC.Do, Temp);

                }
                Watch.Stop();
                timer.Stop();
            }
            else
            {

            }
        }
        public int U { get; set; } = 0;
        private void Timer_Tick(object sender, EventArgs e)
        {
            MessageBox.Show(U.ToString());
            U++;
        }
    }
}
