using JapanischTrainer.Data;
using JapanischTrainer.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JapanischTrainer.Controller
{
    public static class InsertController
    {
        #region Fields

        private static int itemsLearned = 0;
        private const int itemsProInterval = 20;

        #endregion

        #region Initialize

        public static void LoadLessons(Lesson[] selectedLessons)
        {            
            InsertData.Lessons = selectedLessons;

            DataManager.LoadSentences(selectedLessons);
            InsertData.Sentences = AppData.Sentences;

            AppData.Sentences = null;

            Util.SortByRandom(InsertData.Sentences);

            GetNextSentence();
        }

        public static void Deinitialize()
        {
            InsertData.Sentences = null;
            InsertData.SentenceText = null;
            InsertData.SentenceAnswer = null;
            InsertData.Lessons = null;
        }

        #endregion

        #region Practice

        public static bool CheckAnswer(String answer)
        {
            return answer == InsertData.SentenceAnswer;
        }

        public static void GetNextSentence()
        {
            String[] parts = InsertData.ActiveSentence.text.Split('_');
            String[] insertParts = InsertData.ActiveSentence.inserts.Split('_');
            String[] hintParts = InsertData.ActiveSentence.hints.Split('_');

            int insertIndex = Util.GetRandomNumber(insertParts.Length);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < insertIndex; ++i)
            {
                sb.Append(parts[i]);
                sb.Append(insertParts[i]);
            }

            sb.Append(parts[insertIndex]);
            if (hintParts[insertIndex].Length > 0)
            {
                sb.Append("（");
                sb.Append(hintParts[insertIndex]);
                sb.Append("->）");
            }

            sb.Append(" _____ ");

            for (int i = insertIndex + 1; i < insertParts.Length; ++i)
            {
                sb.Append(parts[i]);
                sb.Append(insertParts[i]);
            }

            sb.Append(parts[parts.Length - 1]);

            InsertData.SentenceText = sb.ToString();
            InsertData.SentenceAnswer = insertParts[insertIndex];

            ++InsertData.ItemIndex;
            InsertData.ItemIndex %= InsertData.Sentences.Length;
        }

        #endregion
    }
}
