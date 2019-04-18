using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace BMC.CarDashboard.Models
{
    class QuranClass
    {
    }

    #region content

    public class Surah
    {
        public string index { get; set; }
        public string name { get; set; }
        public dynamic verse { get; set; }
        public int count { get; set; }
        public JuzIndex[] juz { get; set; }
    }

   
    public class JuzIndex
    {
        public string index { get; set; }
        public VerseIndex verse { get; set; }
    }

    public class VerseIndex
    {
        public string start { get; set; }
        public string end { get; set; }
    }

    #endregion

    #region translation

    public class Translation
    {
        public string name { get; set; }
        public string index { get; set; }
        public dynamic verse { get; set; }
        public string count { get; set; }
    }

    #endregion

    #region Juz

    public class Juz
    {
        public string index { get; set; }
        public StartSurah start { get; set; }
        public EndSurah end { get; set; }
    }

    public class StartSurah
    {
        public string index { get; set; }
        public string verse { get; set; }
        public string name { get; set; }
    }

    public class EndSurah
    {
        public string index { get; set; }
        public string verse { get; set; }
        public string name { get; set; }
    }

    #endregion

    #region reciters

    public class Reciter
    {
        public int idx { get; set; }
        public string name { get; set; }
        public string mediaurl { get; set; }
    }

    #endregion

    #region SurahIndex
    public class SurahIndex
    {
        public string place { get; set; }
        public string type { get; set; }
        public int count { get; set; }
        public string title { get; set; }
        public string index { get; set; }
        public string pages { get; set; }
        public JuzData[] juz { get; set; }
    }

    public class JuzData
    {
        public string index { get; set; }
        public VerseData verse { get; set; }
    }

    public class VerseData
    {
        public string start { get; set; }
        public string end { get; set; }
    }
    #endregion

}
