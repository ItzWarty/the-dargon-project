#pragma once
#include "stdafx.h"
#include <windef.h>
#include <memory>
#include "base.hpp"

namespace dargon { namespace Init {
   class bootstrap_context;
} }

namespace dargon {
   class Application {
      static HMODULE module_handle;
      static HANDLE main_thread_handle;

   public:
      static void HandleDllEntry(HMODULE hModule);
      static uint32_t WINAPI BootstrappingThreadStart(void* throwaway);
      static void Initialize(std::shared_ptr<const dargon::Init::bootstrap_context> context);

   private:
   };
}