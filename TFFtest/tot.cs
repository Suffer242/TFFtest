using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

//namespace MakeAdultBase
//{
    public static class tot
    {

        public static Dictionary<String, String> GetDictionary(String fn)
        {
            Dictionary<String, String> d = new Dictionary<String, String>();

            foreach (var s in File.ReadAllLines(fn,Encoding.GetEncoding(1251)))
            {
                int p = s.IndexOf('=');
                try
                {
                    d.Add(s.Substring(0, p).Trim(), s.Substring(p + 1).Trim());
                }
                catch
                {
                }
            }

            return d;

        }

        public static String TextBetween(this String s, String s1, String s2, String After = null)
        {
            int f=0;
            if (s1 == null || s2 == null || s == null)
                f=10;

            if (s == null) return null;

           

            int searchpos;

            if (String.IsNullOrEmpty(After)) searchpos = 0;
            else searchpos = s.IndexOf(After);

            if (f==0)
            if (searchpos == -1) return null;

            int p1 = s.IndexOf(s1, searchpos); if (p1 == -1) return null;
            int p2 = s.IndexOf(s2, p1 + s1.Length); if (p2 == -1) return null;
            return s.Substring(p1 + s1.Length, p2 - (p1 + s1.Length));
        }

        public static String onlychars(this String s)
        {
            if (String.IsNullOrEmpty(s)) return "";

            StringBuilder a = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsLetter(s, i)) a.Append(s[i]);
            }

            return a.ToString().ToLower();

        }

        public static String onlydchars(this String s)
        {
            if (String.IsNullOrEmpty(s)) return "";

            String ss = s.ToUpper();

            StringBuilder a = new StringBuilder();
            for (int i = 0; i < ss.Length; i++)
            {
                if (Char.IsLetter(ss, i) || Char.IsDigit(ss, i)) a.Append(ss[i]);
            }

            return a.ToString().ToLower();

        }

        public static List<String> AllTextBetween(this String s, String s1, String s2)
        {
            int pos = 0;
            var list = new List<String>();

            int l = s.Length;
            while (true)
            {
                int p1 = s.IndexOf(s1, pos); if (p1 == -1) break;
                int p2 = s.IndexOf(s2, p1); if (p2 == -1) break;

                pos = p2;

                list.Add(s.Substring(p1 + s1.Length, p2 - (p1 + s1.Length)));
            }

            return list;
        }

        public static List<String> TextBetween(this String[] s, String s1, String s2)
        {
            var list = new List<String>();
            foreach (var ss in s)
            {
                String r = ss.TextBetween(s1, s2);
                if (r != null) list.Add(r);
            }
            return list;
        }

        public static String AfterLast(this String s, String substr)
        {
            int i = s.LastIndexOf(substr);
            return i == -1 ? null : s.Substring(i + substr.Length);
        }

        public static String BeforeLast(this String s, String substr)
        {
            int i = s.LastIndexOf(substr);
            return i == -1 ? null : s.Substring(0, i);
        }

        public static String AfterFirst(this String s, String substr)
        {
            int i = s.IndexOf(substr);
            return i == -1 ? null : s.Substring(i + substr.Length );
        }

        public static String BeforeFirst(this String s, String substr)
        {
            if (s==null)
            {
                s = "";
            }

            int i = s.IndexOf(substr);
            return i == -1 ? null : s.Substring(0, i);
        }


    }

//}
