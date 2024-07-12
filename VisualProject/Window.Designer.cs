
namespace VisualProject
{
    partial class Window
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
            SuspendLayout();
            // 
            // Window
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Name = "Window";
            Text = "Window";
            FormClosing += Window_FormClosing;
            Load += Window_Load;
            Shown += Window_Shown;
            ResizeBegin += Window_ResizeBegin;
            ResizeEnd += Window_ResizeEnd;
            Scroll += Window_Scroll;
            SizeChanged += Window_SizeChanged;
            Paint += Window_Paint;
            KeyDown += Window_KeyDown;
            KeyUp += Window_KeyUp;
            MouseDown += Window_MouseDown;
            MouseMove += Window_MouseMove;
            MouseUp += Window_MouseUp;
            MouseWheel += Window_MouseWheel;
            Resize += Window_Resize;
            ResumeLayout(false);
        }

        #endregion
    }
}
