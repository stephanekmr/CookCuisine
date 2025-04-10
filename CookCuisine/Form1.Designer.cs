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
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(161)))), ((int)(((byte)(128)))));
            this.panel1.Location = new System.Drawing.Point(385, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(193, 400);
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
            this.meublesPanel.Location = new System.Drawing.Point(13, 50);
            this.meublesPanel.Name = "meublesPanel";
            this.meublesPanel.Size = new System.Drawing.Size(161, 388);
            this.meublesPanel.TabIndex = 2;
            // 
            // searchMeubles
            // 
            this.searchMeubles.AccessibleName = "searchMeubles";
            this.searchMeubles.Location = new System.Drawing.Point(13, 24);
            this.searchMeubles.Name = "searchMeubles";
            this.searchMeubles.Size = new System.Drawing.Size(161, 20);
            this.searchMeubles.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(241)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.meublesPanel);
            this.Controls.Add(this.searchMeubles);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel meublesPanel;
        private System.Windows.Forms.TextBox searchMeubles;
    }
}

