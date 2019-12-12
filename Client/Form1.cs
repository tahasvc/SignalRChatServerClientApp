using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Client
{
    public partial class Form1 : Form
    {
        HubConnection connection; // Connection defination
        IHubProxy chat; // chat proxy defination
        List<Users> users = null; // user list
        Users sender_message = null; // sender message in user
        public Form1()
        {
            InitializeComponent();
        }


        bool connect(string userName)
        {
            connection = new HubConnection("https://localhost:44398/signalr");
            connection.Headers.Add("username", userName);
            chat = connection.CreateHubProxy("ChatHub");
            try
            {
                chat.On<string>("getUserList", (message) =>
                { // getUserList is ChatHub function
                    var json_serialize = new JavaScriptSerializer();
                    users = json_serialize.Deserialize<List<Users>>(message);
                    string[] user_names = users.Select(x => x.username).ToArray();
                    this.BeginInvoke(new Action(() =>
                    {
                        listBox1.Items.Clear();
                        listBox1.Items.AddRange(user_names);//User List in ListBox
                    }));
                });
                chat.On<string, string>("sendMessage", (message, user) => //sendMessage is ChatHub function
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        richTextBox1.Text += user + ":" + message + "\n"; // writing username and message on richTextBox1 
                    }));
                });
                connection.Start().Wait();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (connect(user.Text.Trim()))
            {
                button1.Enabled = false; // Server Connection
                button2.Enabled = true;
            }
            else
            {
                MessageBox.Show("Error");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                sender_message = users[listBox1.SelectedIndex]; //to send a message to the selected person
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
                sender_message = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connection.Stop(); // Connection Stop
            button1.Enabled = true;
            button2.Enabled = false;
            listBox1.Items.Clear();
            sender_message = null;
            button3.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Trim().Equals("") && sender_message != null)
            {
                chat.Invoke("SendMessage", textBox1.Text.Trim(), sender_message.connectionID); //Send message to user
                textBox1.Text = "";
                listBox1.SelectedIndex = -1;
            }
        }
        public class Users  //User Model
        {
            public string username { get; set; }
            public string connectionID { get; set; }
        }
    }
}
