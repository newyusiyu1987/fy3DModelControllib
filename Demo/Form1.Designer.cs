namespace Demo
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.fy3DModelManager1 = new fy3DModelControllib.fy3DModelManager();
            this.fy3DModelCanvas1 = new fy3DModelControllib.fy3DModelCanvas();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 24);
            this.button1.TabIndex = 1;
            this.button1.Text = "Open fyGrid";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // fy3DModelManager1
            // 
            this.fy3DModelManager1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fy3DModelManager1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fy3DModelManager1.Enabled = false;
            this.fy3DModelManager1.Location = new System.Drawing.Point(4, 31);
            this.fy3DModelManager1.Name = "fy3DModelManager1";
            this.fy3DModelManager1.Size = new System.Drawing.Size(251, 552);
            this.fy3DModelManager1.TabIndex = 4;
            // 
            // fy3DModelCanvas1
            // 
            this.fy3DModelCanvas1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fy3DModelCanvas1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fy3DModelCanvas1.Location = new System.Drawing.Point(261, 31);
            this.fy3DModelCanvas1.Name = "fy3DModelCanvas1";
            this.fy3DModelCanvas1.Size = new System.Drawing.Size(759, 552);
            this.fy3DModelCanvas1.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(157, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(123, 24);
            this.button2.TabIndex = 5;
            this.button2.Text = "Save fyGrid";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1022, 585);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.fy3DModelManager1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.fy3DModelCanvas1);
            this.Name = "Form1";
            this.Text = "fy3D Engine";
            this.ResumeLayout(false);

        }

        #endregion

        private fy3DModelControllib.fy3DModelCanvas fy3DModelCanvas1;
        private System.Windows.Forms.Button button1;
        private fy3DModelControllib.fy3DModelManager fy3DModelManager1;
        private System.Windows.Forms.Button button2;
    }
}

