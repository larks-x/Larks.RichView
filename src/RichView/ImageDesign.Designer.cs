namespace RichView
{
    partial class ImageDesign
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.leftTop = new System.Windows.Forms.PictureBox();
            this.rightTop = new System.Windows.Forms.PictureBox();
            this.leftBottom = new System.Windows.Forms.PictureBox();
            this.rightBottom = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.leftTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightBottom)).BeginInit();
            this.SuspendLayout();
            // 
            // leftTop
            // 
            this.leftTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.leftTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftTop.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.leftTop.Location = new System.Drawing.Point(-1, -1);
            this.leftTop.Name = "leftTop";
            this.leftTop.Size = new System.Drawing.Size(12, 12);
            this.leftTop.TabIndex = 0;
            this.leftTop.TabStop = false;
            // 
            // rightTop
            // 
            this.rightTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rightTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rightTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rightTop.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.rightTop.Location = new System.Drawing.Point(197, -1);
            this.rightTop.Name = "rightTop";
            this.rightTop.Size = new System.Drawing.Size(12, 12);
            this.rightTop.TabIndex = 1;
            this.rightTop.TabStop = false;
            // 
            // leftBottom
            // 
            this.leftBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.leftBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.leftBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftBottom.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.leftBottom.Location = new System.Drawing.Point(-1, 131);
            this.leftBottom.Name = "leftBottom";
            this.leftBottom.Size = new System.Drawing.Size(12, 12);
            this.leftBottom.TabIndex = 2;
            this.leftBottom.TabStop = false;
            // 
            // rightBottom
            // 
            this.rightBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.rightBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rightBottom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rightBottom.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.rightBottom.Location = new System.Drawing.Point(197, 131);
            this.rightBottom.Name = "rightBottom";
            this.rightBottom.Size = new System.Drawing.Size(12, 12);
            this.rightBottom.TabIndex = 3;
            this.rightBottom.TabStop = false;
            this.rightBottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rightBottom_MouseDown);
            this.rightBottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rightBottom_MouseMove);
            this.rightBottom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rightBottom_MouseUp);
            // 
            // ImageDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.rightBottom);
            this.Controls.Add(this.leftBottom);
            this.Controls.Add(this.rightTop);
            this.Controls.Add(this.leftTop);
            this.Name = "ImageDesign";
            this.Size = new System.Drawing.Size(208, 142);
            ((System.ComponentModel.ISupportInitialize)(this.leftTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.leftBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rightBottom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox leftTop;
        private PictureBox rightTop;
        private PictureBox leftBottom;
        private PictureBox rightBottom;
    }
}
