using System;
using Grok.Numenta;

namespace Grok.Numenta.UnitTests
{
    public class SampleUploadCallback : UploadCallback
    {
        public string PercentComplete = string.Empty;

        public override void OnUpdate(UploadEvent e) 
        {
            PercentComplete = e.ToString();
        }
    }
}
