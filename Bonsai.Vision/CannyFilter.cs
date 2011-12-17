﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCV.Net;
using System.ComponentModel;
using System.Drawing.Design;
using Bonsai.Design;

namespace Bonsai.Vision
{
    public class CannyFilter : Filter<IplImage, IplImage>
    {
        IplImage output;

        [Range(0, 255)]
        [Editor(typeof(TrackBarEditor), typeof(UITypeEditor))]
        public double Threshold1 { get; set; }

        [Range(0, 255)]
        [Editor(typeof(TrackBarEditor), typeof(UITypeEditor))]
        public double Threshold2 { get; set; }

        [Range(0, 255)]
        [Editor(typeof(NumericUpDownEditor), typeof(UITypeEditor))]
        public int ApertureSize { get; set; }

        public override IplImage Process(IplImage input)
        {
            ImgProc.cvCanny(input, output, Threshold1, Threshold2, ApertureSize);
            return output;
        }

        public override void Load(WorkflowContext context)
        {
            var size = (CvSize)context.GetService(typeof(CvSize));
            output = new IplImage(size, 8, 1);
        }

        public override void Unload(WorkflowContext context)
        {
            output.Close();
        }
    }
}
