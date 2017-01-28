﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackupLib
{
  public interface IFileLister
  {
    IReadOnlyList<FreezeFile> getList(string root);
  }
}
