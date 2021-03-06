﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BO
{
    public enum UserPermFlag
    {
        MasterAdmin=1,
        MasterReader=2,
        ClientAdmin=4,
        ClientReader=8,
        MasterRecepturyProdukty=16,
        MasterZarizeni=32,
        MasterLicence=64,
        MasterLide=128

    }
    public class UserPermission
    {
        public string Name { get; set; }
        public UserPermFlag Value { get; set; }
        public bool IsSelected { get; set; }
    }
}
