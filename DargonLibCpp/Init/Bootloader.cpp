#include "../dlc_pch.hpp"
#include "../IO.hpp"
#include "Bootloader.hpp"
#include "BootstrapContext.hpp"
#include "BootloaderRemoteLogger.hpp"
using namespace Dargon::Init;
using namespace Dargon::IO::DSP;

void Bootloader::BootstrapInjectedModule(const FunctionInitialize& init, HMODULE moduleHandle)
{
   // Create context object and fill it with parameter data
   BootstrapContext* context = new BootstrapContext();
   std::cout << "Bootloader::BootstrapInjectedModule passed BootstrapContext ctor" << std::endl;
   context->ApplicationModuleHandle = moduleHandle;
   
   // Connect to DSPEx server and get bootstrap arguments
   DSPExNode* dspExNode = new DSPExNode(DSPExNodeRole::Client, "DargonInjectedModule");
   std::cout << "DSPExNode constructed" << std::endl;
   context->DSPExNode = dspExNode;
   
   DSPExNodeSession* session = dspExNode->Connect("Dargon");   
   context->DIMSession = session;
   std::cout << "Bootloader::BootstrapInjectedModule DSPExClient::ConnectLocal passed" << std::endl;
   context->DIMSession->GetBootstrapArguments(context);
   std::cout << "Bootloader::BootstrapInjectedModule GetBootstrapArguments passed" << std::endl;

   // Create a DSPEx Remote Logger
   context->Logger = new BootloaderRemoteLogger(context);
   std::cout << "Bootloader::BootstrapInjectedModule Logger Constructed" << std::endl;

   // Call application's init handler
   init(context);
}