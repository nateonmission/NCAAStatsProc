﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ncaa_grad_info
{
    class Conference
    {
        public string ChosenConf { get; set; }

        // TOTALS
        public int FED_N_2011_SA { get; set; }
        public int FED_N_2010_SA { get; set; }
        public int FED_N_2009_SA { get; set; }
        public int FED_N_2008_SA { get; set; }
        public int FED_N_2007_SA { get; set; }
        public int FED_N_2006_SA { get; set; }
        public int FED_N_2005_SA { get; set; }
        public int FED_N_2004_SA { get; set; }
        public int FED_N_2003_SA { get; set; }
        public int FED_N_2002_SA { get; set; }
        public int FED_N_2001_SA { get; set; }
        public int FED_N_2000_SA { get; set; }
        public int FED_N_1999_SA { get; set; }
        public int FED_N_1998_SA { get; set; }
        public int FED_N_1997_SA { get; set; }
        public int FED_N_1996_SA { get; set; }
        public int FED_N_1995_SA { get; set; }
        public int FED_RATE_2011_SA { get; set; }
        public int FED_RATE_2010_SA { get; set; }
        public int FED_RATE_2009_SA { get; set; }
        public int FED_RATE_2008_SA { get; set; }
        public int FED_RATE_2007_SA { get; set; }
        public int FED_RATE_2006_SA { get; set; }
        public int FED_RATE_2005_SA { get; set; }
        public int FED_RATE_2004_SA { get; set; }
        public int FED_RATE_2003_SA { get; set; }
        public int FED_RATE_2002_SA { get; set; }
        public int FED_RATE_2001_SA { get; set; }
        public int FED_RATE_2000_SA { get; set; }
        public int FED_RATE_1999_SA { get; set; }
        public int FED_RATE_1998_SA { get; set; }
        public int FED_RATE_1997_SA { get; set; }
        public int FED_RATE_1996_SA { get; set; }
        public int FED_RATE_1995_SA { get; set; }
        public int GSR_N_2011_SA { get; set; }
        public int GSR_N_2010_SA { get; set; }
        public int GSR_N_2009_SA { get; set; }
        public int GSR_N_2008_SA { get; set; }
        public int GSR_N_2007_SA { get; set; }
        public int GSR_N_2006_SA { get; set; }
        public int GSR_N_2005_SA { get; set; }
        public int GSR_N_2004_SA { get; set; }
        public int GSR_N_2003_SA { get; set; }
        public int GSR_N_2002_SA { get; set; }
        public int GSR_N_2001_SA { get; set; }
        public int GSR_N_2000_SA { get; set; }
        public int GSR_N_1999_SA { get; set; }
        public int GSR_N_1998_SA { get; set; }
        public int GSR_N_1997_SA { get; set; }
        public int GSR_N_1996_SA { get; set; }
        public int GSR_N_1995_SA { get; set; }
        public int GSR_2011_SA { get; set; }
        public int GSR_2010_SA { get; set; }
        public int GSR_2009_SA { get; set; }
        public int GSR_2008_SA { get; set; }
        public int GSR_2007_SA { get; set; }
        public int GSR_2006_SA { get; set; }
        public int GSR_2005_SA { get; set; }
        public int GSR_2004_SA { get; set; }
        public int GSR_2003_SA { get; set; }
        public int GSR_2002_SA { get; set; }
        public int GSR_2001_SA { get; set; }
        public int GSR_2000_SA { get; set; }
        public int GSR_1999_SA { get; set; }
        public int GSR_1998_SA { get; set; }
        public int GSR_1997_SA { get; set; }
        public int GSR_1996_SA { get; set; }
        public int GSR_1995_SA { get; set; }

        // AVERAGES
        public int AVG_FED_N_2011_SA { get; set; }
        public int AVG_FED_N_2010_SA { get; set; }
        public int AVG_FED_N_2009_SA { get; set; }
        public int AVG_FED_N_2008_SA { get; set; }
        public int AVG_FED_N_2007_SA { get; set; }
        public int AVG_FED_N_2006_SA { get; set; }
        public int AVG_FED_N_2005_SA { get; set; }
        public int AVG_FED_N_2004_SA { get; set; }
        public int AVG_FED_N_2003_SA { get; set; }
        public int AVG_FED_N_2002_SA { get; set; }
        public int AVG_FED_N_2001_SA { get; set; }
        public int AVG_FED_N_2000_SA { get; set; }
        public int AVG_FED_N_1999_SA { get; set; }
        public int AVG_FED_N_1998_SA { get; set; }
        public int AVG_FED_N_1997_SA { get; set; }
        public int AVG_FED_N_1996_SA { get; set; }
        public int AVG_FED_N_1995_SA { get; set; }
        public int AVG_FED_RATE_2011_SA { get; set; }
        public int AVG_FED_RATE_2010_SA { get; set; }
        public int AVG_FED_RATE_2009_SA { get; set; }
        public int AVG_FED_RATE_2008_SA { get; set; }
        public int AVG_FED_RATE_2007_SA { get; set; }
        public int AVG_FED_RATE_2006_SA { get; set; }
        public int AVG_FED_RATE_2005_SA { get; set; }
        public int AVG_FED_RATE_2004_SA { get; set; }
        public int AVG_FED_RATE_2003_SA { get; set; }
        public int AVG_FED_RATE_2002_SA { get; set; }
        public int AVG_FED_RATE_2001_SA { get; set; }
        public int AVG_FED_RATE_2000_SA { get; set; }
        public int AVG_FED_RATE_1999_SA { get; set; }
        public int AVG_FED_RATE_1998_SA { get; set; }
        public int AVG_FED_RATE_1997_SA { get; set; }
        public int AVG_FED_RATE_1996_SA { get; set; }
        public int AVG_FED_RATE_1995_SA { get; set; }
        public int AVG_GSR_N_2011_SA { get; set; }
        public int AVG_GSR_N_2010_SA { get; set; }
        public int AVG_GSR_N_2009_SA { get; set; }
        public int AVG_GSR_N_2008_SA { get; set; }
        public int AVG_GSR_N_2007_SA { get; set; }
        public int AVG_GSR_N_2006_SA { get; set; }
        public int AVG_GSR_N_2005_SA { get; set; }
        public int AVG_GSR_N_2004_SA { get; set; }
        public int AVG_GSR_N_2003_SA { get; set; }
        public int AVG_GSR_N_2002_SA { get; set; }
        public int AVG_GSR_N_2001_SA { get; set; }
        public int AVG_GSR_N_2000_SA { get; set; }
        public int AVG_GSR_N_1999_SA { get; set; }
        public int AVG_GSR_N_1998_SA { get; set; }
        public int AVG_GSR_N_1997_SA { get; set; }
        public int AVG_GSR_N_1996_SA { get; set; }
        public int AVG_GSR_N_1995_SA { get; set; }
        public int AVG_GSR_2011_SA { get; set; }
        public int AVG_GSR_2010_SA { get; set; }
        public int AVG_GSR_2009_SA { get; set; }
        public int AVG_GSR_2008_SA { get; set; }
        public int AVG_GSR_2007_SA { get; set; }
        public int AVG_GSR_2006_SA { get; set; }
        public int AVG_GSR_2005_SA { get; set; }
        public int AVG_GSR_2004_SA { get; set; }
        public int AVG_GSR_2003_SA { get; set; }
        public int AVG_GSR_2002_SA { get; set; }
        public int AVG_GSR_2001_SA { get; set; }
        public int AVG_GSR_2000_SA { get; set; }
        public int AVG_GSR_1999_SA { get; set; }
        public int AVG_GSR_1998_SA { get; set; }
        public int AVG_GSR_1997_SA { get; set; }
        public int AVG_GSR_1996_SA { get; set; }
        public int AVG_GSR_1995_SA { get; set; }

    }
}
