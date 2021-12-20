using System;
using System.Collections.Generic;

namespace FFRK_LabMem.Data
{
    class CounterComparers
    {

        static Dictionary<Char, Int32> romanMap = new Dictionary<char, int>
        {
            {'I', 1 },
            {'V', 5},
            {'X', 10},
        };

        static List<string> materialsStrings = new List<string>
        {
            "Rainbow",
            "Rosetta",
            "Adamantite",
            "Scarletite"
        };

        public class HEComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var first = ExtractRealmValue(x);
                var second = ExtractRealmValue(y);
                return first.CompareTo(second);
            }
        }

        public class DropComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var firstCat = GetDropCategoryValue(x);
                var second = GetDropCategoryValue(y);
                var cmp1 = firstCat.CompareTo(second);
                if (cmp1 == 0)
                {
                    return x.CompareTo(y);
                } else
                {
                    return cmp1;
                }
            }
        }

        private static int ExtractRealmValue(String name)
        {
            if (name.EndsWith(")"))
            {
                var start = name.IndexOf('(') + 1;
                var realm = name.Substring(start, name.Length - start - 1);
                if (realm.EndsWith("-DoC")) return 71;
                if (realm.EndsWith("-CC")) return 72;
                if (realm.Equals("FFT")) return 160;
                if (realm.Equals("Type-0")) return 170;
                if (realm.Equals("Beyond")) return 190;
                return ConvertRomanToInt(realm) * 10;
            }
            return 180;
        }

        private static int ConvertRomanToInt(String romanNumeral) {
            var ret = 0;
            for (Int32 index = romanNumeral.Length - 1, last = 0; index >= 0; index--)
            {
                var key = romanNumeral[index];
                if (!romanMap.ContainsKey(key)) return 0;
                var current = romanMap[key];
                ret += (current < last ? -current : current);
                last = current;
            }
            return ret;
        }

        private static int GetDropCategoryValue(String name)
        {
            if (name.Contains("Mote")) return 1;
            if (name.Contains("Crystal") && !name.Contains("Rainbow")) return 2;
            if (name.Contains("Orb")) return 3;
            if (materialsStrings.Exists(s => name.Contains(s))) return 4;
            if (name.Contains("Tail")) return 5;
            if (name.Contains("Arcana")) return 6;
            if (name.Contains("Egg")) return 7;
            return 8;
        }
    }
}
