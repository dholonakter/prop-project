using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using MySql.Data;
using MySql.Data.MySqlClient;
using EntranceApp.Helper;
using EntranceApp.Models;

namespace EntranceApp
{
    public class DatabaseHelper
    {
        #region Private Fields
        private MySqlConnection connection;
        private ILogger logger;
        #endregion

        #region Properties
        public bool Isconnected { get; set; }
        #endregion

        #region Constructor
        public DatabaseHelper(ILogger logger)
        {
            this.logger = logger;
            try
            {
                String connectionInfo = "server=studmysql01.fhict.local;" +//the hera-server
                               "database=dbi364365;" +
                               "user id=dbi364365;" +
                               "password=Dholon;";
                connection = new MySqlConnection(connectionInfo);
                
          
                LoggingError(ErrorType.Database, "Connected");
                
                Isconnected = true;
            }
            catch
            {
                connection.Close();
                LoggingError(ErrorType.Database, "Something went wrong cannot connect");
                
            }
        }
        #endregion

        #region Private Methods
        private void LoggingError(ErrorType errorType, String message)
        {
            if (this.logger != null)
            {
                logger.LogMessage(errorType, message);
            }
        }
        private Rfid FindRFID(string rfidCode)
        {
            Rfid foundRfid = null;
            string sql = "SELECT * FROM rfid WHERE Code ='" + rfidCode+"'";
            
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();
                
                int id;
                string code;
                bool isCardLinked;
                int? vistorId;
                // reading
                while (reader.Read())
                {
                    id = Convert.ToInt32(reader["Id"]);
                    code = Convert.ToString(reader["Code"]);
                    isCardLinked = Convert.ToBoolean(reader["IsCardLinked"]);
                    vistorId = reader["Visitor_Id"] as int?;
                    foundRfid = new Rfid(id, code, isCardLinked, vistorId);
                }
            }
            catch (MySqlException)
            {
                LoggingError(ErrorType.MySqlException, "FindRFID: Something went wrong while querying");
            }
            finally
            {
                connection.Close();
            }
            return foundRfid;
        }

