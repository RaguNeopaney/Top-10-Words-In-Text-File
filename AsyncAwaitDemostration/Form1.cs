using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncAwaitDemostration
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, EventArgs e) => Close();

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            MostUsedWordBox.Items.Clear();

            try
            {
                var file = openFileDialog.FileName;
                List<Task> tasks = new List<Task>();
                
                var wordTask = ReadBook(file);
                tasks.Add(wordTask);

                await Task.WhenAll(tasks);
                // Other operation after this finish 
            }
            catch (Exception ex)
            {

                throw new Exception("File didn't open correctly");
            }
        }

        private async Task ReadBook(string fileName)
        {
            var lines = await Task.FromResult(File.ReadAllLinesAsync(fileName));
            var arrayLinesWithoutPunctuation =
                lines.Result.Where(x => x != String.Empty)
                .AsParallel().
                Select(x => x.ToLower().Trim().Replace("x", "").Replace("'", "").Replace("{", "").Replace("}", ""));
            var arrayOfWords = arrayLinesWithoutPunctuation.SelectMany(x => x.Split(" "));
            Dictionary<string, int> wordDic = new Dictionary<string, int>();
            arrayOfWords.ToList().ForEach(x =>
            {
                if (wordDic.ContainsKey(x))
                {
                    wordDic[x]++;
                }
                else
                {
                    wordDic.Add(x, 1);
                }
            });

            var top10 = wordDic.Where(x => !string.IsNullOrWhiteSpace(x.Key)).OrderByDescending(x => x.Value).Take(10);
            foreach (var word in top10)
            {
                MostUsedWordBox.Items.Add(word.Key + " - " + word.Value);
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                FilePath.Text = openFileDialog.FileName.Split("\\").LastOrDefault();
            }
        }
    }
}
