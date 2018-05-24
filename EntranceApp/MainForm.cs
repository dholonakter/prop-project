using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Phidget22;
using Phidget22.Events;
using EntranceApp.Helper;
using System.Drawing;
using System.Threading;
using EntranceApp.Models;

namespace EntranceApp
{
    public partial class MainForm : Form, ILogger
    {
        #region Public Constants
        public const string lblDefaultText = "...........................";
        #endregion

        #region Private Fields
        private DatabaseHelper dh;
        private RFID myRFIDReader;
        private bool isAllowedToLinkOrUnLinkCard;
        #endregion

        #region Constructor
        public MainForm()
        {
            InitializeComponent();
            try
            {
                dh = new DatabaseHelper(this);
                myRFIDReader = new RFID();

            }
            catch (PhidgetException)
            {
                lbxLogMessage.Items.Add("startup: so far so good.");

            }
        }
        #endregion

        #region Private Methods
        private bool InputValidation()
        {
            return !String.IsNullOrEmpty(tbName.Text) &&
                   !String.IsNullOrEmpty(tbEmailAddress.Text) &&
                   !String.IsNullOrEmpty(tbPhoneNumber.Text) &&
                   !String.IsNullOrEmpty(tbrfid.Text) &&
                   !String.IsNullOrEmpty(cbxSelectedAmount.Text);
        }
        private void ResetControls()
        {
            lblFullName.Text = lblDefaultText;
            lblEmailAddress.Text = lblDefaultText;
            lblRFIDCode.Text = lblDefaultText;
            lblMessage.Text = lblDefaultText;
            lblMessage.BackColor = SystemColors.Control;
            btnCheckIn.BackColor = SystemColors.Control;
            btnCheckOut.BackColor = SystemColors.Control;
        }

        private void ResetBuyTicketsGroupControls()
        {
            tbName.Text = string.Empty;
            tbEmailAddress.Text = string.Empty;
            tbPhoneNumber.Text = string.Empty;
            tbrfid.Text = string.Empty;
            lblCardLinkedStatus.Text = lblDefaultText;
            cbxSelectedAmount.Text = string.Empty;
            rbtnLinkCard.Checked = false;
            rbtnUnLinkCard.Checked = false;
        }
        #endregion

        #region Private Event Handlers Methods

        private void ShowErrorMessage(string message)
        {
            lblMessage.Text = "Cannot find visitor";
            lblMessage.BackColor = Color.Red;
        }
        private void ShowSuccessMessage(string message)
        {
            lblMessage.BackColor = Color.Green;
            lblMessage.Text = message;
        }
        private void ResetControlsBackColor()
        {
            lblMessage.BackColor = SystemColors.Control;
            btnCheckIn.BackColor = SystemColors.Control;
            btnCheckOut.BackColor = SystemColors.Control;
        }
        private void SetLabelsTextOfCheckInCheckOutGroupBox(Visitor visitor)
        {
            if (visitor != null)
            {
                lblFullName.Text = visitor.FullName;
                lblEmailAddress.Text = visitor.EmailAddress;
                lblRFIDCode.Text = visitor.RFID;
                if (visitor.IsCheckedIn)
                {
                    ShowSuccessMessage("Check In Success");
                    btnCheckIn.BackColor = Color.Green;
                    btnCheckOut.BackColor = SystemColors.Control; 
                }
                else
                {
                    ShowSuccessMessage("Check Out Success");
                    btnCheckOut.BackColor = Color.Green;
                    btnCheckIn.BackColor = SystemColors.Control;
                }
            }
        }
        private void CardCheckedInOrOut(string rfidCode)//Rfid rfid)
        {
            if (!String.IsNullOrEmpty(rfidCode))//rfid != null)
            {
                Visitor checkedInVisitor;
                if (dh.MakeCheckInOrOut(rfidCode, out checkedInVisitor))//rfid.Code, out checkedInVisitor))
                {
                    SetLabelsTextOfCheckInCheckOutGroupBox(checkedInVisitor);
                    timerResetControls.Start();
                }
                else
                {
                    ResetControlsBackColor();
                    ShowErrorMessage("Cannot find vistor");
                    timerResetControls.Start();
                }
            }
        }

