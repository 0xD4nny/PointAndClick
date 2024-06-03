namespace PointAndClick0xDanny
{
    partial class LeaderBoardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listView1 = new ListView();
            label1 = new Label();
            SuspendLayout();
            // 
            // listView1
            // 
            listView1.Alignment = ListViewAlignment.Default;
            listView1.BorderStyle = BorderStyle.None;
            listView1.Font = new Font("Comic Sans MS", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listView1.ForeColor = Color.FromArgb(41, 41, 41);
            listView1.GridLines = true;
            listView1.Location = new Point(30, 50);
            listView1.Name = "listView1";
            listView1.Size = new Size(640, 300);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Tile;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Comic Sans MS", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(200, 10);
            label1.Name = "label1";
            label1.Size = new Size(254, 26);
            label1.TabIndex = 1;
            label1.Text = "Point and Click Leaderboard";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LeaderBoardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(41, 41, 41);
            ClientSize = new Size(700, 450);
            Controls.Add(label1);
            Controls.Add(listView1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LeaderBoardForm";
            StartPosition = FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView1;
        private Label label1;
    }
}