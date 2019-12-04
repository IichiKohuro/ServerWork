namespace JsonTest
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtJsonOutput = new System.Windows.Forms.TextBox();
            this.btnExportJson = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtJsonOutput
            // 
            this.txtJsonOutput.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtJsonOutput.Location = new System.Drawing.Point(12, 55);
            this.txtJsonOutput.Multiline = true;
            this.txtJsonOutput.Name = "txtJsonOutput";
            this.txtJsonOutput.Size = new System.Drawing.Size(776, 383);
            this.txtJsonOutput.TabIndex = 0;
            // 
            // btnExportJson
            // 
            this.btnExportJson.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExportJson.Location = new System.Drawing.Point(12, 12);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(174, 37);
            this.btnExportJson.TabIndex = 1;
            this.btnExportJson.Text = "Export Test Json";
            this.btnExportJson.UseVisualStyleBackColor = true;
            this.btnExportJson.Click += new System.EventHandler(this.btnExportJson_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnExportJson);
            this.Controls.Add(this.txtJsonOutput);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtJsonOutput;
        private System.Windows.Forms.Button btnExportJson;
    }
}

