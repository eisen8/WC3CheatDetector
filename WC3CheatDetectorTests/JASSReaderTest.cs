using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WC3CheatDetector.JASS;

namespace WC3CheatDetectorTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_EmptyString_Exception()
        {
            Assert.Throws<ArgumentNullException>(() => new JSearch(""));
        }

        [Test]
        public void Constructor_NullString_Exception()
        {
            Assert.Throws<ArgumentNullException>(()=>new JSearch(null));
        }

        [Test]
        public void FindMatches_NullString_Exception()
        {
            JSearch t = new JSearch("test");
            Assert.Throws<ArgumentNullException>(() => t.FindMatches(null));
        }

        [Test]
        public void FindMatches_EmptySearch_Exception()
        {
            JSearch t = new JSearch("test test");
            Assert.Throws<ArgumentNullException>(() => t.FindMatches(""));
        }

        [Test]
        public void FindMatches_SingleSimpleMatch_ReturnsMatch()
        {
            JSearch t = new JSearch("test ab test");

            List<JMatch> m = t.FindMatches("ab");

            Assert.AreEqual(1, m.Count);
            Assert.True(m[0].Success);
            Assert.AreEqual("ab", m[0].Value);
            Assert.AreEqual(5, m[0].Index);
        }

        [Test]
        public void FindMatches_SingleSimpleMatchIgnoreCase_ReturnsMatch()
        {
            JSearch t = new JSearch("test AB test");

            List<JMatch> m = t.FindMatches("ab", RegexOptions.IgnoreCase);

            Assert.AreEqual(1, m.Count);
            Assert.True(m[0].Success);
            Assert.AreEqual("AB", m[0].Value);
            Assert.AreEqual(5, m[0].Index);
        }

        [Test]
        public void FindMatches_MultipleSimpleMatches_ReturnsMatches()
        {
            JSearch t = new JSearch("test ab ab ab ab ab test");

            List<JMatch> m = t.FindMatches("ab");

            Assert.AreEqual(5, m.Count);
        }

        [Test]
        public void FindMatches_MultipleSimpleMatches2_ReturnsMatches()
        {
            JSearch t = new JSearch("ab test ab ab ab test ab ab");

            List<JMatch> m = t.FindMatches("ab");

            Assert.AreEqual(6, m.Count);
        }

        [Test]
        public void FindMatches_MultipleComplicatedWSMatches_ReturnsMatches()
        {
            JSearch t = new JSearch("match - m atch - ma tch - mat ch - matc  h - m a t c h - m    a    t     c    h");

            List<JMatch> m = t.FindMatches("match");

            Assert.AreEqual(7, m.Count);
            Assert.AreEqual("match", m[0].Value);
            Assert.AreEqual(0, m[0].Index);

            Assert.AreEqual("m atch", m[1].Value);
            Assert.AreEqual(8, m[1].Index);

            Assert.AreEqual("ma tch", m[2].Value);
            Assert.AreEqual(17, m[2].Index);

            Assert.AreEqual("mat ch", m[3].Value);
            Assert.AreEqual(26, m[3].Index);

            Assert.AreEqual("matc  h", m[4].Value);
            Assert.AreEqual(35, m[4].Index);

            Assert.AreEqual("m a t c h", m[5].Value);
            Assert.AreEqual(45, m[5].Index);

            Assert.AreEqual("m    a    t     c    h", m[6].Value);
            Assert.AreEqual(57, m[6].Index);
        }

        [Test]
        public void FindMatches_MultipleComplicatedTabsMatches_ReturnsMatches()
        {
            JSearch t = new JSearch("match - m\tatch - ma\ttch - mat\tch - matc\t\th - m\ta\tt\tc\th - m\t\t\ta\t\tt\t\t\t\tc\t \t \th");

            List<JMatch> m = t.FindMatches("match");

            Assert.AreEqual(7, m.Count);
            Assert.AreEqual("match", m[0].Value);
            Assert.AreEqual(0, m[0].Index);

            Assert.AreEqual("m\tatch", m[1].Value);
            Assert.AreEqual(8, m[1].Index);

            Assert.AreEqual("ma\ttch", m[2].Value);
            Assert.AreEqual(17, m[2].Index);

            Assert.AreEqual("mat\tch", m[3].Value);
            Assert.AreEqual(26, m[3].Index);

            Assert.AreEqual("matc\t\th", m[4].Value);
            Assert.AreEqual(35, m[4].Index);

            Assert.AreEqual("m\ta\tt\tc\th", m[5].Value);
            Assert.AreEqual(45, m[5].Index);

            Assert.AreEqual("m\t\t\ta\t\tt\t\t\t\tc\t \t \th", m[6].Value);
            Assert.AreEqual(57, m[6].Index);
        }

        [Test]
        public void FindMatches_MultipleNewLineMatches_ReturnsMatches()
        {
            JSearch t = new JSearch("match\n match\r match\n\r match\r\n match\n\n\r\r\r\n\n\r");

            List<JMatch> m = t.FindMatches("match\n");

            Assert.AreEqual(5, m.Count);
            Assert.AreEqual("match\n", m[0].Value);
            Assert.AreEqual(0, m[0].Index);

            Assert.AreEqual("match\r", m[1].Value);
            Assert.AreEqual(7, m[1].Index);

            Assert.AreEqual("match\n\r", m[2].Value);
            Assert.AreEqual(14, m[2].Index);

            Assert.AreEqual("match\r\n", m[3].Value);
            Assert.AreEqual(22, m[3].Index);

            Assert.AreEqual("match\n\n\r\r\r\n\n\r", m[4].Value);
            Assert.AreEqual(30, m[4].Index);
        }

        [Test]
        public void FindMatches_ComplicatedSearchMatches_ReturnsMatches()
        {
            JSearch t = new JSearch("This is a red herring line\r Activator = 'red herring'\r Another herring \n");

            List<JMatch> m = t.FindMatches("activator='.*'", RegexOptions.IgnoreCase);

            Assert.AreEqual(1, m.Count);
            Assert.AreEqual("Activator = 'red herring'", m[0].Value);
            Assert.AreEqual(28, m[0].Index);
        }

        [Test]
        public void FindMatches_ComplicatedSearchMatchesMultiLine_ReturnsMatches()
        {
            JSearch t = new JSearch("This is a red herring line\r Activator = 'red herring'\r Another herring \n");

            List<JMatch> m = t.FindMatches("^activator='.*'$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            Assert.AreEqual(1, m.Count);
            Assert.AreEqual("Activator = 'red herring'", m[0].Value);
            Assert.AreEqual(28, m[0].Index);
        }

        [Test]
        public void FindMatch_NoMatch_ReturnsNoMatch()
        {
            JSearch t = new JSearch("abcd");

            JMatch m = t.FindMatch("hi");

            Assert.IsFalse(m.Success);
        }

        [Test]
        public void FindMatch_ComplexMultipleMatches_ReturnsFirstMatch()
        {
            JSearch t = new JSearch("a123 b123 c123");

            JMatch m = t.FindMatch("[abc]123", RegexOptions.IgnoreCase);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("a123", m.Value);
            Assert.AreEqual(0, m.Index);
        }

        [Test]
        public void FindMatch_SimpleMultipleMatches_ReturnsFirstMatch()
        {
            JSearch t = new JSearch("xbob bob bobx");

            JMatch m = t.FindMatch("bob", RegexOptions.IgnoreCase);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("bob", m.Value);
            Assert.AreEqual(1, m.Index);
        }


        [Test]
        public void FindMatch_ComplicatedSearchMatches_ReturnsMatch()
        {
            JSearch t = new JSearch("This is a red herring line\r Activator = 'red herring'\r Another herring \n");

            JMatch m = t.FindMatch("activator='.*'", RegexOptions.IgnoreCase);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("Activator = 'red herring'", m.Value);
            Assert.AreEqual(28, m.Index);
        }


        [Test]
        public void FindMatch_ComplicatedSearchMatchesMultiLine_ReturnsMatch()
        {
            JSearch t = new JSearch("This is a red herring line\r Activator = 'red herring'\r Another herring \n");

            JMatch m = t.FindMatch("^activator='.*'$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("Activator = 'red herring'", m.Value);
            Assert.AreEqual(28, m.Index);
        }

        [Test]
        public void FindMatchStartIndex_NoMatch_ReturnsNoMatch()
        {
            JSearch t = new JSearch("abcd");

            JMatch m = t.FindMatch("hi", 0);

            Assert.IsFalse(m.Success);
        }

        [Test]
        public void FindMatchStartIndex_MatchAtStartIndex_ReturnsMatch()
        {
            JSearch t = new JSearch("abcdefg");

            JMatch m = t.FindMatch("bcdefg", 1);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("bcdefg", m.Value);
            Assert.AreEqual(1, m.Index);
        }

        [Test]
        public void FindMatchStartIndex_StartIndexZero_ReturnsMatch()
        {
            JSearch t = new JSearch(" abcdefg ");

            JMatch m = t.FindMatch("bcdefg", 0);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("bcdefg", m.Value);
            Assert.AreEqual(2, m.Index);
        }

        [Test]
        public void FindMatchStartIndex_StartIndexLastIndex_ReturnsNoMatch()
        {
            JSearch t = new JSearch("abcdefg");

            JMatch m = t.FindMatch("bcdefg", 6);

            Assert.IsFalse(m.Success);
        }

        [Test]
        public void FindMatchStartIndex_ComplexMultipleMatches_ReturnsFirstMatchPastStartIndex()
        {
            JSearch t = new JSearch("a123 b123 c123");

            JMatch m = t.FindMatch("[abc]123", 1);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("b123", m.Value);
            Assert.AreEqual(5, m.Index);
        }

        [Test]
        public void FindMatchStartIndex_SimpleMultipleMatches_ReturnsFirstMatch()
        {
            JSearch t = new JSearch("xbob bob bobx");

            JMatch m = t.FindMatch("bob", 3);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("bob", m.Value);
            Assert.AreEqual(5, m.Index);
        }

        [Test]
        public void FindMatchStartIndex_SimpleMultipleWSAndMatches_ReturnsFirstMatch()
        {
            JSearch t = new JSearch("xbob          bob               bobx");

            JMatch m = t.FindMatch("bob", 3);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("bob", m.Value);
            Assert.AreEqual(14, m.Index);
        }

        [Test]
        public void FindMatchStartIndex_SimpleMultipleMatchesNewLines_ReturnsFirstMatch()
        {
            JSearch t = new JSearch("xbob\n\r\n\r\n\r bob \n\r\n\r\n\rbobx");

            JMatch m = t.FindMatch("bob", 3);

            Assert.IsTrue(m.Success);
            Assert.AreEqual("bob", m.Value);
            Assert.AreEqual(11, m.Index);
        }

        [Test]
        public void Insert_InsertString_InsertsString()
        {
            JSearch t = new JSearch("aaaaaaaa");

            t.Insert(4, "bb");

            Assert.AreEqual("aaaabbaaaa", t.Input);
        }

        [Test]
        public void Insert_InsertStringAtStart_InsertsString()
        {
            JSearch t = new JSearch("aaaaaaaa");

            t.Insert(0, "bb");

            Assert.AreEqual("bbaaaaaaaa", t.Input);
        }

        [Test]
        public void Insert_InsertStringNearEnd_InsertsString()
        {
            JSearch t = new JSearch("aaaaaaaa");

            t.Insert(7, "bb");

            Assert.AreEqual("aaaaaaabba", t.Input);
        }

        [Test]
        public void Insert_InsertStringAtEnd_InsertsString()
        {
            JSearch t = new JSearch("aaaaaaaa");

            t.Insert(8, "bb");

            Assert.AreEqual("aaaaaaaabb", t.Input);
        }
    }
}