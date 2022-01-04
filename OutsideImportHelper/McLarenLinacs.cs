using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutsideImportHelper
{
    public static class McLarenLinacs
    {
        public static List<Linac> LinacList = new List<Linac>()
        {
            new Linac(),
            Linac.new2300IX("21IX-SN3743", "3743"),
            Linac.new21EX("21IX-SN3856", "3856"),
            Linac.newTrueBeam("BAY_TB3384", "3384"),
            Linac.newTrueBeam("CLK_TB5190", "5190"),
            Linac.newTrueBeam("GROC_TB1601", "1601"),
            Linac.new2300IX("IX_GROC", "4546"),
            Linac.new2300IX("IX_Farmington", "4694"),
            Linac.newTrueBeam("MAC_TB3568", "3568"),
            Linac.newTrueBeam("MGL_TB5333", "5333"),
            Linac.newTrueBeam("NOR_TB4780", "4780"),
            Linac.newTrueBeam("TB2681", "2681"),
            Linac.new2300IX("TRILOGY", "959"),
            Linac.new2300IX("TRILOGY3789", "3789"),
            Linac.newTrueBeam("TrueBeam1030", "1030"),
            Linac.newTrueBeam("TrueBeamSN2873", "2873")
        };
    }
}
