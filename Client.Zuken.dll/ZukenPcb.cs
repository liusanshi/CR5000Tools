using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Kingdee.PLM.Integration.Client.Common.Abstract;
using Zuken.Common;

namespace Zuken.Client
{
    public class ZukenPcb : IIntegration
    {
        #region IIntegration 成员

        public string CadType
        {
            get
            {
#if test
                //return "ADPCB";
                return "ALLEGROPCB";
#else
                return "ZKPCB";
#endif

            }
        }

        public bool CheckApplicationIsRun()
        {
            return Tools.ApplicationIsRun("zfmscheme.exe");
        }

        public System.Collections.ArrayList GetBom()
        {
            return DocumentProperty.LoadXml(Tools.BOMFilePath).ConvertToBOM();
        }

        public System.Collections.ArrayList GetBomByFileName(string Filename)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentFileName()
        {
            return Path.GetFileName(GetCurrentFullPath());
        }

        public string GetCurrentFilePath()
        {
            return Path.GetDirectoryName(GetCurrentFullPath());
        }

        public string GetCurrentFileProperty()
        {
            return "";
        }

        public string GetCurrentFullPath()
        {
            return Tools.GetProjectPath();
        }

        public string GetFileDesCription(string FullPath)
        {
            throw new NotImplementedException();
        }

        public bool OpenFile(string FullPath)
        {
            Tools.ShellExecute(FullPath, "");
            return true;
        }

        public bool SetPropertyInfo(string FullPath, string PropString)
        {
            throw new NotImplementedException();
        }

        public bool WriteFileDesCription(string FullPath, string PropString)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
