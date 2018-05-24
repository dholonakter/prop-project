using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Phidget22;
using Phidget22.Events;



namespace EntranceApp
{
    public partial class Form1 : Form
    {
        private DataHelper dh;
        private RFID myRFIDReader;


        public Form1()
        {
            InitializeComponent();
            try
            {
                dh = new DataHelper();
                myRFIDReader = new RFID();
                myRFIDReader.Attach += new AttachEventHandler(ShowWhoIsAttached);
                myRFIDReader.Detach += new DetachEventHandler(ShowWhoIsDetached);
                myRFIDReader.Tag += new RFIDTagEventHandler(ProcessThisTag);
                listBox1.Items.Add("Welcome to our Event");
            }
            catch(PhidgetException)
            {
                listBox1.Items.Add("startup: so far so good.");

            }



        }
        private void ShowWhoIsAttached(object sender, AttachEventArgs e)
        {
            listBox1.Items.Add("RFIDReader attached!, device serial nr: " + myRFIDReader.DeviceSerialNumber);
        }
        private void ShowWhoIsDetached(object sender, DetachEventArgs e)
        {
            listBox1.Items.Add("RFIDReader detached!, device serial nr: " + myRFIDReader.DeviceSerialNumber);
        }
        private void ProcessThisTag(object sender, RFIDTagEventArgs e)
        {
            listBox1.Items.Add("rfid has tag-nr: " + e.Tag);
        }





        private void button3_Click(object sender, EventArgs e)
        {
            List<Visitors> personlist;
            personlist = dh.GetAllVisitors();

            this.listBox1.Items.Clear();
            foreach (Visitors s in personlist)
            {
                AddToListBox(s);
            }

        }

        /// <summary>
        /// Add visitor information to the list box
        /// </summary>
        /// <param name="visitor">Reference to visitor</param>
        private void AddToListBox(Visitors visitor)
        {
            if (visitor == null)
                return;

            listBox1.Items.Add("FullName: " + visitor.FullName);
            listBox1.Items.Add("EmailAddress: " + visitor.EmailAddress);
        }
        private void button4_Click(object sender, EventArgs e)
        { 
            String name;
            int phoneNumber;
            string emailaddress;
            string rfid;

            if (tbName.Text != "" && tbEmailAddress.Text != "" && tbrfid.Text != ""&& tbPhoneNumber.Text!="")
            {
                name = tbName.Text;
                emailaddress = tbName.Text;
                phoneNumber =Convert.ToInt32(tbPhoneNumber.Text);
                rfid = tbrfid.Text;

                phoneNumber = Convert.ToInt32(tbPhoneNumber.Text);
                int nrAdded = dh.AddVisitor (name, emailaddress,phoneNumber,0,rfid);
                if (nrAdded > 0)
                {
                    MessageBox.Show("Succesfully added to the database");
                }
                else
                {
                    MessageBox.Show("Error while adding a visitor to the database");
                }
            }

        }

        private void button1_Click(object sender, RFIDTagEventArgs e)
        {
            MessageBox.Show("Hello visitor with rfid nr" + e.Tag + "Welcome to our event");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
