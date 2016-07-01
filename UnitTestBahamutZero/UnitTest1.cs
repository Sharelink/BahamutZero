using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DBTek.Crypto;
using EasyEncryption;

namespace UnitTestBahamutZero
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(new MD5_Hsr().HashString("test").ToLower(), MD5.ComputeMD5Hash("test"));
            Assert.AreEqual(new SHA1_Hsr().HashString("test"), SHA.ComputeSHA1Hash("test"));
            Assert.AreEqual(PasswordHash.Encrypt.SHA1("test").ToLower(), SHA.ComputeSHA1Hash("test"));
            Assert.AreEqual(PasswordHash.Encrypt.SHA256("test").ToLower(), SHA.ComputeSHA256Hash("test"));
        }
    }
}
