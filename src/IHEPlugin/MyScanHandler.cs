using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Panasonic.KV_SS1100.API.Scanner;
using Panasonic.KV_SS1100.API.Scanner.Config;
using InterSystems.IHE.Client;
using Panasonic.KV_SS1100.API.UI;

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
            args.SessionScanConfig.ImageFormat = ImageFormat.Pdf;

           
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

            isFirstFile = false;

            //string outputFile = Path.GetTempFileName() + args.FileExtension;
            this.firstPageData = args.FileDataBytes;
            /*
            using (Stream input = args.OpenStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                //request.data = memoryStream.ToArray();
                this.pageArray.Add(memoryStream.ToArray());
            }
             */

            // Open the first document that was scanned (or the result PDF file)
            // ?? Maybe we let them SEE the document before sending off??
            //Process.Start(outputFile);
        }

        //private List<byte[]> pageArray;

        private byte[] firstPageData;

        public void OnScanFinish(IScanner scanner, ScanFinishEventArgs args)
        {
            // Let's store the document in memory - create the XDSb request
            // have user do one confirmation before sending....
            var request = new XDSbRequest();
            var client = new XDSbClient();
            client.ExchangeEndpoint = IHEPlugin.IHEClientUtils.XDSbEndpoint();
            IHEPlugin.IHEClientUtils.ConfigureRequest(request);
            client.Verbose = true;
            // TODO - much more UI to set these options!
            request.options = XDSbRequest.getDefaultOptions();
            var pdq = (PDQResponse)MyPage.PatientContext;
            //var pdq_id = pdq.Identifiers.First();       // TODO - need to have ui to pick this!
            var pdq_id = pdq.Identifiers.Last();
            request.patientId = pdq_id.ID;
            request.idSource = pdq_id.Source;
            request.mimeType = MimeType.JPG;    // TODO!!
            request.data = new byte[this.firstPageData.Length];
            Buffer.BlockCopy(this.firstPageData, 0, request.data, 0, this.firstPageData.Length);
            //   using (Stream input = args.OpenStream())
           /*
            int last = 0;
            int fullSize = 0;
            foreach (var pageArray in this.pageArray)
            {
                fullSize += pageArray.Length;
            }
            request.data = new byte[fullSize];
            foreach (var page in this.pageArray)
            {
                   // memoryStream.open
                    //memoryStream.Position = 0;
                  Buffer.BlockCopy(page, 0, request.data, last, (int)page.Length);
                    last = page.Length; // ?? maybe off one here??
            }
            * */
            //UIManager.ShowMessageDialog(request.ToString(), System.Windows.MessageBoxImage.Asterisk, null);

            var result = client.ProvideAndRegisterDocument(request);
            UIManager.ShowMessageDialog(result.ToString(), System.Windows.MessageBoxImage.Exclamation, null);
       
        }

        public void OnScanError(IScanner scanner, ScanErrorEventArgs args)
        {
            // Normally you don't have to do anything here. 
            // This is simply to notify you that scanning was not completed successfully.
        }

        #endregion
    }
}
