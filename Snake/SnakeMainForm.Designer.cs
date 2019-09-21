namespace Snake
{
    partial class SnakeMainForm
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
            this.PanelArena = new Snake.Controls.DoubleBufferedPanel();
            this.SuspendLayout();
            // 
            // PanelArena
            // 
            this.PanelArena.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelArena.Location = new System.Drawing.Point(10, 9);
            this.PanelArena.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PanelArena.Name = "PanelArena";
            this.PanelArena.Size = new System.Drawing.Size(429, 306);
            this.PanelArena.TabIndex = 2;
            this.PanelArena.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelArena_Paint);
            // 
            // SnakeMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 324);
            this.Controls.Add(this.PanelArena);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SnakeMainForm";
            this.Text = "Snake";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SnakeMainForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
        private Snake.Controls.DoubleBufferedPanel PanelArena;
    }
}

