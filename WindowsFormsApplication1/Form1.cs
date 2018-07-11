using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Data.SqlClient;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        SqlConnection baglanti = new SqlConnection("Data Source=SHANKS\\SQLEXPRESS;Initial Catalog=MerkezBankasi;Integrated Security=True");
        SqlCommand sorgu = new SqlCommand();
        DataSet xmlVerileri;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Interval = 5 * 60 * 1000;
            timer1.Start();

            insertOrUpdate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            insertOrUpdate();
        }

        public void insertOrUpdate()
        {

            if (baglanti.State == ConnectionState.Closed)
            {
                baglanti.Open();
            }

            SqlDataAdapter adapter = new SqlDataAdapter("", baglanti);
            string xmlAdresi = "http://www.tcmb.gov.tr/kurlar/today.xml";
            xmlVerileri = xmlVerileriGetir(xmlAdresi);
            dgwDoviz.DataSource = xmlVerileri;
            sorgu.Connection = baglanti;

            adapter.SelectCommand.CommandText = "SELECT  Tarih , Kod, Isim , Alis , Satis ,id  FROM Doviz";
            adapter.Fill(xmlVerileri, "Doviz");

            dgwDoviz.DataSource = xmlVerileri.Tables["Doviz"];
            dgwDoviz.Columns[5].Visible = false;



            sorgu.CommandText = "TabloyaIslemUygula";
            sorgu.CommandType = CommandType.StoredProcedure;

            for (int i = 0; i < xmlVerileri.Tables[0].Rows.Count; i++)
            {
                sorgu.Parameters.Clear();
                sorgu.Parameters.AddWithValue("@Tarih", xmlVerileri.Tables["Doviz"].Rows[i]["Tarih"]);
                sorgu.Parameters.AddWithValue("@Kod", xmlVerileri.Tables["Doviz"].Rows[i]["Kod"]);
                sorgu.Parameters.AddWithValue("@Isim", xmlVerileri.Tables["Doviz"].Rows[i]["Isim"]);
                sorgu.Parameters.AddWithValue("@Alis", xmlVerileri.Tables["Doviz"].Rows[i]["Alis"]);
                sorgu.Parameters.AddWithValue("@Satis", xmlVerileri.Tables["Doviz"].Rows[i]["Satis"]);
                sorgu.Parameters.AddWithValue("@id", xmlVerileri.Tables["Doviz"].Rows[i]["id"]);
                sorgu.ExecuteNonQuery();
            }

        }

        public DataSet xmlVerileriGetir(string xmlAdresi)
        {
            DataTable dt = new DataTable("Doviz");
            dt.Columns.Add("Tarih");
            dt.Columns.Add("Kod");
            dt.Columns.Add("Isim");
            dt.Columns.Add("Alis");
            dt.Columns.Add("Satis");
            dt.Columns.Add("id");


            XDocument mb = XDocument.Load(xmlAdresi);

            if (mb != null)
            {
                var kur = (from p in mb.Element("Tarih_Date").Elements("Currency")
                           select new
                           {
                               Tarih = (string)mb.Element("Tarih_Date").Attribute("Tarih"),
                               Kod = (string)p.Attribute("Kod"),
                               Isim = (string)p.Element("Isim"),
                               Alis = (string)p.Element("ForexBuying"),
                               Satis = (string)p.Element("ForexSelling"),

                           });


                if (kur.AsEnumerable().Count() > 0)
                {
                    DataRow row;
                    foreach (var item in kur)
                    {
                        row = dt.NewRow();
                        row["Tarih"] = item.Tarih;
                        row["Kod"] = item.Kod;
                        row["Isim"] = item.Isim;
                        row["Alis"] = item.Alis;
                        row["Satis"] = item.Satis;

                        dt.Rows.Add(row);

                    }
                }
            }

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;

        }

        private void dgwDoviz_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }


    }
}
