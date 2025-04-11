namespace CookCuisine
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.meublesPanel = new System.Windows.Forms.Panel();
            this.searchMeubles = new System.Windows.Forms.TextBox();
            this.zoneCuisine = new System.Windows.Forms.Panel();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(161)))), ((int)(((byte)(128)))));
            this.panel1.Location = new System.Drawing.Point(1067, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(203, 661);
            this.panel1.TabIndex = 1;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // meublesPanel
            // 
            this.meublesPanel.AccessibleName = "furniturePanel";
            this.meublesPanel.AllowDrop = true;
            this.meublesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.meublesPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.meublesPanel.Location = new System.Drawing.Point(12, 76);
            this.meublesPanel.Name = "meublesPanel";
            this.meublesPanel.Size = new System.Drawing.Size(150, 600);
            this.meublesPanel.TabIndex = 0;
            // 
            // searchMeubles
            // 
            this.searchMeubles.AccessibleName = "searchMeubles";
            this.searchMeubles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchMeubles.Location = new System.Drawing.Point(12, 50);
            this.searchMeubles.Name = "searchMeubles";
            this.searchMeubles.Size = new System.Drawing.Size(150, 20);
            this.searchMeubles.TabIndex = 3;
            // 
            // zoneCuisine
            // 
            this.zoneCuisine.AccessibleName = "zoneCuisine";
            this.zoneCuisine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zoneCuisine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.zoneCuisine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zoneCuisine.Location = new System.Drawing.Point(201, 50);
            this.zoneCuisine.Name = "zoneCuisine";
            this.zoneCuisine.Size = new System.Drawing.Size(800, 600);
            this.zoneCuisine.TabIndex = 0;
            this.zoneCuisine.Paint += new System.Windows.Forms.PaintEventHandler(this.zoneCuisine_Paint);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(1604, 981);
            this.Controls.Add(this.zoneCuisine);
            this.Controls.Add(this.meublesPanel);
            this.Controls.Add(this.searchMeubles);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CookCuisine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel meublesPanel;
        private System.Windows.Forms.TextBox searchMeubles;
        private System.Windows.Forms.Panel zoneCuisine;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}

