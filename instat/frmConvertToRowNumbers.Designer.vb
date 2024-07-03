<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConvertToRowNumbers
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblConvertToRowNumbers = New System.Windows.Forms.Label()
        Me.cmdInspectValues = New System.Windows.Forms.Button()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.cmdHelp = New System.Windows.Forms.Button()
        Me.cmdNormalConvert = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblConvertToRowNumbers
        '
        Me.lblConvertToRowNumbers.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblConvertToRowNumbers.ForeColor = System.Drawing.Color.Red
        Me.lblConvertToRowNumbers.Location = New System.Drawing.Point(9, 5)
        Me.lblConvertToRowNumbers.Name = "lblConvertToRowNumbers"
        Me.lblConvertToRowNumbers.Size = New System.Drawing.Size(497, 96)
        Me.lblConvertToRowNumbers.TabIndex = 5
        Me.lblConvertToRowNumbers.Text = "We are sorry, but there are some operations we don't allow when your dataframe in" &
    "cludes rownames.  We explain in detail in the Help.  "
        '
        'cmdInspectValues
        '
        Me.cmdInspectValues.Location = New System.Drawing.Point(12, 110)
        Me.cmdInspectValues.Name = "cmdInspectValues"
        Me.cmdInspectValues.Size = New System.Drawing.Size(123, 23)
        Me.cmdInspectValues.TabIndex = 10
        Me.cmdInspectValues.Text = "Row Numbers/Names "
        Me.cmdInspectValues.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(261, 110)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(123, 23)
        Me.cmdCancel.TabIndex = 13
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'cmdHelp
        '
        Me.cmdHelp.Location = New System.Drawing.Point(384, 110)
        Me.cmdHelp.Name = "cmdHelp"
        Me.cmdHelp.Size = New System.Drawing.Size(123, 23)
        Me.cmdHelp.TabIndex = 12
        Me.cmdHelp.Text = "Help"
        Me.cmdHelp.UseVisualStyleBackColor = True
        '
        'cmdNormalConvert
        '
        Me.cmdNormalConvert.Location = New System.Drawing.Point(135, 110)
        Me.cmdNormalConvert.Name = "cmdNormalConvert"
        Me.cmdNormalConvert.Size = New System.Drawing.Size(126, 23)
        Me.cmdNormalConvert.TabIndex = 11
        Me.cmdNormalConvert.Text = "Reset to Row Numbers "
        Me.cmdNormalConvert.UseVisualStyleBackColor = True
        '
        'frmConvertToRowNumbers
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(511, 142)
        Me.Controls.Add(Me.cmdInspectValues)
        Me.Controls.Add(Me.cmdCancel)
        Me.Controls.Add(Me.cmdHelp)
        Me.Controls.Add(Me.cmdNormalConvert)
        Me.Controls.Add(Me.lblConvertToRowNumbers)
        Me.Name = "frmConvertToRowNumbers"
        Me.Text = "Convert To Row Numbers"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lblConvertToRowNumbers As Label
    Friend WithEvents cmdInspectValues As Button
    Friend WithEvents cmdCancel As Button
    Friend WithEvents cmdHelp As Button
    Friend WithEvents cmdNormalConvert As Button
End Class
