#pragma once 

#include <string>
#include <thread>
#include <functional>
#include <mutex>
#include "io/IPCObject.hpp"
#include "IDSPExFrameTransmitter.hpp"

namespace dargon { namespace IO { namespace DSP {
   class DSPExFrameTransmitter : public IDSPExFrameTransmitter
   {
      typedef void(*FunctionThreadStart)();
   public:
      /// <summary>
      /// Initializes a new instance of a Frame Transmitter for DSPEx
      /// </summary>
      DSPExFrameTransmitter(std::shared_ptr<dargon::IO::IoProxy> ioProxy);

      /// <summary>
      /// Connects to a remote endpoint
      /// </summary>
      bool Connect(std::string pipeName);
      
      /// <summary>
      /// Begins receiving message frames.
      /// </summary>
      /// <param name="onFrameReceived">
      /// When a message frame is received, this callback is invoked.
      /// </param>
      void BeginReceivingMessageFrames(FrameReceivedHandler onFrameReceived, int threadCount);
      
      /// <summary>
      /// Sends a raw buffer to the remote endpoint
      /// </summary>
      /// <param name="buffer">
      /// The buffer which contains the data we're sending
      /// </param>
      /// <param name="offset">
      /// The offset in the buffer where our data starts
      /// </param>
      /// <param name="size">
      /// The length of the data in our buffer
      /// </param>
      void SendFrame(void* buffer, UINT32 offset, UINT32 length);

   private:
      IPCObject m_ipc;
      std::vector<std::thread> m_frameTransmitterThreads;
      std::mutex m_writeMutex;
      void ReceiveMessageFramesThreadStart(FrameReceivedHandler onFrameReceived);
   };
} } }