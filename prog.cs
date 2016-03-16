using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

namespace database_server
{
    class prog
    {
        static void Main()
        {
            bool database_flag = false;
            while (true)
            {

                string[] lines = System.IO.File.ReadAllLines(@"C:\Python27\Log.txt");

                // Display the file contents by using a foreach loop.
                System.Console.WriteLine("Contents of packet are as follows ");
                int count = 0;
                foreach (string line in lines)
                {
                    // Use a tab to indent each line of the file.
                    Console.WriteLine("\t" + line);
                    count++;
                }
                string[] packet= new string [9];
                int[] values=new int[3];    //3
                float[] f_value=new float[4];   //0

                Console.WriteLine("\t\nselected data\n");
                count--;
               // string data = "null";
                while (count >= 0)
                {
                    if (lines[count] != "check")
                    {
                        //transfer selested data into packet
                        for (int i = 8; i >= 0; i--)
                        {
                            packet[i] = lines[count];
                            count--;
                        }
                        string[] packet_min=new string[7];      //3

                        int[] strlen = new int[7];
                        for (int j = 0; j < 7; j++)
                        {
                            strlen[j] = packet[j+1].Length;
                        }
                                                   
                        packet_min[0] = packet[1].Substring(11,strlen[0]-11);
                        packet_min[1] = packet[2].Substring(27,strlen[1]-27);
                        packet_min[2] = packet[3].Substring(27,strlen[2]-27);
                        packet_min[3] = packet[4].Substring(11,strlen[3]-11);
                        packet_min[4] = packet[5].Substring(14,strlen[4]-14);
                        packet_min[5] = packet[6].Substring(26,strlen[5]-26);
                        packet_min[6] = packet[7].Substring(29,strlen[6]-29);
                                               
                        for (int i = 0; i < 3; i++) //0  //3
                        {
                            values[i] = Convert.ToInt32(packet_min[i]);
                        }
                        for (int i = 3; i <7 ; i++)
                        {
                            f_value[i-3] = Convert.ToSingle(packet_min[i]);
                        }


                        using (StreamWriter obj = File.AppendText(@"c:\Python27\Log.txt"))
                        {
                            obj.WriteLine("\r\ncheck");                      
                        }

                        database_flag = true;
                        break;
                    }
                    count=-1;
                }
                
                if (database_flag==true)
                {
                   try
                    {
                        string connectionString = "Data Source=PC;Initial Catalog=SIAAP;Persist Security Info" +
                                                  "=True;User ID=sa;Password=admin";
                        SqlConnection conn = new SqlConnection(connectionString);
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("sp_data_received", conn);

                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@obj0", SqlDbType.Int).Value = values[0]; // for Sensor ID
                        cmd.Parameters.Add("@obj1", SqlDbType.Int).Value = values[1];
                        cmd.Parameters.Add("@obj2", SqlDbType.Int).Value = values[2];
                        cmd.Parameters.Add("@obj3", SqlDbType.Int).Value = f_value[0];
                        cmd.Parameters.Add("@obj4", SqlDbType.Int).Value = f_value[1];
                        cmd.Parameters.Add("@obj5", SqlDbType.Int).Value = f_value[2];
                        cmd.Parameters.Add("@obj6", SqlDbType.Int).Value = f_value[3];

                        DateTime saveNow = DateTime.Now;
                        string time = Convert.ToString(saveNow);
                        cmd.Parameters.Add("@obj7", SqlDbType.VarChar).Value = time;
                                             
                        cmd.ExecuteNonQuery();
                        database_flag = false;
                        conn.Close();

                    }
                    catch (Exception database)
                    {
                        Console.WriteLine("Database error");
                    }
                }

                Console.WriteLine("Restarting");
                Thread.Sleep(2000);

            }
        }
    }
}
