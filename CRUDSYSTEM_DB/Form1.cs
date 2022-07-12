using System;
using Npgsql;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDSYSTEM_DB
{
    public partial class Form1 : Form
    {
        private string id = "";
        private int intRow = 0;
        public Form1()
        {
            InitializeComponent();
            resetMe();
        }
        private void resetMe()
        {

            this.id = string.Empty;

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";

            if (genderComboBox.Items.Count > 0)
            {
                genderComboBox.SelectedIndex = 0;
            }

            updateButton.Text = "Update ()";
            deleteButton.Text = "Delete ()";

            keywordTextBox.Clear();

            if (keywordTextBox.CanSelect)
            {
                keywordTextBox.Select();
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            loadData("", dgv1);
        }
        private void loadData(string keyword, DataGridView dgv1)
        {

            CRUD.sql = "SELECT id, firstname, lastname, CONCAT(firstname, ' ', lastname) AS fullname, gender FROM tb_crudsystem " +
                       "WHERE CONCAT(CAST(id as varchar), ' ', firstname, ' ', lastname) LIKE @keyword::varchar " +
                       "OR TRIM(gender) LIKE @keyword::varchar ORDER BY id ASC";

            string strKeyword = string.Format("%{0}%", keyword);

            CRUD.cmd = new NpgsqlCommand(CRUD.sql, CRUD.con);
            CRUD.cmd.Parameters.Clear();
            CRUD.cmd.Parameters.AddWithValue("keyword", strKeyword);

            DataTable dt = CRUD.PerformCRUD(CRUD.cmd);

            if (dt.Rows.Count > 0)
            {
                intRow = Convert.ToInt32(dt.Rows.Count.ToString());
            }
            else
            {
                intRow = 0;
            }

            toolStripStatusLabel1.Text = "Number of row(s): " + intRow.ToString();
            dgv1.MultiSelect = false;
            dgv1.AutoGenerateColumns = true;
            dgv1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv1.DataSource = dt;

            dgv1.Columns[0].HeaderText = "ID";
            dgv1.Columns[1].HeaderText = "First Name";
            dgv1.Columns[2].HeaderText = "Last Name";
            dgv1.Columns[3].HeaderText = "Full Name";
            dgv1.Columns[4].HeaderText = "Gender";

            dgv1.Columns[0].Width = 85;
            dgv1.Columns[1].Width = 170;
            dgv1.Columns[2].Width = 170;
            dgv1.Columns[3].Width = 220;
            dgv1.Columns[4].Width = 100;

        }

        private void execute(string mySQL, string param)
        {
            CRUD.cmd = new NpgsqlCommand(mySQL, CRUD.con);
            addParameters(param);
            CRUD.PerformCRUD(CRUD.cmd);
        }

        private void addParameters(string str)
        {
            CRUD.cmd.Parameters.Clear();
            CRUD.cmd.Parameters.AddWithValue("firstName", firstNameTextBox.Text.Trim());
            CRUD.cmd.Parameters.AddWithValue("lastName", lastNameTextBox.Text.Trim());
            CRUD.cmd.Parameters.AddWithValue("gender", genderComboBox.SelectedItem.ToString());

            if (str == "Update" || str == "Delete" && !string.IsNullOrEmpty(this.id))
            {
                CRUD.cmd.Parameters.AddWithValue("id", this.id);
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                this.id = Convert.ToString(dgv1.CurrentRow.Cells[0].Value);
                updateButton.Text = "Update (" + this.id + ")";
                deleteButton.Text = "Delete (" + this.id + ")";

                firstNameTextBox.Text = Convert.ToString(dgv1.CurrentRow.Cells[1].Value);
                lastNameTextBox.Text = Convert.ToString(dgv1.CurrentRow.Cells[2].Value);

                genderComboBox.SelectedItem = Convert.ToString(dgv1.CurrentRow.Cells[4].Value);

            }
        }
        private void insertButton_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(firstNameTextBox.Text.Trim()) || string.IsNullOrEmpty(lastNameTextBox.Text.Trim()))
            {
                MessageBox.Show("Please input first name and  last name.", "Insert Data : Admin said yes!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            CRUD.sql = "INSERT INTO tb_crudsystem (firstname, lastname, gender) VALUES(@firstName, @lastName, @gender)";

            execute(CRUD.sql, "Insert");

            MessageBox.Show("The record has been saved.", "Insert Data : Admin said yes!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            loadData("", dgv1);

            resetMe();
        }

        private void updateButton_Click_1(object sender, EventArgs e)
        {
            if (dgv1.Rows.Count == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.id))
            {
                MessageBox.Show("Please select an item from the list.", "Update Data : Admin said yes!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (string.IsNullOrEmpty(firstNameTextBox.Text.Trim()) || string.IsNullOrEmpty(lastNameTextBox.Text.Trim()))
            {
                MessageBox.Show("Please input first name and  last name.", "Insert Data : Admin said yes!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            CRUD.sql = "UPDATE tb_crudsystem SET firstName = @firstName, lastname = @lastName, gender = @gender WHERE id = @id::integer";

            execute(CRUD.sql, "Update");

            MessageBox.Show("The record has been updated.", "Update Data : Admin said yes!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            loadData("", dgv1);

            resetMe();
        }

        private void deleteButton_Click_1(object sender, EventArgs e)
        {
            if (dgv1.Rows.Count == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(this.id))
            {
                MessageBox.Show("Please select an item from the list.", "Delete Data : Admin said yes!",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MessageBox.Show("Do you want to permanently delete the selected record?", "Delete Data : Admin said yes!",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {

                CRUD.sql = "DELETE FROM tb_crudsystem WHERE id = @id::integer";

                execute(CRUD.sql, "Update");

                MessageBox.Show("The record has been deleted.", "Delete Data : Admin said yes!",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                loadData("", dgv1);

                resetMe();

            }
        }

        private void button1_Click(object sender, EventArgs e) //search button
        {
            // Let's try :)
            if (string.IsNullOrEmpty(keywordTextBox.Text.Trim()))
            {
                loadData("", dgv1);
            }
            else
            {
                loadData(keywordTextBox.Text.Trim(), dgv1);
            }
            resetMe();
        }
    }
}
