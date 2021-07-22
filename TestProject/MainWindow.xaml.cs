using System;
using System.Collections.Generic;
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
using System.Text.Json;

namespace TestProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string vowels = "АЕЁИОУЫЭЮЯаеЁиоуыэюяaeiouAEIOUÄÖÜäöüÆæØø"; // строка с гласными буквами


        public class JText // класс для десериализирования json
        {
            public string text { get; set; }
        }


        public class List // класс для заполнения таблицы
        {
            public string text { get; set; }
            public int count_words { get; set; }
            public int count_vowel_letters { get; set; }
        }


        int GetCountWords(string text) // функция подсчёта количества слов
        {
            return text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        int GetCountVowelLetters(string text , string vowels) // функция подсчёта количества гласных букв
        {
            int count = 0;
            for (int i = 0; i < text.Length; i++)
            {
                for (int j = 0; j < vowels.Length; j++)
                {
                    if (vowels[j] == text[i]) count++;
                }
            }
            return count;
        }

        private string GET(int id) // функция get запроса к серверу
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("http://tmgwebtest.azurewebsites.net/api/textstrings/" + id.ToString()); // Создание запроса
            req.Headers.Add("TMG-Api-Key", "0J/RgNC40LLQtdGC0LjQutC4IQ==");
            try // Отправка запроса
            {
                System.Net.WebResponse resp = req.GetResponse();
                System.IO.Stream stream = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(stream);
                string Out = sr.ReadToEnd(); // Получения ответа
                sr.Close();
                var obj = JsonSerializer.Deserialize<JText>(Out); // десериализирование json
                return obj.text;
            }
            catch (Exception e) // обработка ошибок
            {
                MessageBox.Show("Request: " + req.RequestUri.ToString() + "  Error: " + e.Message);
                return null;
            }
        }

        int[] TextSlise() // функция нарезания текста из id_textbox на индификаторы
        {
            string str = id_textbox.Text;
            string[] smas = str.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            int[] imas = new int[smas.Length];
            int c = 0;
            bool duplicate = false;
            for (int i = 0; i < smas.Length; i++)
            {
                int buf;
                if (!int.TryParse(smas[i], out buf)) // поиск не числовых значений
                {
                    MessageBox.Show("Значение индефикатора " + smas[i] + " не является числом");
                    continue;
                }
                for (int j = 0; j < c; j++) // поиск дублирующихся индификаторов
                {
                    if (imas[j] == buf) duplicate = true;
                }
                c++;
                if (!duplicate)
                    imas[i] = buf;
                        
                duplicate = false;
            }
            return imas;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // функция обработки нажатия на кнопку
        {
            list.Items.Clear();
            int[] id = TextSlise();
            foreach (var item in id)
            {
                if (item == 0) continue;
                string text = GET(item);
                if (text != null)
                {
                    List d = new List
                    {
                        text = text,
                        count_words = GetCountWords(text),
                        count_vowel_letters = GetCountVowelLetters(text, vowels)
                    };
                    list.Items.Add(d);
                }
            }
        }
    }
}
