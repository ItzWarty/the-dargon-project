﻿using ItzWarty;
using ItzWarty.Collections;
using ItzWarty.Processes;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dargon.Processes.Watching
{
   public class ProcessWatcherServiceImpl : ProcessWatcherService
   {
      private static readonly Logger logger = LogManager.GetCurrentClassLogger();

      private readonly IProcessProxy processProxy;
      private readonly IProcessWatcher processWatcher;
      private readonly MultiValueDictionary<string, Action<CreatedProcessDescriptor>> processSpawnedHandlersByProcessName = new MultiValueDictionary<string, Action<CreatedProcessDescriptor>>();

      public ProcessWatcherServiceImpl(IProcessProxy processProxy, IProcessWatcher processWatcher)
      {
         logger.Info("Constructing Process Watching Service");
         this.processProxy = processProxy;
         this.processWatcher = processWatcher;
      }

      public void Initialize()
      {
         logger.Info("Initializing Process Watching Service");
         this.processWatcher.NewProcessFound += HandleProcessWatcherNewProcessFound;
         this.processWatcher.Start();
      }

      internal void HandleProcessWatcherNewProcessFound(object sender, ProcessFoundEventArgs e)
      {
         var lowerProcessName = e.ProcessName.ToLower();
//         logger.Info(lowerProcessName);
         var handlers = processSpawnedHandlersByProcessName.GetValueOrDefault(lowerProcessName);
         if (handlers != null) {
            foreach (var handler in handlers) {
               handler(new CreatedProcessDescriptor(e.ProcessName, e.ProcessID, e.ParentProcessID));
            }
         }
      }

      public void Subscribe(Action<CreatedProcessDescriptor> handler, IEnumerable<string> names, bool retroactive)
      {
         var lowerCaseNames = new ItzWarty.Collections.HashSet<string>(names.Select(FormatProcessName));
         foreach (var lowerCaseName in lowerCaseNames) {
            processSpawnedHandlersByProcessName.Add(lowerCaseName, handler);
         }

         if (retroactive) {
            var processes = processWatcher.FindProcess((p) => lowerCaseNames.Contains(FormatProcessName(p.ProcessName)));
            foreach (var process in processes) {
               handler(new CreatedProcessDescriptor(process.ProcessName, process.Id, processProxy.GetParentProcess(process).Id));
            }
         }
      }

      private string FormatProcessName(string name) { return name.ToLowerInvariant(); }
   }
}
