﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBackupLib
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using FreezeFile = BUCommon.FreezeFile;
  using UploadCache = BUCommon.UploadCache;
  using BackupLib;

  [TestClass]
  public class CacheTest
  {
    private UploadCache _buildCache()
    {
      var uc = new UploadCache();

      var ff = new FreezeFile 
        { 
          fileID="blarg"
          , localHash=new BUCommon.Hash { type="SHA0", raw=new byte[] { 0, 22, 44, 11} } 
          , mimeType="application/byte-stream"
          , modified=new DateTime(2016,12,01)
          , path="blarg/blarg1.obj"
          , storedHash=BUCommon.Hash.Create("SHA0", new byte[] { 22,44, 0, 89 })
          , uploaded=new DateTime(2016,12,03)
        };

      uc.add(ff);

      return uc;
    }
    
    [TestMethod]
    public void TestCache()
    {
      var uc = _buildCache();

      var blargdir = uc.getdir("blarg");

      Assert.IsTrue(blargdir.Any());
      var item = blargdir.FirstOrDefault();
      Assert.IsNotNull(item);
      Assert.AreEqual("blarg/blarg1.obj", item.path);
    }

    [TestMethod]
    public void TestCacheWrite()
    {
      var uc = _buildCache();
      uc.write("c:\\tmp\\cachexml.xml");
    }

    [TestMethod]
    public void TestCacheRead()
    {
      var uc = _buildCache();
      uc.write("c:\\tmp\\cachexml.xml");

      uc.read("c:\\tmp\\cachexml.xml");
      var blardir = uc.getdir("blarg");

      Assert.IsNotNull(blardir);
      var item = blardir.FirstOrDefault();
      Assert.IsNotNull(item);
      Assert.AreEqual("blarg/blarg1.obj", item.path);
    }

    [TestMethod]
    public void RegCache()
    {
      var fil = new FileInfoLister();
      fil.init();

      fil.Dispose();
      fil = null;
    }
  }
}
