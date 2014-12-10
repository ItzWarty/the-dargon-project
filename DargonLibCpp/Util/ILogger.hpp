#pragma once

#include <ostream>
#include <functional>
#include "../Dargon.hpp"
#include "LoggerLevels.hpp"

typedef std::function<void(std::ostream&)> LoggingFunction;

namespace dargon { namespace util {
   class ILogger
   {
   public:
      virtual void Log(UINT32 loggerLevel, LoggingFunction loggingFunction) = 0;
   };
} }