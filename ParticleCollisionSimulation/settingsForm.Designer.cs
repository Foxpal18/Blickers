namespace ParticleCollisionSimulation
{
    partial class settingsForm
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
            this.sParticleCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sMassMultiplier = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sVelocityMultiplier = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sImpulse = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // sParticleCount
            // 
            this.sParticleCount.Location = new System.Drawing.Point(13, 13);
            this.sParticleCount.Name = "sParticleCount";
            this.sParticleCount.Size = new System.Drawing.Size(100, 20);
            this.sParticleCount.TabIndex = 0;
            this.sParticleCount.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(119, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Particle Count (Default: 100)";
            // 
            // sMassMultiplier
            // 
            this.sMassMultiplier.Location = new System.Drawing.Point(13, 39);
            this.sMassMultiplier.Name = "sMassMultiplier";
            this.sMassMultiplier.Size = new System.Drawing.Size(100, 20);
            this.sMassMultiplier.TabIndex = 2;
            this.sMassMultiplier.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mass Multiplier (Default: 10)";
            // 
            // sVelocityMultiplier
            // 
            this.sVelocityMultiplier.Location = new System.Drawing.Point(13, 65);
            this.sVelocityMultiplier.Name = "sVelocityMultiplier";
            this.sVelocityMultiplier.Size = new System.Drawing.Size(100, 20);
            this.sVelocityMultiplier.TabIndex = 4;
            this.sVelocityMultiplier.Text = "12";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(119, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Velocity Multiplier (Default: 12)";
            // 
            // sImpulse
            // 
            this.sImpulse.Location = new System.Drawing.Point(13, 92);
            this.sImpulse.Name = "sImpulse";
            this.sImpulse.Size = new System.Drawing.Size(100, 20);
            this.sImpulse.TabIndex = 6;
            this.sImpulse.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(119, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Impulse (Default: 1)";
            // 
            // settingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 126);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sImpulse);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sVelocityMultiplier);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.sMassMultiplier);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sParticleCount);
            this.DoubleBuffered = true;
            this.Name = "settingsForm";
            this.Text = "Blickers v1.3 - Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox sParticleCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sMassMultiplier;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox sVelocityMultiplier;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sImpulse;
        private System.Windows.Forms.Label label4;
    }
}

