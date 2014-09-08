using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Panasonic.KV_SS1100.API.Scanner;
using Panasonic.KV_SS1100.API.Scanner.Config;

namespace PluginExample
{
    class MyScanHandler : IScanHandler
    {
        private bool isFirstFile = true;
        private bool dropOutRed, smoothBg, dynThresh;

        public MyScanHandler(bool dropOutRed, bool smoothBg, bool dynThresh)
        {
            this.dropOutRed = dropOutRed;
            this.smoothBg = smoothBg;
            this.dynThresh = dynThresh;
        }

        #region IScanHandler Members

        public void OnScanBegin(IScanner scanner, ScanBeginEventArgs args)
        {
            args.SessionScanConfig.DropOut = dropOutRed ? DropOut.Red : DropOut.None;
            args.SessionScanConfig.SmoothBackground = smoothBg;
            args.SessionScanConfig.DynamicThreshold = dynThresh;
        }

        public void OnPageScan(IScanner scanner, PageScanEventArgs args)
        {
            if (!isFirstFile)
            {
                // Only scan the first page (don't care about anything else ..)
                args.CancelScan = true;

                return;
            }

            if (args.FileExtension == null)
            {
                // This is not a file (maybe just page data)
                // Wait for OnPageScan event with actual data.
                return; 
            }

            isFirstFile = true;

            string outputFile = Path.GetTempFileName() + args.FileExtension;

            // Open the first document that was scanned (or the result PDF file)
            using (Stream input = args.OpenStream())
            using (Stream output = File.OpenWrite(outputFile))
            {
                input.CopyTo(output);
            }

            Process.Start(outputFile);
        }

        public void OnScanFinish(IScanner scanner, ScanFinishEventArgs args)
        {
        }

        public void OnScanError(IScanner scanner, ScanErrorEventArgs args)
        {
            // Normally you don't have to do anything here. 
            // This is simply to notify you that scanning was not completed successfully.
        }

        #endregion
    }
}
