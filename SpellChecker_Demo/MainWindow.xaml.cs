using System;
using SpellCheck_Deletions;

using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Text;

namespace SpellChecker_Demo
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

        //public static void Correct_Deletions(string input, string language)
        //{
        //    List<string> suggestions = null;

        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    var spellChecker = new SpellCheck_Deletions.SpellChecker();

        //    suggestions = spellChecker.Correct(input, "bg", 3);
        //    stopWatch.Stop();
        //    Debug.WriteLine(stopWatch.ElapsedMilliseconds.ToString() + " ms");

        //    foreach (var suggestion in suggestions)
        //    {
        //        Debug.WriteLine("SHOSHO " + suggestion);
        //    }
        //}

        //public static void Correct(string input)
        //{
        //    var spelling = new SpellCheck_PN.SpellChecker();
        //    string word = "";

        //    word = "тряба"; //Shocking - it's inside the bg_frequently used words.
        //    Debug.WriteLine("{0} => {1}", word, spelling.Correct(word, "bg"));

        //    word = "программа"; // 'correcter' is not in the dictionary file so this doesn't work
        //    Debug.WriteLine("{0} => {1}", word, spelling.Correct(word, "bg"));

        //    word = "гришки";
        //    Debug.WriteLine("{0} => {1}", word, spelling.Correct(word, "bg"));

        //    word = "Обчам";
        //    Debug.WriteLine("{0} => {1}", word, spelling.Correct(word, "bg"));

        //    // A sentence
        //    string sentence = "Врме е да заввали сняк"; 
        //    string correction = "";
        //    foreach (string item in sentence.Split(' '))
        //    {
        //        correction += " " + spelling.Correct(item, "bg");
        //    }
        //    Debug.WriteLine("Did you mean:" + correction);

        //    //Debug.Read();
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    Correct_Deletions("тряба", "");

        //    Debug.WriteLine("SHOSHOSHOSHOSHOSHOHSOS HSOHSOSHOSH");

        //    Correct("тряба");
        //}
    }

}
