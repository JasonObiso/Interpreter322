namespace Interpreter322
{
    partial class Interface
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            code_input = new RichTextBox();
            btnRun = new Button();
            SuspendLayout();
            // 
            // code_input
            // 
            code_input.AcceptsTab = true;
            code_input.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            code_input.AutoWordSelection = true;
            code_input.BackColor = Color.Black;
            code_input.BorderStyle = BorderStyle.None;
            code_input.Cursor = Cursors.IBeam;
            code_input.Font = new Font("Lucida Console", 12F, FontStyle.Regular, GraphicsUnit.Point);
            code_input.ForeColor = Color.White;
            code_input.Location = new Point(12, 12);
            code_input.Name = "code_input";
            code_input.Size = new Size(420, 308);
            code_input.TabIndex = 2;
            code_input.Text = "BEGIN CODE\n\n#Place code here...\n\nEND CODE";
            // 
            // btnRun
            // 
            btnRun.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            btnRun.Location = new Point(12, 326);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(420, 34);
            btnRun.TabIndex = 3;
            btnRun.Text = "RUN INTERPRETER";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += btnRun_Click;
            // 
            // Interface
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(444, 372);
            Controls.Add(btnRun);
            Controls.Add(code_input);
            Name = "Interface";
            Opacity = 0.98D;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Interpreter322Final";
            ResumeLayout(false);
        }

        #endregion
        private RichTextBox code_input;
        private Button btnRun;
    }
}