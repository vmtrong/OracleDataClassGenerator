using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OracleDataClassGenerator.Controls
{
    /// <summary>
    /// <para>An auto sized label with the ability to display text with formattings by using the Rich Text Format.</para>
    /// <para>­</para>
    /// <para>Short RTF syntax examples: </para>
    /// <para>­</para>
    /// <para>Paragraph: </para>
    /// <para>{\pard This is a paragraph!\par}</para>
    /// <para>­</para>
    /// <para>Bold / Italic / Underline: </para>
    /// <para>\b bold text\b0</para>
    /// <para>\i italic text\i0</para>
    /// <para>\ul underline text\ul0</para>
    /// <para>­</para>
    /// <para>Alternate color using color table: </para>
    /// <para>{\colortbl ;\red0\green77\blue187;}{\pard The word \cf1 fish\cf0  is blue.\par</para>
    /// <para>­</para>
    /// <para>Additional information: </para>
    /// <para>Always wrap every text in a paragraph. </para>
    /// <para>Different tags can be stacked (i.e. \pard\b\i Bold and Italic\i0\b0\par)</para>
    /// <para>The space behind a tag is ignored. So if you need a space behind it, insert two spaces (i.e. \pard The word \bBOLD\0  is bold.\par)</para>
    /// <para>Full specification: http://www.biblioscape.com/rtf15_spec.htm </para>
    /// </summary>
    public partial class AutoRichLabel : UserControl
    {
        /// <summary>
        /// The rich text content. 
        /// <para>­</para>
        /// <para>Short RTF syntax examples: </para>
        /// <para>­</para>
        /// <para>Paragraph: </para>
        /// <para>{\pard This is a paragraph!\par}</para>
        /// <para>­</para>
        /// <para>Bold / Italic / Underline: </para>
        /// <para>\b bold text\b0</para>
        /// <para>\i italic text\i0</para>
        /// <para>\ul underline text\ul0</para>
        /// <para>­</para>
        /// <para>Alternate color using color table: </para>
        /// <para>{\colortbl ;\red0\green77\blue187;}{\pard The word \cf1 fish\cf0  is blue.\par</para>
        /// <para>­</para>
        /// <para>Additional information: </para>
        /// <para>Always wrap every text in a paragraph. </para>
        /// <para>Different tags can be stacked (i.e. \pard\b\i Bold and Italic\i0\b0\par)</para>
        /// <para>The space behind a tag is ignored. So if you need a space behind it, insert two spaces (i.e. \pard The word \bBOLD\0  is bold.\par)</para>
        /// <para>Full specification: http://www.biblioscape.com/rtf15_spec.htm </para>
        /// </summary>
        [Browsable(true)]
        public string RtfContent
        {
            get
            {
                return this.rtb.Rtf;
            }
            set
            {
                this.rtb.WordWrap = false; // to prevent any display bugs, word wrap must be off while changing the rich text content. 
                this.rtb.Rtf = value.StartsWith(@"{\rtf1") ? value : @"{\rtf1" + value + "}"; // Setting the rich text content will trigger the ContentsResized event. 
                this.Fit(); // Override width and height. 
                this.rtb.WordWrap = this.WordWrap; // Set the word wrap back. 
            }
        }

        /// <summary>
        /// Dynamic width of the control. 
        /// </summary>
        [Browsable(false)]
        public new int Width
        {
            get
            {
                return base.Width;
            }
        }

        /// <summary>
        /// Dynamic height of the control. 
        /// </summary>
        [Browsable(false)]
        public new int Height
        {
            get
            {
                return base.Height;
            }
        }

        /// <summary>
        /// The measured width based on the content. 
        /// </summary>
        public int DesiredWidth { get; private set; }

        /// <summary>
        /// The measured height based on the content. 
        /// </summary>
        public int DesiredHeight { get; private set; }

        /// <summary>
        /// Determines the text will be word wrapped. This is true, when the maximum size has been set. 
        /// </summary>
        public bool WordWrap { get; private set; }

        /// <summary>
        /// Constructor. 
        /// </summary>
        public AutoRichLabel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Overrides the width and height with the measured width and height
        /// </summary>
        public void Fit()
        {
            base.Width = this.DesiredWidth;
            base.Height = this.DesiredHeight;
        }

        /// <summary>
        /// Will be called when the rich text content of the control changes. 
        /// </summary>
        private void rtb_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            this.AutoSize = false; // Disable auto size, else it will break everything
            this.WordWrap = this.MaximumSize.Width > 0; // Enable word wrap when the maximum width has been set. 
            this.DesiredWidth = this.rtb.WordWrap ? this.MaximumSize.Width : e.NewRectangle.Width; // Measure width. 
            this.DesiredHeight = this.MaximumSize.Height > 0 && this.MaximumSize.Height < e.NewRectangle.Height ? this.MaximumSize.Height : e.NewRectangle.Height; // Measure height. 
            this.Fit(); // Override width and height. 
        }
    }
}
