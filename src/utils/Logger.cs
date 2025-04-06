using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace PPirate.VoxReactor
{
   
    internal class Logger
    {
        public static bool logsEnabled = true;

        public static bool debugLogsEnabled = false;
        public bool debugLogsEnabledOverride;
        public static bool traceLogsEnabled = false;
        public bool traceLogsEnabledOverride;


        private readonly string className;

        public Logger(String className)
        {
            this.className = className;
            this.debugLogsEnabledOverride = debugLogsEnabledOverride;
            this.traceLogsEnabledOverride = traceLogsEnabledOverride;
        }
        public Logger(String className, int signatureDifferentator)
        {
            this.className = className;
            this.debugLogsEnabledOverride = true;
            this.traceLogsEnabledOverride = true;
        }

        public void LOG(String msg)
        {
            if (logsEnabled)
            {
                SuperController.LogMessage(className + " " + msg);
            }
        }

        public void DEBUG(String msg)
        {
            if (debugLogsEnabled || debugLogsEnabledOverride)
            {
                SuperController.LogMessage(className + " " + msg);
            }
        }
        public void StartMethod(string methodName) {
            if (traceLogsEnabled || traceLogsEnabledOverride)
            {
                SuperController.LogMessage("START " +className +"."+ methodName);
            }
        }
        public void EndMethod()
        {
            if (traceLogsEnabled || traceLogsEnabledOverride)
            {
                SuperController.LogMessage(className + "TODO" + " END");
            }
        }
        public void Constructor()
        {
            if (traceLogsEnabled || traceLogsEnabledOverride)
            {
                SuperController.LogMessage(className + "Constructor");
            }
        }

        public void ERR(String msg)
        {
           
            SuperController.LogError(className + msg);
     
        }

        public void SPECIAL(String msg)
        {
            SuperController.LogError("SPECIAL " + className + msg);
        }

     
    }
}
