using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Panasonic.KV_SS1100.API.Scanner;
using Panasonic.KV_SS1100.API.Scanner.Config;
//using InterSystems.IHE.Client;
using Panasonic.KV_SS1100.API.UI;
using HSPanasonic.ScanPoint;
using Panasonic.KV_SS1100.API.Config;

namespace PluginExample
{
    class MyScanHandler : IScanHandler
    {
        private bool isFirstFile = true;
        private bool dropOutRed, smoothBg, dynThresh;


        public  string DocumentName { get; set; }
        public string DocumentType { get; set; }
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
            else
            {
                this.fileType = args.FileExtension;
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
        private string fileType = "";
        private byte[] firstPageData;

        public void OnScanFinish(IScanner scanner, ScanFinishEventArgs args)
        {
            var sdr = new SubmitDocumentRequest();
            sdr.Subject = new Dictionary<string, string>();
            if (MyPage.Subject == null)
            {
                UIManager.ShowMessageDialog("No patient information was entered. Please search or enter patient information and try your scan again",
                    System.Windows.MessageBoxImage.Error,null);
                return;
            }
            foreach (var k in MyPage.Subject.Keys)
            {
                sdr.Subject.Add(k, MyPage.Subject[k].ToString());
            }
            sdr.Subject.Add("DocumentType", this.DocumentType);
            sdr.Name = this.DocumentName;   // MyPage.DocumentName;
            sdr.Type = this.fileType;
            sdr.User = ConfigManager.CurrentUser.ToString();
            sdr.Size = this.firstPageData.Length;
            sdr.Body = new byte[this.firstPageData.Length];
            Buffer.BlockCopy(this.firstPageData, 0, sdr.Body, 0, this.firstPageData.Length);
            try
            {
                var result = MyPage.scanPointClient.SubmitDocument(sdr);

                /*
                var msg = "";
                if (result.Successful)
                {
                    msg = "Scanned document was successfully uploaded to HealthShare";
                } else {
                    msg = string.Join(",", result.Response.RegistryErrorList.RegistryError.Select(re => re.Value));
                }
                 * */
                string s = string.Join(";", result.Select(x => x.Key + "=" + x.Value).ToArray());
                UIManager.ShowMessageDialog(s, System.Windows.MessageBoxImage.Information, null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void OnScanError(IScanner scanner, ScanErrorEventArgs args)
        {
            // Normally you don't have to do anything here. 
            // This is simply to notify you that scanning was not completed successfully.
        }

        #endregion
    }
}