        public Visitor FindVisitor(string rfidCode)
        {
            Visitor foundVisitor = null;
            string sql = "SELECT * FROM visitor WHERE RFID ='" + rfidCode + "'";

            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(sql, connection);
                MySqlDataReader reader = command.ExecuteReader();

                int id;
                string fullName;
                int phone;
                string mail;
                bool isCardLinked, isCheckedIn;
                double balance;
                // reading
                while (reader.Read())
                {
                    id = Convert.ToInt32(reader["Id"]);
                    fullName = Convert.ToString(reader["FullName"]);
                    mail = Convert.ToString(reader["EmailAddress"]);
                    phone = Convert.ToInt32(reader["PhoneNumber"]);
                    isCardLinked = Convert.ToBoolean(reader["IsCardLinked"]);
                    isCheckedIn = Convert.ToBoolean(reader["IsCheckedIn"]);
                    balance = Convert.ToInt32(reader["Balance"]);
                    foundVisitor = new Visitor(fullName, mail, phone, balance, rfidCode);
                    foundVisitor.IsCheckedIn = isCheckedIn;
                    foundVisitor.IsCardLinked = isCardLinked;
                    foundVisitor.Id = id;
                }
            }
            catch (MySqlException)
            {
                LoggingError(ErrorType.MySqlException, "Something went wrong while querying");
            }
            finally
            {
                connection.Close();
            }
            return foundVisitor;
        }
        private bool UpdateVisitorTableIsCheckedInColumn(string rfidCode, bool checkedIn)
        {
            bool onSuccess = false;

            string sql = "UPDATE visitor " +
            "SET IsCheckedIn = " + checkedIn +
             " WHERE RFID='" + rfidCode+"'";

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                if (command.ExecuteNonQuery() > 0)
                {
                    onSuccess = true;
                }

            }
            catch (MySqlException)
            {

                LoggingError(ErrorType.MySqlException, "Something went wrong while querying");

            }
            finally
            {
                connection.Close();
            }
            return onSuccess;
        }

        private bool DeleteVisitorFromTable(string rfidCode)
        {
            bool onSuccess = false;

            string sql = "DELETE FROM visitor WHERE RFID='"+rfidCode+"'";


            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                if (command.ExecuteNonQuery() > 0)
                {
                    onSuccess = true;
                }

            }
            catch (MySqlException)
            {

                LoggingError(ErrorType.MySqlException, "DeleteVisitorFromTable: Something went wrong while deleting");

            }
            finally
            {
                connection.Close();
            }
            return onSuccess;
        }

        private bool UpdateRfidTableIsCardLinkedColumn(string rfidCode, bool isCardLinked)
        {
            bool onSuccess = false;

            string sql = "UPDATE rfid " +
            "SET IsCardLinked = " + isCardLinked +
             " WHERE Code='" + rfidCode + "'";

            try
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand(sql, connection);
                if (command.ExecuteNonQuery() > 0)
                {
                    onSuccess = true;
                }

            }
            catch (MySqlException)
            {

                LoggingError(ErrorType.MySqlException, "UpdateRfidColumn: Something went wrong while updating");

            }
            finally
            {
                connection.Close();
            }
            return onSuccess;
        }

        private bool AddVisitorToDeletedVisitorTable(Visitor visitorTobeAdded)
        {
            bool onSuccess = false;

            String sql = "INSERT INTO deletedvisitor (FullName,EmailAddress,PhoneNumber,Balance,RFID,VisitorId)" +
               "VALUES  (@FullName,@EmailAddress,@PhoneNumber,@Balance,@RFID,@VisitorId)";
            MySqlCommand command = new MySqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FullName", visitorTobeAdded.FullName);
            command.Parameters.AddWithValue("@EmailAddress", visitorTobeAdded.EmailAddress);
            command.Parameters.AddWithValue("@PhoneNumber", visitorTobeAdded.PhoneNumber);
            command.Parameters.AddWithValue("@RFID", visitorTobeAdded.RFID);
            command.Parameters.AddWithValue("@Balance", visitorTobeAdded.Balance);
            command.Parameters.AddWithValue("@VisitorId", visitorTobeAdded.Id);

            try
            {

                connection.Open();
                if (command.ExecuteNonQuery() > 0)
                {
                    connection.Close();
                    onSuccess = true;
                }


            }
            catch
            {
                LoggingError(ErrorType.MySqlException, "AddVisitorToDeletedVisitorTable: Error while adding visitor");
            }
            finally
            {
                connection.Close();
            }
            return onSuccess;
        }
        #endregion

        /*
        private void change(ref Visitor visitor)
        {
            visitor.FullName = "";
        }
        */
        #region Public Methods
        /// <summary>
        /// Update IscheckedIn column value of database true or flase
        /// </summary>
        /// <param name="rfidCode"></param>
        /// <param name="foundVistor"></param>
        /// <returns></returns>
        public bool MakeCheckInOrOut(string rfidCode, out Visitor foundVistor)
        {
            foundVistor = FindVisitor(rfidCode);            
            bool onSuccess = false;
            if(foundVistor!=null)
            {
               if(foundVistor.IsCheckedIn && foundVistor.IsCardLinked)
                {
                    foundVistor.IsCheckedIn = false;
                    onSuccess= UpdateVisitorTableIsCheckedInColumn(rfidCode, false);
                }
                else if(foundVistor.IsCardLinked)
                {
                    foundVistor.IsCheckedIn = true;
                    onSuccess= UpdateVisitorTableIsCheckedInColumn(rfidCode, true);
                }
            }
            return onSuccess;
        }
        public Rfid GetRFID(string rfidCode)
        {
            return FindRFID(rfidCode);
        }

        public List<Visitor> GetAllVisitors()
        {
            String sql = "SELECT * FROM Visitor";
            MySqlCommand command = new MySqlCommand(sql, connection);
            List<Visitor> temp = new List<Visitor>();
            try
            {
                MySqlDataReader reader = command.ExecuteReader();
                int id;
                string name;
                string emailAddress;
                int phoneNumber;
                double balance;
                string rfid;
                while (reader.Read())
                {
                    id = Convert.ToInt32(reader["Id"]);
                    name = Convert.ToString(reader["FullName"]);
                    emailAddress = Convert.ToString(reader["EmailAddress"]);
                    phoneNumber = Convert.ToInt32(reader["PhoneNumber"]);
                    balance = Convert.ToDouble(reader["Balance"]);
                    rfid = Convert.ToString(reader["RFID"]);
                    temp.Add(new Visitor(name, emailAddress,phoneNumber, balance, rfid));
                }
            }
            catch
            {
                MessageBox.Show("error while loading the Visitors");
            }
            finally
            {
                connection.Close();
            }
            return temp;
        }

        public bool LinkOrUnLinkCard(string name, string emailAddress, int phoneNumber, double balance, bool linkCard, string rfid)
        {
            bool onSuccess = false;
            Visitor foundvisitor = FindVisitor(rfid);
            if (linkCard)
            {
                if (foundvisitor == null)
                {
                    String sql = "INSERT INTO visitor (FullName,EmailAddress,PhoneNumber,Balance,RFID,IsCardLinked,IsCheckedIn)" +
                                  "VALUES  (@FullName,@EmailAddress,@PhoneNumber,@Balance,@RFID,@IsCardLinked,@IsCheckedIn )";
                    MySqlCommand command = new MySqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@FullName", name);
                    command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                    command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    command.Parameters.AddWithValue("@RFID", rfid);
                    command.Parameters.AddWithValue("@Balance", balance);
                    command.Parameters.AddWithValue("@IsCardLinked", linkCard);
                    command.Parameters.AddWithValue("@IsCheckedIn", false);

                    try
                    {

                        connection.Open();
                        if (command.ExecuteNonQuery() > 0)
                        {
                            connection.Close();
                            onSuccess = true; //UpdateRfidTableIsCardLinkedColumn(rfid, linkCard);
                        }


                    }
                    catch
                    {
                        LoggingError(ErrorType.MySqlException, "AddVisitor: Error while adding visitor");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
            else
            {
                if (foundvisitor != null)
                {
                    onSuccess = AddVisitorToDeletedVisitorTable(foundvisitor);
                    if(onSuccess)
                    {
                        onSuccess = DeleteVisitorFromTable(rfid);
                        /*
                        if (onSuccess)
                        {
                            onSuccess = UpdateRfidTableIsCardLinkedColumn(rfid, false);
                        }
                        */
                    }
                   
                }
            }
            return onSuccess;
        }
        #endregion        
    }
}
