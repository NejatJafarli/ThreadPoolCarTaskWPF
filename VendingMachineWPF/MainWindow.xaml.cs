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

        public DispatcherTimer timer { get; set; } = new DispatcherTimer();
        public Stopwatch Watch { get; set; } = new Stopwatch();
        public MyThreadClass MyTC { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
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
                for (int i = 0; i < 10; i++)
                    Scroll.LineDown();
            });
        }
        public int TimerCounter { get; set; } = 0;
        public Action<object> MyAct { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductsUC.Clear();
            var path = $@"C:\Users\{Environment.UserName}\Source\Repos\ThreadPoolCarTaskWPF\VendingMachineWPF\JsonFiles";
            var size = Directory.GetFiles(path).Length;
            if (TGbtn.IsChecked.Value)
            {
                timer.Start();
                Watch.Start();
                for (int i = 1; i <= size; i++)
                {
                    var text = File.ReadAllText($@"{path}\{i}.json");
                    var Temp = JsonSerializer.Deserialize<List<Product>>(text);

                    MyTC = new MyThreadClass(Temp);
                    MyTC.MyAction = MyActionMethod;

                    MyAct = new Action<object>(MyTC.DoForMultiCore);
                    MyAct.BeginInvoke(Temp, (ar) =>
                    {
                        if (ar.IsCompleted)
                        {
                            TimerCounter++;
                            if (TimerCounter == size)
                            {
                                Watch.Stop();
                                Watch.Reset();
                                timer.Stop();
                                TimerCounter = 0;
                                MessageBox.Show("END");
                            }
                        }
                    }, null);
                }
            }
            else
            {
                timer.Start();
                Watch.Start();
                MyTC = new MyThreadClass(path);
                MyTC.MyAction = MyActionMethod;
                MyAct =new Action<object>(MyTC.DoForSingleCore);
                MyAct.BeginInvoke(null, (ar) =>
                {

                    if (ar.IsCompleted)
                    {
                        Watch.Stop();
                        Watch.Reset();
                        timer.Stop();
                        MessageBox.Show("END");
                    }
                },null);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerTxt.Text = $"{Watch.Elapsed.Hours}:{Watch.Elapsed.Minutes}:{Watch.Elapsed.Seconds}";
        }
    }
}
