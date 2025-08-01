﻿' R- Instat
' Copyright (C) 2015-2017
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License 
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Imports instat.Translations
Public Class dlgSpells
    Private bFirstload As Boolean = True
    Private bReset As Boolean = True
    Private clsSpellLength, clsMaxSpellManipulation, clsIfElseFirstDoyFilledFunction, clsSubSpellLength1 As New RFunction
    Private clsMaxSpellSummary, clsDummyFunction, clsMaxValueList, clsMaxFunction, clsMaxSpellSubCalcs As New RFunction
    Private clsDayFilter, clsGroupBy, clsDayFilterCalcFromConvert, clsDayFilterCalcFromList As New RFunction
    Private clsDayFromAndToOperator, clsDayFromOperator, clsDayToOperator As New ROperator
    Private clsApplyInstatFunction, clsSpellLogicalCalc, clsSpellsLogicalCalc, clsSpellsLogCalcFunc, clsDotSpellsFunction As New RFunction
    Private clsSpellsFunction, clsSpellsManipulationsFunc, clsSpellManipulationsFunc, clsSpellFunction, clsRSpellSubFunct, clsRSpellFilterSubFunct, clsSpellFilterFunction As New RFunction
    Private clsCurrCalc As New RFunction
    Private clsRRaindayOperator, clsSpellLogicalAndOperator, clsSpellLogicalGreaterThanOperator, clsSpellLogicalLessThanOperator, clsAdditionalConditionReplaceOperator, clsAdditionalConditionReplaceOperator2, clsGreaterThanOperator, clsLessThanOperator As New ROperator
    Private clsAdditionalCondition, clsAdditionalConditionList, clsSubSpellLength2, clsAdditionalConditionReplaceFunction As New RFunction

    Private strCurrDataName As String = ""
    Private strSpellDay As String = "spell_day"

    Private strLessThan As String = "Less than or equal to"
    Private strGreaterThan As String = "Greater than or equal to"
    Private strBetween As String = "Between"
    Private strExcludingBetween As String = "Excluding between"
    Private bResetSubdialog As Boolean = False

    Private Sub dlgSpells_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If bFirstload Then
            InitialiseDialog()
            bFirstload = False
        End If
        If bReset Then
            SetDefaults()
        End If
        SetRCodeForControls(bReset)
        bReset = False
        TestOKEnabled()
        autoTranslate(Me)
    End Sub

    Private Sub InitialiseDialog()
        ucrBase.iHelpTopicID = 200

        ucrPnlOptions.AddRadioButton(rdoAnnuel)
        ucrPnlOptions.AddRadioButton(rdoSpells)
        ucrPnlOptions.AddRadioButton(rdoStation)
        ucrPnlOptions.AddParameterValuesCondition(rdoAnnuel, "type", Chr(34) & "summary" & Chr(34))
        ucrPnlOptions.AddParameterValuesCondition(rdoSpells, "type", Chr(34) & "filter" & Chr(34))

        ucrPnlOptions.AddToLinkedControls(ucrReceiverYear, {rdoSpells, rdoAnnuel}, bNewLinkedAddRemoveParameter:=True, bNewLinkedHideIfParameterMissing:=True)

        ucrSelectorForSpells.SetParameter(New RParameter("data_name", 0))
        ucrSelectorForSpells.SetParameterIsString()

        ' Setting receivers
        ucrReceiverElement.SetParameter(New RParameter("rain", 0))
        ucrReceiverElement.SetParameterIsString()
        ucrReceiverElement.bWithQuotes = False
        ucrReceiverElement.Selector = ucrSelectorForSpells
        ucrReceiverElement.SetDataType("numeric")
        ucrReceiverElement.SetClimaticType("rain")
        ucrReceiverElement.bAutoFill = True

        ucrReceiverStation.SetParameter(New RParameter("station", 1, False))
        ucrReceiverStation.SetParameterIsString()
        ucrReceiverStation.Selector = ucrSelectorForSpells
        ucrReceiverStation.SetClimaticType("station")
        ucrReceiverStation.bAutoFill = True

        ucrReceiverYear.Selector = ucrSelectorForSpells
        ucrReceiverYear.SetClimaticType("year")
        ucrReceiverYear.bAutoFill = True
        ucrReceiverYear.SetLinkedDisplayControl(lblYear)

        ucrReceiverDate.SetParameter(New RParameter("date", 0, False))
        ucrReceiverDate.SetParameterIsString()
        ucrReceiverDate.Selector = ucrSelectorForSpells
        ucrReceiverDate.SetDataType("Date")
        ucrReceiverDate.SetClimaticType("date")
        ucrReceiverDate.bAutoFill = True
        ucrReceiverDate.strSelectorHeading = "Date Variables"

        ucrReceiverDOY.SetParameter(New RParameter("day", 0))
        ucrReceiverDOY.SetParameterIsString()
        ucrReceiverDOY.bWithQuotes = False
        ucrReceiverDOY.Selector = ucrSelectorForSpells
        ucrReceiverDOY.SetDataType("numeric")
        ucrReceiverDOY.SetClimaticType("doy")
        ucrReceiverDOY.bAutoFill = True

        clsSubSpellLength1.SetRCommand("list")
        clsMaxValueList.SetRCommand("list")

        ucrInputSpellLower.SetParameter(New RParameter("min", 1))
        ucrInputSpellLower.SetValidationTypeAsNumeric()
        ucrInputSpellLower.AddQuotesIfUnrecognised = False

        ucrInputSpellUpper.SetParameter(New RParameter("max", 1))
        ucrInputSpellUpper.SetValidationTypeAsNumeric()
        ucrInputSpellUpper.AddQuotesIfUnrecognised = False
        ucrInputSpellUpper.SetLinkedDisplayControl(lblAnd)

        ucrInputCondition.SetItems({strLessThan, strGreaterThan, strBetween, strExcludingBetween})
        ucrInputCondition.SetDropDownStyleAsNonEditable()

        'ucrChkConditional.SetText("Assume condition not satisfied at start of each period")
        'ucrChkConditional.SetParameter(New RParameter("initial_value"))
        'ucrChkConditional.SetValuesCheckedAndUnchecked("0", "NA_real_")
        'ucrChkConditional.SetRDefault("NA_real_

        ucrChkDayRange.SetText("Day Range")
        ucrChkDayRange.AddParameterValuesCondition(True, "day", "True")
        ucrChkDayRange.AddParameterValuesCondition(False, "day", "False")

        ucrInputNewColumnName.SetParameter(New RParameter("result_name", 2))
        ucrInputNewColumnName.SetDataFrameSelector(ucrSelectorForSpells.ucrAvailableDataFrames)
        ucrInputNewColumnName.SetValidationTypeAsRVariable()
        ucrInputNewColumnName.SetName("spells")

        ucrInputCondition.AddToLinkedControls(ucrInputSpellLower, {strLessThan, strGreaterThan, strBetween, strExcludingBetween}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedChangeToDefaultState:=True, objNewDefaultState:=0)
        ucrInputCondition.AddToLinkedControls(ucrInputSpellUpper, {strBetween, strExcludingBetween}, bNewLinkedHideIfParameterMissing:=True, bNewLinkedChangeToDefaultState:=True, objNewDefaultState:=0.85)
        ucrInputFilterPreview.IsReadOnly = True
    End Sub

    Private Sub SetDefaults()
        clsSpellsManipulationsFunc = New RFunction
        clsSpellManipulationsFunc = New RFunction
        clsSpellFunction = New RFunction
        clsRSpellSubFunct = New RFunction
        clsRSpellFilterSubFunct = New RFunction
        clsSpellFilterFunction = New RFunction
        clsDotSpellsFunction = New RFunction
        clsIfElseFirstDoyFilledFunction = New RFunction
        clsDummyFunction = New RFunction
        Dim strSpellLogical As String = "spell_day"
        Dim strSpellName As String = "spell_length"

        clsApplyInstatFunction.Clear()
        clsMaxSpellManipulation.Clear()
        clsDayFilter.Clear()
        clsGroupBy.Clear()
        clsSpellsFunction.Clear()
        clsSpellLogicalCalc.Clear()
        clsSpellsLogicalCalc.Clear()
        clsSpellsLogCalcFunc.Clear()
        clsSpellLength.Clear()
        clsMaxSpellSummary.Clear()
        clsDayFromAndToOperator.Clear()
        clsDayFromOperator.Clear()
        clsDayToOperator.Clear()
        clsRRaindayOperator.Clear()
        clsSpellLogicalAndOperator.Clear()
        clsSpellLogicalGreaterThanOperator.Clear()
        clsSpellLogicalLessThanOperator.Clear()
        clsMaxFunction.Clear()
        clsAdditionalCondition.Clear()
        clsAdditionalConditionList.Clear()
        clsSubSpellLength2.Clear()
        clsAdditionalConditionReplaceFunction.Clear()
        clsAdditionalConditionReplaceOperator2.Clear()
        clsAdditionalConditionReplaceOperator.Clear()
        clsGreaterThanOperator.Clear()
        clsLessThanOperator.Clear()

        ucrSelectorForSpells.Reset()
        ucrReceiverElement.SetMeAsReceiver()
        ucrInputCondition.SetName(strBetween)
        ucrInputNewColumnName.SetName("spells")
        ucrInputSpellLower.SetName(0)
        ucrInputSpellUpper.SetName(0.85)

        clsDummyFunction.AddParameter("day", "False", iPosition:=0)

        ' key

        clsDayFilterCalcFromConvert = New RFunction
        clsDayFilterCalcFromConvert.SetPackageName("databook")
        clsDayFilterCalcFromConvert.SetRCommand("calc_from_convert")
        clsDayFilterCalcFromList = New RFunction
        clsDayFilterCalcFromList.SetRCommand("list")
        clsDayFilterCalcFromConvert.AddParameter("x", clsRFunctionParameter:=clsDayFilterCalcFromList, iPosition:=0)

        bResetSubdialog = True
        'DayFromandTo
        clsDayFromAndToOperator.bToScriptAsRString = True
        clsDayFilter.SetRCommand("instatCalculations::instat_calculation$new")
        clsDayFilter.AddParameter("type", Chr(34) & "filter" & Chr(34), iPosition:=0)
        clsDayFilter.AddParameter("function_exp", clsROperatorParameter:=clsDayFromAndToOperator, iPosition:=1)
        clsDayFromAndToOperator.SetOperation("&")
        clsDayFromAndToOperator.bBrackets = False
        clsDayFromAndToOperator.AddParameter("from", clsROperatorParameter:=clsDayFromOperator, iPosition:=0)
        clsDayFromOperator.SetOperation(">=")
        clsDayFromOperator.AddParameter("from", 1)
        clsDayFromAndToOperator.AddParameter("to", clsROperatorParameter:=clsDayToOperator, iPosition:=1)
        clsDayToOperator.SetOperation("<=")
        clsDayToOperator.AddParameter("to", 366)
        clsDayFilter.SetAssignTo("day_from_and_to")
        clsDayFilter.AddParameter("calculated_from", clsRFunctionParameter:=clsDayFilterCalcFromConvert, iPosition:=2)

        ' group
        clsGroupBy.SetRCommand("instatCalculations::instat_calculation$new")
        clsGroupBy.AddParameter("type", Chr(34) & "by" & Chr(34))
        clsGroupBy.SetAssignTo("grouping")

        ' rain_day
        clsSpellLogicalCalc.SetRCommand("instatCalculations::instat_calculation$new")
        clsSpellLogicalCalc.AddParameter("type", Chr(34) & "calculation" & Chr(34), iPosition:=0)
        clsSpellLogicalCalc.AddParameter("function_exp", clsROperatorParameter:=clsSpellLogicalAndOperator, iPosition:=1)
        clsSpellLogicalCalc.AddParameter("result_name", Chr(34) & strSpellLogical & Chr(34), iPosition:=2)
        clsSpellLogicalCalc.AddParameter("save", "0", iPosition:=6)
        clsSpellLogicalCalc.SetAssignTo(strSpellLogical)

        'clsSpellLogicalAndOperator.bToScriptAsRString = True
        clsSpellLogicalAndOperator.SetOperation("&")
        clsSpellLogicalGreaterThanOperator.SetOperation(">=")
        clsSpellLogicalGreaterThanOperator.AddParameter("min", 0, iPosition:=1)
        clsSpellLogicalLessThanOperator.SetOperation("<=")
        clsSpellLogicalLessThanOperator.AddParameter("max", 0.85, iPosition:=1)

        clsGreaterThanOperator.SetOperation(">")
        clsLessThanOperator.SetOperation("<")

        ' Spell Length
        clsSpellLength.SetRCommand("instatCalculations::instat_calculation$new")
        clsSpellLength.AddParameter("type", Chr(34) & "calculation" & Chr(34), iPosition:=0)
        clsSpellLength.AddParameter("result_name", Chr(34) & strSpellName & Chr(34), iPosition:=2)
        clsSpellLength.AddParameter("function_exp", clsRFunctionParameter:=clsSpellsFunction)
        clsSpellLength.AddParameter("sub_calculations", clsRFunctionParameter:=clsSubSpellLength1, iPosition:=5)
        clsSubSpellLength1.AddParameter("sub1", clsRFunctionParameter:=clsSpellLogicalCalc, bIncludeArgumentName:=False)
        clsSpellLength.AddParameter("save", 0, iPosition:=6)
        clsSpellLength.SetAssignTo("spell_length")

        clsSpellsManipulationsFunc.SetRCommand("list")
        clsSpellsManipulationsFunc.AddParameter("group_by_year", clsRFunctionParameter:=clsGroupBy, bIncludeArgumentName:=False, iPosition:=0)

        clsSpellsLogicalCalc.SetRCommand("instatCalculations::instat_calculation$new")
        clsSpellsLogicalCalc.AddParameter("type", Chr(34) & "calculation" & Chr(34), iPosition:=0)
        clsSpellsLogicalCalc.AddParameter("function_exp", clsROperatorParameter:=clsSpellLogicalAndOperator, iPosition:=1)
        clsSpellsLogicalCalc.AddParameter("result_name", Chr(34) & strSpellDay & Chr(34), iPosition:=2)
        clsSpellsLogicalCalc.SetAssignTo(strSpellDay)

        clsSpellManipulationsFunc.SetRCommand("list")
        clsSpellManipulationsFunc.AddParameter("grouping", clsRFunctionParameter:=clsGroupBy, bIncludeArgumentName:=False, iPosition:=0)

        clsDotSpellsFunction.bToScriptAsRString = True
        clsDotSpellsFunction.SetPackageName("instatClimatic")
        clsDotSpellsFunction.SetRCommand("spells")
        clsDotSpellsFunction.AddParameter("x", clsROperatorParameter:=clsSpellLogicalAndOperator, iPosition:=0)

        clsSpellFunction.SetRCommand("instatCalculations::instat_calculation$new")
        clsSpellFunction.AddParameter("type", Chr(34) & "calculation" & Chr(34), iPosition:=0)
        clsSpellFunction.AddParameter("function_exp", clsRFunctionParameter:=clsDotSpellsFunction, iPosition:=1) ' changes depending on the rdo
        clsSpellFunction.AddParameter("result_name", Chr(34) & "spell" & Chr(34), iPosition:=2)
        clsSpellFunction.AddParameter("manipulations", clsRFunctionParameter:=clsSpellManipulationsFunc, iPosition:=3)
        clsSpellFunction.SetAssignTo("spells_calculation")

        clsRSpellFilterSubFunct.SetRCommand("list")
        clsRSpellFilterSubFunct.AddParameter("sub1", clsRFunctionParameter:=clsSpellFunction, bIncludeArgumentName:=False, iPosition:=0)

        clsSpellFilterFunction.SetRCommand("instatCalculations::instat_calculation$new")
        clsSpellFilterFunction.AddParameter("type", Chr(34) & "filter" & Chr(34), iPosition:=0)
        clsSpellFilterFunction.AddParameter("function_exp", Chr(34) & "dplyr::lead(c(NA,diff(spell)))<0" & Chr(34), iPosition:=1) ' changes depending on the rdo
        clsSpellFilterFunction.AddParameter("sub_calculations", clsRFunctionParameter:=clsRSpellFilterSubFunct, iPosition:=2)
        clsSpellFilterFunction.AddParameter("save", 2, iPosition:=3)
        clsSpellFilterFunction.SetAssignTo("spells_filter")

        ' Additional Checkbox
        'clsAdditionalConditionReplaceFunction.bToScriptAsRString = True
        'clsAdditionalCondition.SetRCommand("instatCalculations::instat_calculation$new")
        'clsAdditionalCondition.AddParameter("type", Chr(34) & "calculation" & Chr(34), iPosition:=0)
        'clsAdditionalCondition.AddParameter("function_exp", clsRFunctionParameter:=clsAdditionalConditionReplaceFunction, iPosition:=1)
        'clsAdditionalCondition.AddParameter("result_name", Chr(34) & strSpellDay & Chr(34), iPosition:=2)
        'clsAdditionalCondition.AddParameter("save", 0, iPosition:=6)
        'clsAdditionalCondition.SetAssignTo("additional_condition")
        'clsAdditionalConditionReplaceFunction.SetRCommand("replace")
        'clsAdditionalConditionReplaceFunction.AddParameter("x", strSpellDay, iPosition:=0)
        'clsAdditionalConditionReplaceFunction.AddParameter("values", "0", iPosition:=2)
        'clsAdditionalConditionList.SetRCommand("list")
        'clsAdditionalConditionList.AddParameter("sub1", clsRFunctionParameter:=clsRRainday)
        'clsAdditionalCondition.AddParameter("sub_calculation", clsRFunctionParameter:=clsAdditionalConditionList)

        'Max Summary
        clsMaxSpellSummary.SetRCommand("instatCalculations::instat_calculation$new")
        clsMaxSpellSummary.AddParameter("type", Chr(34) & "summary" & Chr(34), iPosition:=0)
        clsMaxSpellSummary.AddParameter("function_exp", clsRFunctionParameter:=clsMaxFunction, iPosition:=1)
        clsMaxSpellSummary.AddParameter("save", 2, iPosition:=6)
        clsMaxSpellSummary.AddParameter("result_name", Chr(34) & ucrInputNewColumnName.GetText() & Chr(34), iPosition:=3)
        clsMaxSpellSummary.AddParameter("manipulations", clsRFunctionParameter:=clsMaxSpellManipulation, iPosition:=5)
        clsMaxSpellSummary.SetAssignTo("spells")

        clsMaxFunction.bToScriptAsRString = True
        clsMaxFunction.SetRCommand("max")
        clsMaxFunction.AddParameter("x", strSpellName)

        clsMaxSpellSubCalcs.SetRCommand("list")
        clsMaxSpellSubCalcs.AddParameter("sub1", clsRFunctionParameter:=clsSpellLength, bIncludeArgumentName:=False, iPosition:=0)

        clsMaxSpellManipulation.SetRCommand("list")
        clsMaxSpellManipulation.AddParameter("manip1", clsRFunctionParameter:=clsSpellLength, bIncludeArgumentName:=False, iPosition:=0)
        clsMaxSpellManipulation.AddParameter("manip2", clsRFunctionParameter:=clsGroupBy, bIncludeArgumentName:=False, iPosition:=1)

        clsSpellsFunction.bToScriptAsRString = True
        clsSpellsFunction.SetPackageName("instatClimatic")
        clsSpellsFunction.SetRCommand("spells")
        clsSpellsFunction.AddParameter("x", strSpellLogical)

        clsCurrCalc = clsMaxSpellSummary

        clsApplyInstatFunction.SetRCommand(frmMain.clsRLink.strInstatDataObject & "$run_instat_calculation")
        clsApplyInstatFunction.AddParameter("calc", clsRFunctionParameter:=clsMaxSpellSummary, iPosition:=0)
        clsApplyInstatFunction.AddParameter("display", "FALSE", iPosition:=1)
        AddDayRange()
        AddDateDoy()
        UpdateDateDoy()
        'Base Function
        ucrBase.clsRsyntax.SetBaseRFunction(clsApplyInstatFunction)
    End Sub

    Private Sub SetRCodeForControls(bReset As Boolean)
        ucrReceiverElement.AddAdditionalCodeParameterPair(clsSpellLogicalGreaterThanOperator, New RParameter("rain", 0), iAdditionalPairNo:=1)
        ucrReceiverElement.AddAdditionalCodeParameterPair(clsGreaterThanOperator, New RParameter("rain", 0), iAdditionalPairNo:=2)
        ucrReceiverElement.AddAdditionalCodeParameterPair(clsLessThanOperator, New RParameter("rain", 0), iAdditionalPairNo:=3)
        ucrInputSpellUpper.AddAdditionalCodeParameterPair(clsGreaterThanOperator, New RParameter("left", 1), iAdditionalPairNo:=1)
        ucrInputSpellLower.AddAdditionalCodeParameterPair(clsLessThanOperator, New RParameter("left", 1), iAdditionalPairNo:=1)
        ucrInputNewColumnName.AddAdditionalCodeParameterPair(clsSpellFilterFunction, New RParameter("result_data_frame"), iAdditionalPairNo:=1)

        ucrReceiverElement.SetRCode(clsSpellLogicalLessThanOperator, bReset)
        ucrInputSpellLower.SetRCode(clsSpellLogicalGreaterThanOperator, bReset)
        ucrInputSpellUpper.SetRCode(clsSpellLogicalLessThanOperator, bReset)
        ucrInputNewColumnName.SetRCode(clsMaxSpellSummary, bReset)
        If bReset Then
            ucrChkDayRange.SetRCode(clsDummyFunction, bReset)
            ucrPnlOptions.SetRCode(clsCurrCalc, bReset)
        End If
        AddDateDoy()
        UpdateDateDoy()
    End Sub

    Private Sub cmdDoyRange_Click(sender As Object, e As EventArgs) Handles cmdDoyRange.Click
        Dim bUseDate As Boolean = rdoStation.Checked
        sdgDoyRange.Setup(clsNewDoyFilterCalc:=clsDayFilter, clsNewIfElseFirstDoyFilledFunction:=clsIfElseFirstDoyFilledFunction, clsNewDayFromOperator:=clsDayFromOperator, clsNewDayToOperator:=clsDayToOperator, clsNewCalcFromList:=clsDayFilterCalcFromList, strNewMainDataFrame:=ucrSelectorForSpells.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strNewDoyColumn:=ucrReceiverDOY.GetVariableNames(False), bSetUseDateVisible:=True, bReset:=bResetSubdialog)
        sdgDoyRange.SetUseDateVisibility(bUseDate)
        If Not bUseDate Then
            sdgDoyRange.ucrChkUseDate.Checked = False
        End If
        sdgDoyRange.ShowDialog()
            UpdateDayFilterPreview()
        AddDayRange()
        AddDateDoy()
        UpdateDateDoy()
        bResetSubdialog = False
    End Sub

    Private Sub TestOKEnabled()
        If rdoSpells.Checked OrElse rdoAnnuel.Checked Then
            If Not ucrReceiverElement.IsEmpty AndAlso Not ucrInputNewColumnName.IsEmpty AndAlso Not ucrReceiverDate.IsEmpty AndAlso Not ucrReceiverDOY.IsEmpty AndAlso Not ucrReceiverYear.IsEmpty AndAlso ((ucrInputCondition.GetText = strBetween AndAlso Not ucrInputSpellLower.IsEmpty AndAlso Not ucrInputSpellUpper.IsEmpty) OrElse (ucrInputCondition.GetText = strExcludingBetween AndAlso Not ucrInputSpellLower.IsEmpty AndAlso Not ucrInputSpellUpper.IsEmpty) OrElse (ucrInputCondition.GetText = strLessThan AndAlso Not ucrInputSpellUpper.IsEmpty) OrElse (ucrInputCondition.GetText = strGreaterThan AndAlso Not ucrInputSpellUpper.IsEmpty)) Then
                ucrBase.OKEnabled(True)
            Else
                ucrBase.OKEnabled(False)
            End If
        Else
            If Not ucrReceiverElement.IsEmpty AndAlso Not ucrReceiverStation.IsEmpty AndAlso Not ucrInputNewColumnName.IsEmpty AndAlso Not ucrReceiverDate.IsEmpty AndAlso Not ucrReceiverDOY.IsEmpty AndAlso ((ucrInputCondition.GetText = strBetween AndAlso Not ucrInputSpellLower.IsEmpty AndAlso Not ucrInputSpellUpper.IsEmpty) OrElse (ucrInputCondition.GetText = strExcludingBetween AndAlso Not ucrInputSpellLower.IsEmpty AndAlso Not ucrInputSpellUpper.IsEmpty) OrElse (ucrInputCondition.GetText = strLessThan AndAlso Not ucrInputSpellUpper.IsEmpty) OrElse (ucrInputCondition.GetText = strGreaterThan AndAlso Not ucrInputSpellUpper.IsEmpty)) Then
                ucrBase.OKEnabled(True)
            Else
                ucrBase.OKEnabled(False)
            End If
        End If

    End Sub

    Private Sub ucrBase_ClickReset(sender As Object, e As EventArgs) Handles ucrBase.ClickReset
        SetDefaults()
        SetRCodeForControls(True)
        TestOKEnabled()
    End Sub

    Private Sub InputConditionOptions()
        'Set here as they are > and < for "Excluding between"
        clsSpellLogicalGreaterThanOperator.SetOperation(">=")
        clsSpellLogicalLessThanOperator.SetOperation("<=")
        Select Case ucrInputCondition.GetText
            Case strLessThan
                clsSpellLogicalAndOperator.RemoveParameterByName("lower")
                clsSpellLogicalGreaterThanOperator.RemoveParameterByName("min")
                clsSpellLogicalAndOperator.AddParameter("upper", clsROperatorParameter:=clsSpellLogicalLessThanOperator, iPosition:=0)
                clsSpellLogicalLessThanOperator.AddParameter("max", ucrInputSpellLower.GetText, iPosition:=1)
                clsRRaindayOperator.AddParameter("x", clsROperatorParameter:=clsSpellLogicalLessThanOperator, iPosition:=0)
            Case strGreaterThan
                clsSpellLogicalAndOperator.RemoveParameterByName("upper")
                clsSpellLogicalLessThanOperator.RemoveParameterByName("max")
                clsSpellLogicalAndOperator.AddParameter("lower", clsROperatorParameter:=clsSpellLogicalGreaterThanOperator, iPosition:=0)
                clsSpellLogicalGreaterThanOperator.AddParameter("min", ucrInputSpellLower.GetText, iPosition:=1)
                clsRRaindayOperator.AddParameter("x", clsROperatorParameter:=clsSpellLogicalGreaterThanOperator, iPosition:=0)
            Case strBetween
                clsSpellLogicalAndOperator.SetOperation("&")
                clsSpellLogicalAndOperator.AddParameter("lower", clsROperatorParameter:=clsSpellLogicalGreaterThanOperator, iPosition:=0)
                clsSpellLogicalGreaterThanOperator.AddParameter("min", ucrInputSpellLower.GetText, iPosition:=1)
                clsSpellLogicalAndOperator.AddParameter("upper", clsROperatorParameter:=clsSpellLogicalLessThanOperator, iPosition:=1)
                clsSpellLogicalLessThanOperator.AddParameter("max", ucrInputSpellUpper.GetText, iPosition:=1)
                clsRRaindayOperator.AddParameter("x", clsROperatorParameter:=clsSpellLogicalAndOperator, iPosition:=0)
            Case strExcludingBetween
                clsSpellLogicalAndOperator.SetOperation("|")
                clsSpellLogicalAndOperator.AddParameter("lower", clsROperatorParameter:=clsSpellLogicalGreaterThanOperator, iPosition:=1)
                clsSpellLogicalGreaterThanOperator.SetOperation(">")
                clsSpellLogicalGreaterThanOperator.AddParameter("min", ucrInputSpellUpper.GetText, iPosition:=1)
                clsSpellLogicalAndOperator.AddParameter("upper", clsROperatorParameter:=clsSpellLogicalLessThanOperator, iPosition:=0)
                clsSpellLogicalLessThanOperator.SetOperation("<")
                clsSpellLogicalLessThanOperator.AddParameter("max", ucrInputSpellLower.GetText, iPosition:=1)
                clsRRaindayOperator.AddParameter("x", clsROperatorParameter:=clsSpellLogicalAndOperator, iPosition:=0)
        End Select
    End Sub

    Private Sub ucrInputSpellLower_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrInputSpellLower.ControlValueChanged, ucrInputSpellUpper.ControlValueChanged, ucrInputCondition.ControlValueChanged
        InputConditionOptions()
    End Sub

    Private Sub RainDays()
        clsSpellLogicalCalc.AddParameter("calculated_from", " list(" & strCurrDataName & "=" & ucrReceiverElement.GetVariableNames() & ")", iPosition:=0)
        clsSpellsLogCalcFunc.AddParameter("calculated_from", " list(" & strCurrDataName & "=" & ucrReceiverElement.GetVariableNames() & ")", iPosition:=3)
        clsSpellFunction.AddParameter("calculated_from", " list(" & strCurrDataName & "=" & ucrReceiverElement.GetVariableNames() & ")", iPosition:=3)
    End Sub

    Private Sub GroupByOptions()
        If rdoAnnuel.Checked OrElse rdoSpells.Checked Then
            If Not ucrReceiverStation.IsEmpty Then
                If Not ucrReceiverYear.IsEmpty Then
                    clsGroupBy.AddParameter("calculated_from", "list(" & strCurrDataName & "=" & ucrReceiverYear.GetVariableNames & "," & strCurrDataName & "=" & ucrReceiverStation.GetVariableNames & ")")
                Else
                    clsGroupBy.AddParameter("calculated_from", "list(" & strCurrDataName & "=" & ucrReceiverStation.GetVariableNames & ")", iPosition:=1)
                End If
            ElseIf Not ucrReceiverYear.IsEmpty Then
                clsGroupBy.AddParameter("calculated_from", "list(" & strCurrDataName & "=" & ucrReceiverYear.GetVariableNames & ")")
            End If
        End If

    End Sub

    Private Sub ucrReceiverStation_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverDOY.ControlValueChanged, ucrReceiverDOY.ControlValueChanged
        UpdateDayFilterPreview()
        AddDateDoy()
        UpdateDateDoy()
    End Sub

    'Private Sub ucrChkConditional_ControlValueChanged(ucrChangedControl As ucrCore)
    '    If ucrChkConditional.Checked Then
    '        clsMaxSpellSummary.AddParameter("sub_calculation", clsRFunctionParameter:=clsMaxSpellSubCalcs, iPosition:=5)
    '        clsMaxSpellManipulation.RemoveParameterByName("manip1")
    '    Else
    '        clsMaxSpellSummary.RemoveParameterByName("sub_calculation")
    '        clsMaxSpellManipulation.AddParameter("manip1", clsRFunctionParameter:=clsSpellLength, bIncludeArgumentName:=False, iPosition:=0)
    '    End If
    'End Sub

    Private Sub ucrSelectorForSpells_ControlContentsChanged(ucrchangedControl As ucrCore) Handles ucrSelectorForSpells.ControlContentsChanged
        strCurrDataName = Chr(34) & ucrSelectorForSpells.ucrAvailableDataFrames.cboAvailableDataFrames.SelectedItem & Chr(34)
        RainDays()
        GroupByOptions()
    End Sub

    Private Sub ucrReceiverYear_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverYear.ControlValueChanged, ucrReceiverStation.ControlValueChanged
        GroupByOptions()
    End Sub

    Private Sub ucrPnlOptions_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrPnlOptions.ControlValueChanged
        ResetUseDateIfNotStation()
        GroupByOptions()
        TestOKEnabled()

        If rdoAnnuel.Checked OrElse rdoStation.Checked Then
            clsCurrCalc = clsMaxSpellSummary
            clsSpellLogicalAndOperator.bToScriptAsRString = True
            clsApplyInstatFunction.AddParameter("calc", clsRFunctionParameter:=clsMaxSpellSummary, iPosition:=0)
        Else
            clsSpellLogicalAndOperator.bToScriptAsRString = False
            clsCurrCalc = clsSpellFilterFunction
            clsApplyInstatFunction.AddParameter("calc", clsRFunctionParameter:=clsSpellFilterFunction, iPosition:=0)
        End If
        AddDateDoy()
        UpdateDateDoy()
    End Sub

    Private Sub ucrReceiverElement_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverElement.ControlValueChanged
        RainDays()
    End Sub

    Private Sub ucrSelectorVariable_DataFrameChanged() Handles ucrSelectorForSpells.DataFrameChanged
        clsDayFilterCalcFromList.ClearParameters()
    End Sub

    Private Sub CoreControls_ControlContentsChanged(ucrChangedControl As ucrCore) Handles ucrReceiverElement.ControlContentsChanged, ucrReceiverYear.ControlContentsChanged, ucrReceiverDOY.ControlContentsChanged, ucrReceiverDate.ControlContentsChanged, ucrInputNewColumnName.ControlContentsChanged, ucrInputCondition.ControlContentsChanged, ucrInputSpellLower.ControlContentsChanged, ucrInputSpellUpper.ControlContentsChanged, ucrReceiverStation.ControlContentsChanged
        TestOKEnabled()
    End Sub

    Public Sub AddDateDoy()
        If rdoStation.Checked AndAlso sdgDoyRange.UseDateChecked Then
            clsDayFromOperator.AddParameter("date", ucrReceiverDate.GetVariableNames(False), iPosition:=0)
            clsDayToOperator.AddParameter("date", ucrReceiverDate.GetVariableNames(False), iPosition:=0)
            clsDayFromOperator.RemoveParameterByName("doy")
            clsDayToOperator.RemoveParameterByName("doy")
        Else
            clsDayFromOperator.AddParameter("doy", ucrReceiverDOY.GetVariableNames(False), iPosition:=0)
            clsDayToOperator.AddParameter("doy", ucrReceiverDOY.GetVariableNames(False), iPosition:=0)
            clsDayFromOperator.RemoveParameterByName("date")
            clsDayToOperator.RemoveParameterByName("date")
        End If
    End Sub

    Private Sub AddDayRange()
        If ucrChkDayRange.Checked Then
            cmdDoyRange.Enabled = True
            ucrInputFilterPreview.Visible = True
            clsMaxSpellManipulation.AddParameter("manip3", clsRFunctionParameter:=clsDayFilter, bIncludeArgumentName:=False, iPosition:=2)
        Else
            cmdDoyRange.Enabled = False
            ucrInputFilterPreview.Visible = False
            clsMaxSpellManipulation.RemoveParameterByName("manip3")
        End If
    End Sub

    Private Sub ucrChkDayRange_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrChkDayRange.ControlValueChanged
        AddDayRange()
        UpdateDayFilterPreview()
    End Sub

    Private Sub ucrReceiverDate_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrReceiverDate.ControlValueChanged
        AddDateDoy()
        UpdateDateDoy()
        UpdateDayFilterPreview()
    End Sub

    Private bIsUpdatingPreview As Boolean = False

    Private Sub ucrInputFilterPreview_ControlValueChanged(ucrChangedControl As ucrCore) Handles ucrInputFilterPreview.ControlValueChanged
        If bIsUpdatingPreview Then Exit Sub
        bIsUpdatingPreview = True
        UpdateDayFilterPreview()
        bIsUpdatingPreview = False
    End Sub

    Private Sub UpdateDayFilterPreview()
        AddDateDoy() ' update first
        UpdateDateDoy()
        Dim strPreviewName As String
        If Not ucrReceiverDate.IsEmpty AndAlso sdgDoyRange.UseDateChecked Then
            strPreviewName = clsDayFromAndToOperator.ToScript()
        ElseIf Not ucrReceiverDOY.IsEmpty Then
            strPreviewName = clsDayFromAndToOperator.ToScript()
        Else
            strPreviewName = ""
        End If

        If ucrInputFilterPreview.GetText() <> strPreviewName Then
            ucrInputFilterPreview.SetName(strPreviewName)
        End If
    End Sub

    Private Sub ResetUseDateIfNotStation()
        If Not rdoStation.Checked Then
            sdgDoyRange.ResetUseDate()
            AddDateDoy()
            UpdateDateDoy()
            UpdateDayFilterPreview()
        End If
    End Sub

    Private Sub UpdateDateDoy()
        If rdoStation.Checked AndAlso sdgDoyRange.ucrChkUseDate.Checked Then
            If Not ucrReceiverDate.IsEmpty Then
                clsDayFilterCalcFromList.AddParameter(ucrSelectorForSpells.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strParameterValue:=ucrReceiverDate.GetVariableNames(), iPosition:=0)
            Else
                clsDayFilterCalcFromList.RemoveParameterByName(ucrSelectorForSpells.ucrAvailableDataFrames.cboAvailableDataFrames.Text)
            End If
        Else
            If Not ucrReceiverDOY.IsEmpty Then
                clsDayFilterCalcFromList.AddParameter(ucrSelectorForSpells.ucrAvailableDataFrames.cboAvailableDataFrames.Text, strParameterValue:=ucrReceiverDOY.GetVariableNames(), iPosition:=0)
            Else
                clsDayFilterCalcFromList.RemoveParameterByName(ucrSelectorForSpells.ucrAvailableDataFrames.cboAvailableDataFrames.Text)
            End If
        End If
    End Sub
End Class