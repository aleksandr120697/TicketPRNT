using System;
using System.Collections.Generic;
using System.IO;
using System.Printing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;
using SysTask = System.Threading.Tasks;

namespace TicketPrint
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        private string SaveData;
        private static string data;
        private static string time;
        private static int number;
        private bool printComplite = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            clearBack();
            SaveData = Properties.Settings.Default.data;
            string NewData = DateTime.Now.ToString("dd/MM/yy");
            if(SaveData != NewData)
            {
                Properties.Settings.Default.data = NewData;
                SaveData = NewData;
                Properties.Settings.Default.num = 0;
                Properties.Settings.Default.Save();
            }
            number = Properties.Settings.Default.num;
        }
        public void clearBack()
        {
            double backupDay = 1;
            string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory+@"\Documents");
            try
            {
                string[] direct = Directory.GetDirectories(backupDir);
                 foreach (string item in direct )
                {
                    DateTime dateTime = Directory.GetLastWriteTime(item);
                    if (dateTime < DateTime.Now - TimeSpan.FromDays(backupDay))
                    {
                        Directory.Delete(item);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private void SaveNumber(int number)
        {
            Properties.Settings.Default.num = number;
            Properties.Settings.Default.Save();
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            number++;
            time = DateTime.Now.ToString("HH/mm/ss");
            data = DateTime.Now.ToString("dd/MM/yy");
            this.numberBox.Text = number.ToString();
            this.dataBox.Text = data.ToString();
            this.timeBox.Text = time.ToString();
            //StartAnimation();
            SaveNumber(number);

            var helper = new WordHelper("Documents/talon.docx");
            var items = new Dictionary<string, string>
            {
                {"<NUM>",number.ToString()},
                {"<DAT>",data },
                {"<TIME>",time },
            };
            string docPath = helper.Process(items);
            if (docPath != null)
            {
                if (helper.Print(docPath) == false)
                {
                    MessageBox.Show("При печати произошла ошибка.\n Пожалуйста сообщите об этом сотруднику. \n Ваш номер талона: " + number + "");
                }
                else
                {
                    var comp = System.Environment.MachineName;
                    PrintServer myPrintServer = new PrintServer(@"\\DESKTOP-0F07K48" /*+ comp*/);
                    PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                    try
                    {
                        foreach (PrintQueue pq in myPrintQueues)
                        {
                            pq.Refresh();
                            PrintJobInfoCollection pCollection = pq.GetPrintJobInfoCollection();
                            foreach (PrintSystemJobInfo job in pCollection)
                            {
                                SpotTroubleUsingJobAttributes(job);
                            }
                            if (printComplite)
                            {
                                SysTask.Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                            }

                        }
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }
            }
            else
            {
                return;
            }
        }
        public void SpotTroubleUsingJobAttributes(PrintSystemJobInfo theJob)
        {
            if ((theJob.JobStatus & PrintJobStatus.Blocked) == PrintJobStatus.Blocked)
            {
                //list.Items.Add("Задание заблокировано.");
            }
            if (((theJob.JobStatus & PrintJobStatus.Completed) == PrintJobStatus.Completed)
                ||
                ((theJob.JobStatus & PrintJobStatus.Printed) == PrintJobStatus.Printed))
            {
                //list.Items.Add("Работа закончена. Попросите пользователя перепроверить все выходные ячейки и убедиться, что проверяется правильный принтер.");
                printComplite = true;
            }
            if (((theJob.JobStatus & PrintJobStatus.Deleted) == PrintJobStatus.Deleted)
                ||
                ((theJob.JobStatus & PrintJobStatus.Deleting) == PrintJobStatus.Deleting))
            {
                //list.Items.Add( "Пользователь или кто-либо другой, обладающий правами администратора очереди, удалил задание. Оно должно быть отправлено повторно.");
            }
            if ((theJob.JobStatus & PrintJobStatus.Error) == PrintJobStatus.Error)
            {
                //list.Items.Add("В работе произошла ошибка.");
            }
            if ((theJob.JobStatus & PrintJobStatus.Offline) == PrintJobStatus.Offline)
            {
                //list.Items.Add("Принтер отключен от сети. Попросите пользователя подключить его к сети с помощью передней панели принтера.");
            }
            if ((theJob.JobStatus & PrintJobStatus.PaperOut) == PrintJobStatus.PaperOut)
            {
                //list.Items.Add("В принтере закончилась бумага нужного формата для выполнения задания. Попросите пользователя добавить бумагу.");
            }

            //if (((theJob.JobStatus & PrintJobStatus.Paused) == PrintJobStatus.Paused)
            //    ||
            //    ((theJob.HostingPrintQueue.QueueStatus & PrintQueueStatus.Paused) == PrintQueueStatus.Paused))
            //{
            //    HandlePausedJob(theJob);
            //    //HandlePausedJob is defined in the complete example.
            //}

            if ((theJob.JobStatus & PrintJobStatus.Printing) == PrintJobStatus.Printing)
            {
                //list.Items.Add("Сейчас задание печатается.");
            }
            if ((theJob.JobStatus & PrintJobStatus.Spooling) == PrintJobStatus.Spooling)
            {
                //list.Items.Add("Сейчас работа сворачивается.");
            }
            if ((theJob.JobStatus & PrintJobStatus.UserIntervention) == PrintJobStatus.UserIntervention)
            {
                //list.Items.Add("Принтер нуждается в вмешательстве человека.");
            }

        }


    }
    
}
