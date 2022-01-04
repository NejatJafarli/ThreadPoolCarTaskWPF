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

            BtnCancel.IsEnabled = false;
            BtnStart.IsEnabled = true;
        }
        private void EscHandle(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Application.Current.Shutdown();
        }
        public CancellationTokenSource MyToken { get; set; }

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
        public Action<object> MyActForSingleCore { get; set; }
        public Action<object, object> MyActForMultiCore { get; set; }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = false;
            BtnCancel.IsEnabled = true;
            MyToken = new CancellationTokenSource();
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

                    MyActForMultiCore = new Action<object, object>(MyTC.DoForMultiCore);
                    MyActForMultiCore.BeginInvoke(Temp, MyToken.Token, (ar) =>
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
                                 this.Dispatcher.Invoke(() =>
                                 {
                                     BtnCancel.IsEnabled = false;
                                     BtnStart.IsEnabled = true;
                                 });
                                 if (MyToken.IsCancellationRequested)
                                     MessageBox.Show("Canceled");
                                 else
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
                MyActForSingleCore = new Action<object>(MyTC.DoForSingleCore);
                MyActForSingleCore.BeginInvoke(MyToken.Token, (ar) =>
                {

                    if (ar.IsCompleted)
                    {
                        Watch.Stop();
                        Watch.Reset();
                        timer.Stop();
                        this.Dispatcher.Invoke(() =>
                        {
                            BtnCancel.IsEnabled = false;
                            BtnStart.IsEnabled = true;
                        });
                        if (MyToken.IsCancellationRequested)
                            MessageBox.Show("Canceled");
                        else
                            MessageBox.Show("END");
                    }
                }, null);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimerTxt.Text = $"{Watch.Elapsed.Hours}:{Watch.Elapsed.Minutes}:{Watch.Elapsed.Seconds}";
        }

        private void LB_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                for (int i = 0; i < 5; i++)
                    Scroll.LineDown();
            else
                for (int i = 0; i < 5; i++)
                    Scroll.LineUp();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (MyToken.IsCancellationRequested == false)
                MyToken.Cancel();
            BtnStart.IsEnabled = true;
            BtnCancel.IsEnabled = false;
        }
    }
}
