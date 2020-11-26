using Jc.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Jc.Core.TestApp.Test
{
    /// <summary>
    /// XmlHelper Test
    /// </summary>
    public class XmlHelperTest
    {
        public void Test()
        {
            string filePath = @"\\10.10.11.199\PoleStar\Sinogram.System\AlgorithmParameters\4ring.para";
            XmlHelper xmlHelper = new XmlHelper(filePath);

            AcParameter acParam = xmlHelper.DeserializeNode<AcParameter>("AlignmentParameters");
            DicomParameter dicomParam = xmlHelper.DeserializeNode<DicomParameter>("DICOMParameters");
        }


        public class AcParameter
        {
            [Description("AC水平方向偏差")]
            public float AlignmentXOffSet { get; set; }

            [Description("AC竖直方向偏差")]
            public float AlignmentYOffSet { get; set; }

            [Description("AC轴向偏差")]
            public float AlignmentZOffSet { get; set; }

            [Description("AC旋转四元数1")]
            public float Alignmentqx { get; set; }

            [Description("AC旋转四元数2")]
            public float Alignmentqy { get; set; }

            [Description("AC旋转四元数3")]
            public float Alignmentqz { get; set; }

            [Description("AC旋转四元数4")]
            public float Alignmentqw { get; set; }
        }

        public class DicomParameter
        {
            [Description("Dicom坐标水平方向偏差")]
            public float PetDicomOffsetX { get; set; }

            [Description("Dicom坐标竖直方向偏差")]
            public float PetDicomOffsetY { get; set; }

            [Description("轴向偏差床位补偿")]
            public float CaliberationDeviation { get; set; }
        }
    }
}
