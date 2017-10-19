using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpellCheck_PN;

namespace SpellChecker_PN_Tests
{
    [TestClass]
    public class SpellCheckersTests
    {
        SpellChecker spellChecker;

        [TestInitialize]
        public void Initialize()
        {
            this.spellChecker = new SpellChecker();
        }

        [TestMethod]
        public void Dictionary_IsInitialized()
        {
            var word = "теменужка";
            var correctWords = spellChecker.Correct(word, "bg");

            Assert.AreEqual(1, correctWords.Count);
        }

        [TestMethod]
        public void InputValue_WhenCorrect_IsReturned()
        {
            var word = "обичам";

            var correctWords = spellChecker.Correct(word, "bg");

            Assert.AreEqual("обичам", correctWords[0]);
            Assert.AreEqual(1, correctWords.Count);
        }

        [TestMethod]
        public void InputValue_WhenOneLetterIsMissed_IsReturned()
        {
            var word = "обчам";

            var correctWords = spellChecker.Correct(word, "bg");

            CollectionAssert.Contains(correctWords, "обичам");
        }

        [TestMethod]
        public void InputValue_WhenOneLetterIsAdded_IsReturned()
        {
            var word = "снякк";

            var correctWords = spellChecker.Correct(word, "bg");

            CollectionAssert.Contains(correctWords, "сняг");
            Assert.AreEqual(10, correctWords.Count);
        }

        [TestMethod]
        public void InputValue_WhenHaveCappitalLetters_IsCorrectedAsWell()
        {
            var word = "вИнагИ";

            var correctWords = spellChecker.Correct(word, "bg");

            CollectionAssert.Contains(correctWords, "винаги");
            Assert.AreEqual(1, correctWords.Count);
        }

        [TestMethod]
        public void InputValue_WhenNotInDictionary_IsNotCorrected()
        {
            var word = "мушмла";

            var correctWords = spellChecker.Correct(word, "bg");

            CollectionAssert.DoesNotContain(correctWords, "мушмула");
        }
    }
}