        private void SetTextBoxTextWhenUnLinkRadioButtonIsChecked(string rfidCode)
        {
            Visitor foundVisitor = dh.FindVisitor(rfidCode);
            tbName.Text = foundVisitor.FullName;
            tbEmailAddress.Text = foundVisitor.EmailAddress;
            tbPhoneNumber.Text = foundVisitor.PhoneNumber.ToString();
            tbrfid.Text = foundVisitor.RFID;
            cbxSelectedAmount.Text = foundVisitor.Balance.ToString();
            lblCardLinkedStatus.Text = "Card is Linked";
        }
        private void ProcessThisTag(object sender, RFIDTagEventArgs e)
        {
           // Rfid foundRFID = null; //dh.GetRFID(e.Tag);

            //if (foundRFID==null) //!= null)
            {
                if (rbtnLinkCard.Checked)
                {
                   // if (!foundRFID.IsCardLinked)
                    {
                        isAllowedToLinkOrUnLinkCard = true;
                        tbrfid.Text = e.Tag; //foundRFID.Code;
                    }

                }
                else if (rbtnUnLinkCard.Checked)
                {
                   // if (foundRFID.IsCardLinked)
                    {
                        isAllowedToLinkOrUnLinkCard = true;
                        SetTextBoxTextWhenUnLinkRadioButtonIsChecked(e.Tag);//foundRFID.Code);
                    }

                }
                else
                {
                    CardCheckedInOrOut(e.Tag);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                myRFIDReader.Tag += new RFIDTagEventHandler(ProcessThisTag);
                myRFIDReader.Open();
                LogMessage(ErrorType.RFID, "Open");
            }
            catch (PhidgetException)
            {
                LogMessage(ErrorType.RFID, "No RFID-reader opened");
            }
        }
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {

            myRFIDReader.Tag -= new RFIDTagEventHandler(ProcessThisTag);
            myRFIDReader.Close();
            LogMessage(ErrorType.RFID, "Closed");
        }

        private void TimerToResetControls_Tick(object sender, EventArgs e)
        {
            if (isAllowedToLinkOrUnLinkCard)
            {
                ResetBuyTicketsGroupControls();
                isAllowedToLinkOrUnLinkCard = false;
            }
            else
            {
                ResetControls();
            }
            timerResetControls.Stop();
        }
        private void WhenPerformButtonIsClicked(object sender, EventArgs e)
        {
            string name, email, rfidCode;
            int phone;
            double balance;
            if (isAllowedToLinkOrUnLinkCard)
            {
                if (rbtnLinkCard.Checked)
                {
                    if (InputValidation())
                    {
                        name = tbName.Text;
                        email = tbEmailAddress.Text;
                        phone = int.Parse(tbPhoneNumber.Text);
                        rfidCode = tbrfid.Text;
                        balance = Convert.ToDouble(cbxSelectedAmount.Text);
                        if (dh.LinkOrUnLinkCard(name, email, phone, balance, true, rfidCode))
                        {
                            lblCardLinkedStatus.Text = "Linked Successfully";
                            timerResetControls.Start();
                        }
                        else
                        {
                            lblCardLinkedStatus.Text = "Card is already Linked";
                            timerResetControls.Start();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Not All Fields are complete!!!");
                    }
                }
                else if (rbtnUnLinkCard.Checked)
                {
                    if (InputValidation())
                    {
                        name = tbName.Text;
                        email = tbEmailAddress.Text;
                        phone = int.Parse(tbPhoneNumber.Text);
                        rfidCode = tbrfid.Text;
                        balance = Convert.ToInt32(cbxSelectedAmount.Text);
                        if (dh.LinkOrUnLinkCard(name, email, phone, balance, false, rfidCode))
                        {
                            lblCardLinkedStatus.Text = "UnLinked Successfully";
                            timerResetControls.Start();
                        }
                        else
                        {
                            lblCardLinkedStatus.Text = "Card is not Linked yet";
                            timerResetControls.Start();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Not All Fields are complete!!!");
                    }

                }
                else
                {
                    MessageBox.Show("Please select one of the option Link or Unlink card");
                }
            }
        }

        private void tbPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            ResetBuyTicketsGroupControls();
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            lbxLogMessage.Items.Clear();
        }
        #endregion

        #region ILogger Implementation
        public void LogMessage(ErrorType errorType, string message)
        {
            lbxLogMessage.Items.Add(errorType + ":" + message);
        }
        #endregion

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
