using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Windows.Controls;
using System.Text;

namespace AutoPowerOff
{
    public partial class MainWindow : Window
    {

        DispatcherTimer timer;

        int wrongPasEnter = 0;
        int PasEnterLeft = 4;
        string PasChgSet = "821491";

        int key = 102;

        public MainWindow()
        {
            InitializeComponent();

            Pas.Focus();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler((o, e) =>
            {
                pBar.Value += 1;
                pBar.Maximum = 60;
            });
            timer.Start();

            ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files (x86)\AutoPowerOff\bat\Restart.bat");
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startInfo.FileName);
            Process.Start(startInfo);
        }

        private void CancelPOff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var PasChgSet = File.ReadAllText(@"C:\Program Files (x86)\AutoPowerOff\PassVault.txt");

                int decrypt = Convert.ToInt32(PasChgSet) ^ key;
                PasChgSet = Convert.ToString(decrypt);

                if (Pas.Password == PasChgSet)
                {
                    //MessageBox.Show("Вошел"); // Comment
                    ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files (x86)\AutoPowerOff\bat\CancelPOff.bat");
                    startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startInfo.FileName);
                    Process.Start(startInfo);
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    wrongPasEnter++;
                    PasEnterLeft = PasEnterLeft - 1;
                    string show = Convert.ToString(PasEnterLeft);
                    MessageBox.Show("Неверный пароль !\nОсталось попыток: " + show);
                    Pas.Clear();
                }
                if (PasEnterLeft == 0)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files (x86)\AutoPowerOff\bat\POffNow.bat");
                    startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startInfo.FileName);
                    Process.Start(startInfo);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void btChPas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Pas.Password == PasChgSet)
            {
                NewPas.Visibility = Visibility.Visible;
                NewPasOk.Visibility = Visibility.Visible;

                timer.Stop();

                    ProcessStartInfo startInfo = new ProcessStartInfo(@"C:\Program Files (x86)\AutoPowerOff\bat\CancelPOff.bat");
                    startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(startInfo.FileName);
                    Process.Start(startInfo);
                }
            else
            {
                MessageBox.Show("Нет прав !");
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void NewPasOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int encrypt = Convert.ToInt32(NewPas.Text) ^ key;

            string text = Convert.ToString(encrypt);
            using (FileStream fstream = new FileStream(@"C:\Program Files (x86)\AutoPowerOff\PassVault.txt", FileMode.Truncate)) //Truncate  //Create
            {
                byte[] array = System.Text.Encoding.Default.GetBytes(text);
                fstream.Write(array, 0, array.Length);

                Pas.Clear();
                NewPas.Clear();
                NewPas.Visibility = Visibility.Hidden;
                NewPasOk.Visibility = Visibility.Hidden;
                Check.Visibility = Visibility.Visible;

                MessageBox.Show("Пароль изменен");
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var PasChgSet = File.ReadAllText(@"C:\Program Files (x86)\AutoPowerOff\PassVault.txt");

            int decrypt = Convert.ToInt32(PasChgSet) ^ key;
            PasChgSet = Convert.ToString(decrypt);

            if (Pas.Password == PasChgSet)
            {
                MessageBox.Show("Пароль декодирован верно");
                Check.Visibility = Visibility.Hidden;
                Pas.Clear();
            }
            else
            {
                MessageBox.Show("Пароль нельзя декодировать !");
                Pas.Clear();
                Check.Visibility = Visibility.Hidden;

                NewPasOk_Click(sender, e);
                Pas.Clear();
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}












