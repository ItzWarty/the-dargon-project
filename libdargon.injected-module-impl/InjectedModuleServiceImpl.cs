﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using Dargon.Processes.Injection;
using Dargon.Transport;
using ItzWarty.Services;
using NLog;

namespace Dargon.InjectedModule
{
   public class InjectedModuleServiceImpl : InjectedModuleService
   {
      private static readonly Logger logger = LogManager.GetCurrentClassLogger();

      private readonly IServiceLocator serviceLocator;
      private readonly ProcessInjectionService processInjectionService;
      private readonly ISessionFactory sessionFactory;
      private readonly IInjectedModuleServiceConfiguration injectedModuleServiceConfiguration;
      private readonly ConcurrentDictionary<int, ISession> sessionsByProcessId = new ConcurrentDictionary<int, ISession>();

      public InjectedModuleServiceImpl(IServiceLocator serviceLocator, ProcessInjectionService processInjectionService, ISessionFactory sessionFactory, IInjectedModuleServiceConfiguration injectedModuleServiceConfiguration)
      {
         logger.Info("Initializing Injected Module Service");

         this.serviceLocator = serviceLocator;
         this.processInjectionService = processInjectionService;
         this.sessionFactory = sessionFactory;
         this.injectedModuleServiceConfiguration = injectedModuleServiceConfiguration;
      }

      public void Initialize()
      {
         serviceLocator.RegisterService(typeof(InjectedModuleService), this);

         var injectedDllPath = injectedModuleServiceConfiguration.GetInjectedDllPath();
         if (!File.Exists(injectedDllPath)) {
            logger.Warn("Injected DLL does not exist at " + injectedDllPath + "!");
         }
      }

      public ISession InjectToProcess(int processId, IInjectedModuleConfiguration configuration) 
      {
         logger.Info("Injecting to process " + processId);

         var session = sessionFactory.CreateSession(processId, configuration);
         processInjectionService.InjectToProcess(processId, injectedModuleServiceConfiguration.GetInjectedDllPath());
         sessionsByProcessId.AddOrUpdate(processId, session, (a, b) => session);
         session.Ended += HandleSessionEnded;
         return session;
      }

      internal void HandleSessionEnded(ISession session, SessionEndedEventArgs sessionEndedEventArgs)
      {
         session.Ended -= HandleSessionEnded;

         ISession removedSession;
         sessionsByProcessId.TryRemove(session.ProcessId, out removedSession);
      }
   }
}
