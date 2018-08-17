using System;
using System.Windows.Forms;
using RefProp_wrapper;

namespace iRefProp_Test
{
    public partial class Form1 : Form
    {
        static string FName; // only necessary for the new RefProp API to work correctly

        iRefProp irp;

        public Form1()
        {
            InitializeComponent();
            irp = new iRefProp();  // iRefProp instance
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (iRefProp.DLLinit() == iRefProp.NoDLL)
            {
                if (MessageBox.Show(this, "Unable to locate NIST RefProp DLL file.\n\nNIST RefProp software may be " +
                    "purchased from the NIST Standard Reference Data website.  Would you like to visit this website?",
                    "RefProp DLL Not Found", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                    System.Diagnostics.Process.Start("https://www.nist.gov/srd/refprop");
                Application.Exit();
            }
        }

        public const double // exact conversions
            FT_M = 0.3048, // converts feet to meters
            IN2_M2 = 0.00064516,  // converts in2 to m2
            LBF_KN = 0.0044482216152605, // converts pounds force to kilonewtons
            LBF_MN = 0.0000044482216152605, // converts pounds force to meganewtons
            ATM_MPA = 0.101325, // converts atmosphere to MPa
            PSIA_KPA = LBF_KN / IN2_M2, // converts psia to kPa
            PSIA_MPA = LBF_MN / IN2_M2, // converts psia to MPa
            ATM_PSIA = ATM_MPA / PSIA_MPA, // converts atmosphere to psia
            FT3_L = 28.316846592, // converts ft3 to liters
            LB_GM = 453.59237, // converts pounds to grams
            FT3LB_LGM = FT3_L / LB_GM, // converts ft3 per pound to liters per gram
            BTUitLB_KJKG = 2.326, // converts BtuIT per pround to kilojoules per kilogram
            BTUitLBR_KJKGK = 4.1868, // converts BtuIT per pound-°F to kilojoules per kilogram-K
            LBFTS_UPAS = 0.45359237 / 0.0000003048, // converts lbm/(ft-s) to uPa-s
            BTUITFTHFT2F_WMK = 0.52752792631 / 0.3048; // converts BtuIT-ft/h-ft2-°F to W/m-K

        public static double ftok(double t)
        {
            return ((t + 459.67) / 1.8);
        }

        public static double ktof(double t)
        {
            return (1.8 * t - 459.67);
        }

        public static double psig_kpa(double p)
        {
            return ((p + ATM_PSIA) * PSIA_KPA);
        }

        public static double kpa_psig(double p)
        {
            return ((p / PSIA_KPA) - ATM_PSIA);
        }

        public static double psia_psig(double p)
        {
            return (p - ATM_PSIA);
        }

        public static double psig_psia(double p)
        {
            return (p + ATM_PSIA);
        }

        public static double moll_lbft3(double r, double mw)
        {
            return (r * mw * FT3LB_LGM);
        }

        public static double jmol_blb(double h, double mw)
        {
            return (h / (mw * BTUitLB_KJKG));
        }

        public static double jmolk_blbr(double s, double mw)
        {
            return (s / (mw * BTUitLBR_KJKGK));
        }

        public static double upas_lbfts(double eta)
        {
            return (eta / LBFTS_UPAS);
        }

        public static double wmk_btuitfthft2f(double tcx)
        {
            return (tcx / BTUITFTHFT2F_WMK);
        }

        public static double m_ft(double l)
        {
            return (l / FT_M);
        }

        private void satrefprops(double ldensity, double vdensity, double hf, double hg, double sf, double sg,
            double cvf, double cvg, double cpf, double cpg, double wf, double wg, double etaf, double etag,
            double tcxf, double tcxg)
        {
            listBox1.Items.Add("Liquid Density (lbm/ft3): " + String.Format("{0:0.000}", ldensity));
            listBox1.Items.Add("Vapor Density (lbm/ft3): " + String.Format("{0:0.000}", vdensity));
            listBox1.Items.Add("Liquid Enthalpy (Btu/lbm): " + String.Format("{0:0.00}", hf));
            listBox1.Items.Add("Vapor Enthalpy (Btu/lbm): " + String.Format("{0:0.00}", hg));
            listBox1.Items.Add("Liquid Entropy (Btu/lbm-°F): " + String.Format("{0:0.0000}", sf));
            listBox1.Items.Add("Vapor Entropy (Btu/lbm-°F): " + String.Format("{0:0.0000}", sg));
            listBox1.Items.Add("Liquid Isochoric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", cvf));
            listBox1.Items.Add("Vapor Isochoric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", cvg));
            listBox1.Items.Add("Liquid Isobaric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", cpf));
            listBox1.Items.Add("Vapor Isobaric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", cpg));
            listBox1.Items.Add("Liquid Speed of Sound (ft/s): " + String.Format("{0:0.00}", wf));
            listBox1.Items.Add("Vapor Speed of Sound (ft/s): " + String.Format("{0:0.00}", wg));
            listBox1.Items.Add("Liquid Dynamic Viscosity (lbm/ft-s): " + String.Format("{0:e4}", etaf));
            listBox1.Items.Add("Vapor Dynamic Viscosity (lbm/ft-s): " + String.Format("{0:e4}", etag));
            listBox1.Items.Add("Liquid Thermal Conductivity (Btu-ft/h-ft2-°F): " + String.Format("{0:e4}", tcxf));
            listBox1.Items.Add("Vapor Thermal Conductivity (Btu-ft/h-ft2-°F): " + String.Format("{0:e4}", tcxg));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double ppsig, tf, p = 0.0, pb = 0.0, t = 0.0, tb = 0.0, ldensity = 0.0,
                vdensity = 0.0, h = 0.0, hf = 0.0, hg = 0.0, q = 0.0, s = 0.0, sf = 0.0, sg = 0.0, cv = 0.0,
                cvf = 0.0, cvg = 0.0, cp = 0.0, cpf = 0.0, cpg = 0.0, w = 0.0, wf = 0.0, wg = 0.0,
                eta = 0.0, etaf = 0.0, etag = 0.0, tcx = 0.0, tcxf = 0.0, tcxg = 0.0;
            string dlltesterror = "iRefProp Test Program Error";
            if (irp.NISTSetup(textBox1.Text))
            {
                if (String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox3.Text))
                    MessageBox.Show(this, "Must enter either a temperature or pressure, or both.",
                        dlltesterror, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                {
                    listBox1.Items.Clear();
                    if (!String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox3.Text))
                    {
                        Double.TryParse(textBox2.Text, out tf);
                        t = ftok(tf);
                        irp.sattdpt(t, iRefProp.BUBBLEPOINT, ref pb, ref ldensity, ref vdensity, ref hf, ref sf, ref cvf, ref cpf,
                            ref wf, ref etaf, ref tcxf);
                        irp.sattdpt(t, iRefProp.DEWPOINT, ref p, ref ldensity, ref vdensity, ref hg, ref sg, ref cvg, ref cpg,
                            ref wg, ref etag, ref tcxg);
                        listBox1.Items.Add("Bubble Point Pressure (psig): " + String.Format("{0:0.00}", kpa_psig(pb)));
                        listBox1.Items.Add("Dew Point Pressure (psig): " + String.Format("{0:0.00}", kpa_psig(p)));
                        satrefprops(moll_lbft3(ldensity, irp.MW), moll_lbft3(vdensity, irp.MW), jmol_blb(hf, irp.MW),
                            jmol_blb(hg, irp.MW), jmolk_blbr(sf, irp.MW), jmolk_blbr(sg, irp.MW), jmolk_blbr(cvf, irp.MW),
                            jmolk_blbr(cvg, irp.MW), jmolk_blbr(cpf, irp.MW), jmolk_blbr(cpg, irp.MW), m_ft(wf),
                            m_ft(wg), upas_lbfts(etaf), upas_lbfts(etag), wmk_btuitfthft2f(tcxf), wmk_btuitfthft2f(tcxg));
                    }
                    else if (String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text))
                    {
                        Double.TryParse(textBox3.Text, out ppsig);
                        p = psig_kpa(ppsig);
                        irp.sattdpp(p, iRefProp.BUBBLEPOINT, ref tb, ref ldensity, ref vdensity, ref hf, ref sf, ref cvf, ref cpf,
                            ref wf, ref etaf, ref tcxf);
                        irp.sattdpp(p, iRefProp.DEWPOINT, ref t, ref ldensity, ref vdensity, ref hg, ref sg, ref cvg, ref cpg,
                            ref wg, ref etag, ref tcxg);
                        listBox1.Items.Add("Bubble Point Temperature (°F): " + String.Format("{0:0.00}", ktof(tb)));
                        listBox1.Items.Add("Dew Point Temperature (°F): " + String.Format("{0:0.00}", ktof(t)));
                        satrefprops(moll_lbft3(ldensity, irp.MW), moll_lbft3(vdensity, irp.MW), jmol_blb(hf, irp.MW),
                            jmol_blb(hg, irp.MW), jmolk_blbr(sf, irp.MW), jmolk_blbr(sg, irp.MW), jmolk_blbr(cvf, irp.MW),
                            jmolk_blbr(cvg, irp.MW), jmolk_blbr(cpf, irp.MW), jmolk_blbr(cpg, irp.MW), m_ft(wf),
                            m_ft(wg), upas_lbfts(etaf), upas_lbfts(etag), wmk_btuitfthft2f(tcxf), wmk_btuitfthft2f(tcxg));
                    }
                    else // if (!String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text))
                    {
                        Double.TryParse(textBox2.Text, out tf);
                        t = ftok(tf);
                        Double.TryParse(textBox3.Text, out ppsig);
                        p = psig_kpa(ppsig);
                        irp.flshtdp(t, p, ref ldensity, ref vdensity, ref h, ref q, ref s, ref cv, ref cp, ref w, ref eta, ref tcx);
                        if (q < 0.0)
                        {
                            listBox1.Items.Add("Subcooled Liquid");
                            listBox1.Items.Add("Liquid Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(ldensity, irp.MW)));
                        }
                        else if (q > 1.0)
                        {
                            listBox1.Items.Add("Superheated Vapor");
                            listBox1.Items.Add("Vapor Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(vdensity, irp.MW)));
                        }
                        else // saturated refrigerant
                        {
                            listBox1.Items.Add("Liquid Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(ldensity, irp.MW)));
                            listBox1.Items.Add("Vapor Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(vdensity, irp.MW)));
                            listBox1.Items.Add("Saturated, Refrigerant Quality: " + String.Format("{0:0.0000}", q));
                        }
                        listBox1.Items.Add("Enthalpy (Btu/lbm): " + String.Format("{0:0.00}", jmol_blb(h, irp.MW)));
                        listBox1.Items.Add("Entropy (Btu/lbm-°F): " + String.Format("{0:0.0000}", jmolk_blbr(s, irp.MW)));
                        if (q < 0.0 || q > 1.0)
                        {
                            listBox1.Items.Add("Isochoric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", jmolk_blbr(cv, irp.MW)));
                            listBox1.Items.Add("Isobaric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", jmolk_blbr(cp, irp.MW)));
                            listBox1.Items.Add("Speed of Sound (ft/s): " + String.Format("{0:0.00}", m_ft(w)));
                            listBox1.Items.Add("Dynamic Viscosity (lbm/ft-s): " + String.Format("{0:e4}", upas_lbfts(eta)));
                            listBox1.Items.Add("Thermal Conductivity (Btu-ft/h-ft2-°F): " + String.Format("{0:e4}", wmk_btuitfthft2f(tcx)));
                        }
                    }
                    listBox1.Items.Add("Critical Temperature (°F): " + String.Format("{0:0.00}", ktof(irp.TC)));
                    listBox1.Items.Add("Critical Pressure (psig): " + String.Format("{0:0.00}", kpa_psig(irp.PC)));
                    listBox1.Items.Add("Critical Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(irp.DC, irp.MW)));
                    listBox1.Items.Add("Molecular Weight: " + String.Format("{0:0.00}", irp.MW));
                    listBox1.Items.Add("RefProp Version: " + String.Format("{0:0.0000}", iRefProp.dllversetup()));
                }
            }
            else
                MessageBox.Show(this, "Unidentified refrigerant file", dlltesterror, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double ppsig, tf, p = 0.0, pb = 0.0, t = 0.0, tb = 0.0, density = 0.0, ldensity = 0.0,
                vdensity = 0.0, h = 0.0, hf = 0.0, hg = 0.0, q = 0.0, s = 0.0, sf = 0.0, sg = 0.0, cv = 0.0,
                cvf = 0.0, cvg = 0.0, cp = 0.0, cpf = 0.0, cpg = 0.0, w = 0.0, wf = 0.0, wg = 0.0,
                eta = 0.0, etaf = 0.0, etag = 0.0, tcx = 0.0, tcxf = 0.0, tcxg = 0.0;
            string FName1, dlltesterror = "iRefProp Test Program Error";
            if (String.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show(this, "Must enter a NIST refrigerant file.",
                    dlltesterror, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox3.Text))
                MessageBox.Show(this, "Must enter either a temperature or pressure, or both.",
                    dlltesterror, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                listBox1.Items.Clear();
                // the following check on static string FName is necessary for the new RefProp API to work correctly
                if (String.Equals(textBox1.Text, FName))
                    FName1 = String.Empty;
                else
                    FName1 = FName = textBox1.Text;
                if (!String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox3.Text))
                {
                    Double.TryParse(textBox2.Text, out tf);
                    t = tf;
                    irp.refpropt(FName1, t, iRefProp.BUBBLEPOINT, ref pb, ref ldensity, ref hf, ref sf, ref cvf, ref cpf,
                        ref wf, ref etaf, ref tcxf);
                    irp.refpropt(String.Empty, t, iRefProp.DEWPOINT, ref p, ref vdensity, ref hg, ref sg, ref cvg, ref cpg,
                        ref wg, ref etag, ref tcxg);
                    listBox1.Items.Add("Bubble Point Pressure (psig): " + String.Format("{0:0.00}", psia_psig(pb)));
                    listBox1.Items.Add("Dew Point Pressure (psig): " + String.Format("{0:0.00}", psia_psig(p)));
                    satrefprops(ldensity, vdensity, hf, hg, sf, sg, cvf, cvg, cpf, cpg, wf, wg, etaf, etag, tcxf, tcxg);
                }
                else if (String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text))
                {
                    Double.TryParse(textBox3.Text, out ppsig);
                    p = psig_psia(ppsig);
                    irp.refpropp(FName1, p, iRefProp.BUBBLEPOINT, ref tb, ref ldensity, ref hf, ref sf, ref cvf, ref cpf,
                        ref wf, ref etaf, ref tcxf);
                    irp.refpropp(String.Empty, p, iRefProp.DEWPOINT, ref t, ref vdensity, ref hg, ref sg, ref cvg, ref cpg,
                        ref wg, ref etag, ref tcxg);
                    listBox1.Items.Add("Bubble Point Temperature (°F): " + String.Format("{0:0.00}", tb));
                    listBox1.Items.Add("Dew Point Temperature (°F): " + String.Format("{0:0.00}", t));
                    satrefprops(ldensity, vdensity, hf, hg, sf, sg, cvf, cvg, cpf, cpg, wf, wg, etaf, etag, tcxf, tcxg);
                }
                else // if (!String.IsNullOrEmpty(textBox2.Text) && !String.IsNullOrEmpty(textBox3.Text))
                {
                    Double.TryParse(textBox2.Text, out tf);
                    t = tf;
                    Double.TryParse(textBox3.Text, out ppsig);
                    p = psig_psia(ppsig);
                    irp.refproppt(FName1, t, p, ref density, ref h, ref q, ref s, ref cv, ref cp, ref w,
                        ref eta, ref tcx);
                    if (q < 0.0)
                    {
                        listBox1.Items.Add("Subcooled Liquid");
                        listBox1.Items.Add("Liquid Density (lbm/ft3): " + String.Format("{0:0.000}", density));
                    }
                    else if (q > 1.0)
                    {
                        listBox1.Items.Add("Superheated Vapor");
                        listBox1.Items.Add("Vapor Density (lbm/ft3): " + String.Format("{0:0.000}", density));
                    }
                    else // two-phase refrigerant - requires call to a flash routine to determine liquid and vapor densities
                    {
                    //    listBox1.Items.Add("Liquid Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(ldensity, irp.MW)));
                    //    listBox1.Items.Add("Vapor Density (lbm/ft3): " + String.Format("{0:0.000}", moll_lbft3(vdensity, irp.MW)));
                        listBox1.Items.Add("Saturated, Refrigerant Quality: " + String.Format("{0:0.0000}", q));
                    }
                    listBox1.Items.Add("Enthalpy (Btu/lbm): " + String.Format("{0:0.00}", h));
                    listBox1.Items.Add("Entropy (Btu/lbm-°F): " + String.Format("{0:0.0000}", s));
                    if (q < 0.0 || q > 1.0)
                    {
                        listBox1.Items.Add("Isochoric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", cv));
                        listBox1.Items.Add("Isobaric Specific Heat (Btu/lbm-°F): " + String.Format("{0:0.0000}", cp));
                        listBox1.Items.Add("Speed of Sound (ft/s): " + String.Format("{0:0.00}", w));
                        listBox1.Items.Add("Dynamic Viscosity (lbm/ft-s): " + String.Format("{0:e4}", eta));
                        listBox1.Items.Add("Thermal Conductivity (Btu-ft/h-ft2-°F): " + String.Format("{0:e4}", tcx));
                    }
                }
                listBox1.Items.Add("Critical Temperature (°F): " + String.Format("{0:0.00}", irp.TC));
                listBox1.Items.Add("Critical Pressure (psig): " + String.Format("{0:0.00}", psia_psig(irp.PC)));
                listBox1.Items.Add("Critical Density (lbm/ft3): " + String.Format("{0:0.000}", irp.DC));
                listBox1.Items.Add("Molecular Weight: " + String.Format("{0:0.00}", irp.MW));
                listBox1.Items.Add("RefProp Version: " + String.Format("{0:0.0000}", iRefProp.dllverrefprop()));
            }
        }
    }
}
