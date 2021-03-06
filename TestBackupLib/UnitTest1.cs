﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBackupLib
{
  [TestClass]
  public class BBlazeTest
  {
    public static string BASEPATH()
    { return "c:\\users\\salfors\\source\\b2app\\b2app"; }
    
    [TestMethod]
    public void B2LoginTest()
    {
      var accts = BackupLib.AccountBuilder.BuildAccounts();
      var acct = accts.accounts.FirstOrDefault();

      if (acct == null)
        {
          var contents = string.Empty;
          {
            var keyf = new System.IO.FileStream(System.IO.Path.Combine(BASEPATH(), "b2.key"), System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            var strm = new System.IO.StreamReader(keyf);
            while(!strm.EndOfStream) 
              { var str = strm.ReadLine(); if (!string.IsNullOrWhiteSpace(str) && str[0] != '#') { contents = str; } }

            strm.Close();
            keyf = null;
            contents = contents.Trim();
          }

          acct = accts.create("b2");
          acct.connStr = contents;
          acct.svcName = "CommB2.Connection";
          BackupLib.AccountBuilder.Load(accts, acct);
        }
      BackupLib.AccountBuilder.Save(accts);

      //acct.service.authorize();
    }

    [TestMethod]
    public void B2ContainerListTest()
    {
      BUCommon.AccountList accts = BackupLib.AccountBuilder.BuildAccounts();
      var acct = accts.accounts.FirstOrDefault();
      
      Assert.IsNotNull(acct);
      Assert.AreEqual("CommB2.Connection", acct.svcName);

      Assert.IsNotNull(acct.service);
      //acct.service.open();

      var conts = acct.service.getContainers();
      Assert.IsNotNull(conts);
    }

    [TestMethod]
    public void B2UploadTest()
    {
      BUCommon.AccountList accts = BackupLib.AccountBuilder.BuildAccounts();
      var acct = accts.accounts.FirstOrDefault();
      Assert.AreEqual("CommB2.Connection", acct.svcName);
      acct.service.authorize();

      var cont = acct.service.getContainers().FirstOrDefault();
      
      Assert.IsNotNull(cont);
      string srcfile = "c:\\tmp\\photos\\2016\\1231-newyears\\DSC06562.ARW";
      var ff = new BUCommon.FreezeFile { path="2016/1231-newyears/DSC06562.ARW" };

      var rsa = BackupLib.KeyLoader.LoadRSAKey("c:\\tmp\\id_rsa_1_pub");
      var fe = new BackupLib.FileEncrypt(rsa);
      var fs = ff.readStream("c:\\tmp\\photos");
      var encstrm = fe.encrypt(fs);
      var sha1 = System.Security.Cryptography.SHA1Cng.Create();

      var res = sha1.ComputeHash(encstrm);
      encstrm.Seek(0, System.IO.SeekOrigin.Begin);

      DateTime now = DateTime.Now;
      var td = acct.service.threadStart();
      acct.service.uploadFile(td, cont, ff, encstrm, null);
      acct.service.threadStop(td);
      Assert.AreEqual(2017,ff.uploaded.Year);
      Assert.AreEqual(now.Month, ff.uploaded.Month);
      Assert.AreEqual(now.Day, ff.uploaded.Day);
    }

    [TestMethod]
    public void UploadCacheTest()
    {
      BUCommon.FileCache uc = new BUCommon.FileCache();
      uc.load("C:\\tmp\\photos.cache");

      var files = uc.getdir("2016");
      Assert.IsNotNull(files);
      Assert.IsTrue(files.Any());
    }
  }
}
