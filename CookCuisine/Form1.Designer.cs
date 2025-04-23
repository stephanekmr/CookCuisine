namespace CookCuisine
{
    partial class principalForm
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

        private System.Windows.Forms.TableLayoutPanel mainTLPanel;
        private System.Windows.Forms.FlowLayoutPanel meubleFLPanel;
        private System.Windows.Forms.FlowLayoutPanel menuFLPanel;
        private System.Windows.Forms.Panel meublesPanel;
        private System.Windows.Forms.MenuStrip menuBar;
        private System.Windows.Forms.ToolStripMenuItem fichier;
        private System.Windows.Forms.ToolStripMenuItem edition;
        private System.Windows.Forms.ToolStripMenuItem aide;
        private System.Windows.Forms.ToolStripMenuItem load;
        private System.Windows.Forms.ToolStripMenuItem save;
        private System.Windows.Forms.ToolStripMenuItem quitter;
        private System.Windows.Forms.Panel cuisinePanel;
        private System.Windows.Forms.Panel zoneCuisine;
        private System.Windows.Forms.Panel gripItem;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.TextBox searchMeubles;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
    }
}

