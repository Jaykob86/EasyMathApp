using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.IO;
using System.Text.RegularExpressions;

namespace EasyMathApp
{

    public partial class MainWindow : Window
    {

        int ROUNDS = 50;
        int THEHIGHESTNUMBER = 20;
        int MAXRESULT = 100;
        const string SETTINGSFILE = "settings.txt";
        const string LOGFILE = "log.txt";

        List<string> resultList = new List<string>();
        bool challengeRunning = false;
        Stopwatch stopwatch = new Stopwatch();
        int roundCounter = 0;

        int num1 = 0;
        int num2 = 0;
        string opr = "";
        int result = 0;
        int userResultInt = 0;

        public MainWindow()
        {

            InitializeComponent();

            readSettings();

            textRounds.Text = ROUNDS.ToString();
            textHighestnum.Text = THEHIGHESTNUMBER.ToString();
            textMaxres.Text = MAXRESULT.ToString();
        }


        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            labelTimer.Content = "";
            labelResult.Content = "";
            if (labelNum1.Content.ToString() == "")
            {
                easyMath(out num1, out num2, out opr, out result);
                populateTextBoxes(num1, num2, opr);
                return;
            }

            if (int.TryParse(textResult.Text, out userResultInt) || textResult.Text == "")
            {
                if (textResult.Text == "") userResultInt = 0;
                textResult.Text = "";
                if (result == userResultInt)
                {
                    labelResult.Content = "Správně";
                }
                else
                {
                    labelResult.Content = "Špatně";
                }

                if (challengeRunning == false)
                {
                    labelRunning.Content = "";
                    easyMath(out num1, out num2, out opr, out result);
                    populateTextBoxes(num1, num2, opr);
                }

                if (challengeRunning == true)
                {
                    roundCounter++;

                    if (result != userResultInt)
                    {
                        resultList.Add($"{num1.ToString()} {opr} {num2.ToString()} = {result.ToString()} (Tvůj výsledek: {userResultInt.ToString()})\n");
                    }

                    if (roundCounter < ROUNDS)
                    {
                        easyMath(out num1, out num2, out opr, out result);
                        populateTextBoxes(num1, num2, opr);
                    }
                    else if (roundCounter == ROUNDS)
                    {
                        stopwatch.Stop();
                        labelResult.Content = "";
                        challengeRunning = false;
                        labelTimer.Content = stopwatch.Elapsed.ToString(@"mm\:ss");
                        labelRunning.Content = "Hotovo!";
                        easyMath(out num1, out num2, out opr, out result);
                        populateTextBoxes(num1, num2, opr);
                        if (resultList.Count > 0)
                        {
                            MessageBox.Show($"{string.Join("", resultList.ToArray())}", "Chybné výsledky");
                        }
                        else
                        {
                            MessageBox.Show("Žádná chyba! Skvělý výsledek!");
                        }
                        List<string> logList = new List<string>();
                        logList.Add($@"{DateTime.Now}, 
                                        Počet chyb: {resultList.Count},
                                        Počet kol: {ROUNDS}, 
                                        Nejvyšší číslo: {THEHIGHESTNUMBER}, 
                                        Nejvyšší výsledek: {MAXRESULT}");
                            foreach (var item in resultList)
                            {
                            logList.Add(item);
                            }
                        logList.Add(@"---------------------------------------------------------------------------");
                        File.AppendAllLines(LOGFILE, logList);
                    }
                }
            }
        }

        public void easyMath(out int num1, out int num2, out string opr, out int result)
        {
            num1 = 0;
            num2 = 0;
            opr = "";
            result = 0;

            int oprDecision = 0;
            Random rnd = new Random();

            do
            {
                num1 = rnd.Next(0, THEHIGHESTNUMBER);
                num2 = rnd.Next(0, THEHIGHESTNUMBER);
                oprDecision = rnd.Next(0, 4);
                switch (oprDecision)
                {
                    case 0:
                        result = num1 + num2;
                        opr = "+";
                        break;
                    case 1:
                        result = num1 - num2;
                        opr = "-";
                        break;
                    case 2:
                        result = num1 * num2;
                        opr = "*";
                        break;
                    default:
                        result = num1 + num2;
                        opr = "+";
                        break;
                }
            } while (result < 0 || result > MAXRESULT);
        }

        public void populateTextBoxes(int num1, int num2, string opr)
        {
            labelNum1.Content = num1.ToString();
            labelNum2.Content = num2.ToString();
            labelOperator.Content = opr;
        }

        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            roundCounter = 0;
            labelResult.Content = "";
            labelTimer.Content = "";
            labelRunning.Content = "Čas běží!";
            stopwatch.Reset();
            challengeRunning = true;
            resultList.Clear();
            easyMath(out num1, out num2, out opr, out result);
            populateTextBoxes(num1, num2, opr);
            stopwatch.Start();
        }

        private void textRounds_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textRounds.Text, out ROUNDS))
            {
                roundCounter = 0;
                labelResult.Content = "";
                labelTimer.Content = "";
                stopwatch.Reset();
                resultList.Clear();
                labelRunning.Content = "";
                changeOfSettings();
            }
        }

        private void textMaxres_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textMaxres.Text, out MAXRESULT))
            {
                roundCounter = 0;
                labelResult.Content = "";
                labelTimer.Content = "";
                stopwatch.Reset();
                resultList.Clear();
                labelRunning.Content = "";
                changeOfSettings();
            }
        }

        private void textHighestnum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(textHighestnum.Text, out THEHIGHESTNUMBER))
            {
                roundCounter = 0;
                labelResult.Content = "";
                labelTimer.Content = "";
                stopwatch.Reset();
                resultList.Clear();
                labelRunning.Content = "";
                changeOfSettings();
            }
        }

        public void changeOfSettings()
        {
            string[] settingsArray = { $"Rounds: {textRounds.Text}", $"Highest number: {textHighestnum.Text}", $"Maximal result: {textMaxres.Text}"};
            File.WriteAllLines(SETTINGSFILE, settingsArray, Encoding.UTF8);
        }

        public void readSettings()
        {
            if (File.Exists(SETTINGSFILE))
            {
                string[] settingsStr = File.ReadAllLines(SETTINGSFILE, Encoding.UTF8);
                for (int i = 0; i < settingsStr.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            int.TryParse(Regex.Match(settingsStr[i], @"\d+").ToString(), out ROUNDS);
                            break;
                        case 1:
                            int.TryParse(Regex.Match(settingsStr[i], @"\d+").ToString(), out THEHIGHESTNUMBER);
                            break;
                        case 2:
                            int.TryParse(Regex.Match(settingsStr[i], @"\d+").ToString(), out MAXRESULT);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
