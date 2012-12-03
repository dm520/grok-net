using System;
using Grok.Numenta;

namespace HelloGrok
{
    public class SampleUploadCallback : UploadCallback
    {
        public override void OnUpdate(UploadEvent e) 
        {
            Console.WriteLine("Upload is " + e.ToString() + " complete.");
        }
    }
}
