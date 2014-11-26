﻿using System;
using System.IO;

namespace Dargon.Daemon
{
   public class DaemonConfiguration : IDaemonConfiguration
   {
      private const string kUserDataPathName = ".dargon";
      private const string kAppDataSubdirectory = "ItzWarty/Dargon";

      private readonly string userDataDirectoryPath;
      private readonly string appDataDirectoryPath;

      public DaemonConfiguration()
      {
         userDataDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), kUserDataPathName);
         appDataDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), kAppDataSubdirectory);
         

         Directory.CreateDirectory(userDataDirectoryPath);
         Directory.CreateDirectory(appDataDirectoryPath);
      }

      public string UserDataDirectoryPath { get { return userDataDirectoryPath; } }
      public string AppDataDirectoryPath { get { return appDataDirectoryPath; } }

      public bool IsDebugCompilation
      {
         get
         {
#if DEBUG
            return true;
#else
            return false;
#endif
         }
      }
   }
}