#region usings
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Text;
using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using VVVV.Core.Logging;
using MediaInfoNET;
using System.Runtime.InteropServices;

#endregion usings

namespace myVVVVdes
{
    #region PluginInfo
    [PluginInfo(Name = "GetMediaDuration", Category = "File", Help = "Gets the duration of media files", Author = "aivenhoe", Tags = "video, duration, MediaInfo")]
    #endregion PluginInfo

    public class GetMediaDurationNode : IPluginEvaluate
    {
        #region fields & pins

        [Input("File Path In", DefaultString = "MyVideoFile")]
        public IDiffSpread<string> FFilePath;

        [Input("Reset", IsBang = true, IsSingle = true)]
        public ISpread<bool> FReset;


        [Output("Duration")]
        public ISpread<double> FDuration;
        [Output("File Path Out", Visibility = PinVisibility.Hidden)]
        public ISpread<string> FNames;


        #endregion fields & pins

        //called when data for any output pin is requested

        Dictionary<string, double> IdValue = new Dictionary<string, double>();

        public void Evaluate(int SpreadMax)
        {
            if (FReset[0]) IdValue.Clear();
            if (FFilePath.IsChanged || FReset[0])
            {
                for (int i = 0; i < FFilePath.SliceCount; i++)
                {
                    //updating dictionnary
                    if (!IdValue.ContainsKey(FFilePath[i]))
                    {
                        MediaFile uploadedFile = new MediaFile(FFilePath[i]);
                        IdValue.Add(FFilePath[i], TimeSpan.Parse(uploadedFile.General.DurationStringAccurate.ToString()).TotalSeconds);
                    }

                }
                //outputing dictionary
                FNames.SliceCount = FFilePath.SliceCount;
                FDuration.SliceCount = FFilePath.SliceCount;

                for (int i = 0; i < FFilePath.SliceCount; i++)
                {
                    if (IdValue.ContainsKey(FFilePath[i]))
                    {
                        FNames[i] = FFilePath[i];
                        FDuration[i] = IdValue[FFilePath[i]];
                    }
                    else
                        IdValue.Remove(FFilePath[i]);
                }


            }


        }
    }
}