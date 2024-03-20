﻿using System.Runtime.Serialization;

namespace DiplomaProject
{
    [DataContract]
    public class ImageMessage
    {
        [DataMember]
        public byte[] ImageData { get; set; }

        [DataMember]
        public string ImageName { get; set; }
    }
}