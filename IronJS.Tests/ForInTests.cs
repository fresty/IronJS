﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class ForInTests
    {
        [TestMethod]
        public void TestForIn()
        {
            Assert.AreEqual(
                "abc",
                ScriptRunner.Run(
                    "foo = { a: 1, b: 2, c: 3}; "
                    + "for(key in foo) { emit(key); }"
                )
            );
        }

        [TestMethod]
        public void TestForInAccessingPropertiesByKeyProducedByLoop()
        {
            Assert.AreEqual(
                "123",
                ScriptRunner.Run(
                    "foo = { a: 1, b: 2, c: 3}; "
                    + "for(key in foo) { emit(foo[key]); }"
                )
            );
        }

        [TestMethod]
        public void TestForInNested()
        {
            Assert.AreEqual(
                "abcd",
                ScriptRunner.Run(
                    "foo = { bar: { a: 1, b: 2 }, boo: { c: 3, d: 4 } }; "
                    + "for(k1 in foo) { for(k2 in foo[k1]) { emit(k2); } }"
                )
            );
        }

        [TestMethod]
        public void TestForInBreak()
        {
            Assert.AreEqual(
                "a",
                ScriptRunner.Run(
                    "foo = { a: 1, b: 2, c: 3}; "
                    + "for(key in foo) { emit(key); break; }"
                )
            );
        }

        [TestMethod]
        public void TestForInContinue()
        {
            Assert.AreEqual(
                "ac",
                ScriptRunner.Run(
                    "foo = { a: 1, b: 2, c: 3}; "
                    + "for(key in foo) { if(key == 'b') continue; emit(key); }"
                )
            );
        }

        [TestMethod]
        public void TestForInLabelledBreak()
        {
            Assert.AreEqual(
                "a",
                ScriptRunner.Run(
                    "foo = { bar: { a: 1, b: 2 }, boo: { c: 3, d: 4 } }; "
                    + "outer: for(k1 in foo) { inner: for(k2 in foo[k1]) { if(k2 == 'b') break outer; emit(k2); } }"
                )
            );
        }

        [TestMethod]
        public void TestForInLabelledContinue()
        {
            Assert.AreEqual(
                "acd",
                ScriptRunner.Run(
                    "foo = { bar: { a: 1, b: 2 }, boo: { c: 3, d: 4 } }; "
                    + "outer: for(k1 in foo) { inner: for(k2 in foo[k1]) { if(k2 == 'b') continue outer; emit(k2); } }"
                )
            );
        }
    }
}