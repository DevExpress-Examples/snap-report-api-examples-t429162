﻿Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks

Namespace SnapServerExamples.CodeUtils
    Public MustInherit Class ExampleCodeEvaluator
        Protected MustOverride ReadOnly Property CodeStart() As String
        Protected MustOverride ReadOnly Property CodeBeforeClasses() As String
        Protected MustOverride ReadOnly Property CodeEnd() As String
        Protected MustOverride Function GetCodeDomProvider() As CodeDomProvider
        Protected MustOverride Function GetModuleAssembly() As String
        Protected MustOverride Function GetExampleClassName() As String

        Public Function ExecuteCodeAndGenerateDocument(ByVal args As CodeEvaluationEventArgs) As Boolean
            Dim theCode As String = String.Concat(CodeStart, args.Code, CodeBeforeClasses, args.CodeClasses, CodeEnd)
            Dim linesOfCode() As String = { theCode }
            Return CompileAndRun(linesOfCode, args.EvaluationParameter)
        End Function

        Protected Friend Function CompileAndRun(ByVal linesOfCode() As String, ByVal evaluationParameter As Object) As Boolean
            Dim CompilerParams As New CompilerParameters()

            CompilerParams.GenerateInMemory = True
            CompilerParams.TreatWarningsAsErrors = False
            CompilerParams.GenerateExecutable = False

            Dim referencesSystem() As String = { "System.dll", "System.Windows.Forms.dll", "System.Data.dll", "System.Xml.dll", "System.Core.dll", "System.Drawing.dll" }

            Dim referencesDX() As String = { AssemblyInfo.SRAssemblyData, GetModuleAssembly(), AssemblyInfo.SRAssemblyOfficeCore, AssemblyInfo.SRAssemblyPrintingCore, AssemblyInfo.SRAssemblyPrinting, AssemblyInfo.SRAssemblyDocs, AssemblyInfo.SRAssemblyUtils, AssemblyInfo.SRAssemblyRichEdit, AssemblyInfo.SRAssemblyRichEditCore, AssemblyInfo.SRAssemblySparklineCore }
            Dim references((referencesSystem.Length + referencesDX.Length) - 1) As String

            For referenceIndex As Integer = 0 To referencesSystem.Length - 1
                references(referenceIndex) = referencesSystem(referenceIndex)
            Next referenceIndex

            Dim i As Integer = 0
            Dim initial As Integer = referencesSystem.Length
            Do While i < referencesDX.Length
                Dim assembly As System.Reflection.Assembly = System.Reflection.Assembly.Load(referencesDX(i) & AssemblyInfo.FullAssemblyVersionExtension)
                If assembly IsNot Nothing Then
                    references(i + initial) = assembly.Location
                End If
                i += 1
            Loop
            CompilerParams.ReferencedAssemblies.AddRange(references)


            Dim provider As CodeDomProvider = GetCodeDomProvider()
            Dim compile As CompilerResults = provider.CompileAssemblyFromSource(CompilerParams, linesOfCode)

            If compile.Errors.HasErrors Then
                Dim text As String = "Compile error: "
                For Each ce As CompilerError In compile.Errors
                    text &= "rn" & ce.ToString()
                Next ce
                System.Windows.Forms.MessageBox.Show(text)
                Return False
            End If

            Dim [module] As System.Reflection.Module = Nothing
            Try
                [module] = compile.CompiledAssembly.GetModules()(0)
            Catch
            End Try
            Dim moduleType As Type = Nothing
            If [module] Is Nothing Then
                Return False
            End If
            moduleType = [module].GetType(GetExampleClassName())

            Dim methInfo As MethodInfo = Nothing
            If moduleType Is Nothing Then
                Return False
            End If
            methInfo = moduleType.GetMethod("Process")

            If methInfo IsNot Nothing Then
                Try
                    methInfo.Invoke(Nothing, New Object() { evaluationParameter })
                Catch e1 As Exception
                    Return False ' an error
                End Try
                Return True
            End If
            Return False
        End Function
    End Class

    Public MustInherit Class SnapExampleCodeEvaluator
        Inherits ExampleCodeEvaluator


        Protected Overrides Function GetModuleAssembly() As String
            Return AssemblyInfo.SRAssemblySnapCore
        End Function
        Protected Overrides Function GetExampleClassName() As String
            Return "SnapCodeResultViewer.ExampleItem"
        End Function
    End Class
    #Region "RichEditCSExampleCodeEvaluator"
    Public Class SnapCSExampleCodeEvaluator
        Inherits SnapExampleCodeEvaluator

        Protected Overrides Function GetCodeDomProvider() As CodeDomProvider
            Return New Microsoft.CSharp.CSharpCodeProvider()
        End Function

        Private Const codeStart_Renamed As String = "using System;" & ControlChars.CrLf & "using DevExpress.Data;" & ControlChars.CrLf & "using DevExpress.XtraPrinting;" & ControlChars.CrLf & "using DevExpress.XtraRichEdit;" & ControlChars.CrLf & "using DevExpress.XtraRichEdit.API.Native;" & ControlChars.CrLf & "using DevExpress.Snap;" & ControlChars.CrLf & "using DevExpress.Snap.Core.API;" & ControlChars.CrLf & "using DevExpress.Sparkline;" & ControlChars.CrLf & "using System.Drawing;" & ControlChars.CrLf & "using System.Windows.Forms;" & ControlChars.CrLf & "using DevExpress.Utils;" & ControlChars.CrLf & "using System.IO;" & ControlChars.CrLf & "using System.Diagnostics;" & ControlChars.CrLf & "using System.Xml;" & ControlChars.CrLf & "using System.Data;" & ControlChars.CrLf & "using System.Collections.Generic;" & ControlChars.CrLf & "using System.Linq;" & ControlChars.CrLf & "using System.Globalization;" & ControlChars.CrLf & "using SnapDocument = DevExpress.Snap.Core.API.SnapDocument;" & ControlChars.CrLf & "using SnapSparkline = DevExpress.Snap.Core.API.SnapSparkline;" & ControlChars.CrLf & "using TableRow = DevExpress.XtraRichEdit.API.Native.TableRow;" & ControlChars.CrLf & "using SnapList=DevExpress.Snap.Core.API.SnapList;" & ControlChars.CrLf & "namespace SnapCodeResultViewer { " & ControlChars.CrLf & "public class ExampleItem { " & ControlChars.CrLf & "        public static void Process(SnapDocumentServer server) { " & ControlChars.CrLf & ControlChars.CrLf


        Private Const codeBeforeClasses_Renamed As String = "       " & ControlChars.CrLf & " }" & ControlChars.CrLf & "    }" & ControlChars.CrLf


        Private Const codeEnd_Renamed As String = ControlChars.CrLf & "    }" & ControlChars.CrLf

        Protected Overrides ReadOnly Property CodeStart() As String
            Get
                Return codeStart_Renamed
            End Get
        End Property
        Protected Overrides ReadOnly Property CodeBeforeClasses() As String
            Get
                Return codeBeforeClasses_Renamed
            End Get
        End Property
        Protected Overrides ReadOnly Property CodeEnd() As String
            Get
                Return codeEnd_Renamed
            End Get
        End Property
    End Class
    #End Region
    #Region "RichEditVbExampleCodeEvaluator"
    Public Class SnapVbExampleCodeEvaluator
        Inherits SnapExampleCodeEvaluator

        Protected Overrides Function GetCodeDomProvider() As CodeDomProvider
            Return New Microsoft.VisualBasic.VBCodeProvider()
        End Function

        Private Const codeStart_Renamed As String = "Imports Microsoft.VisualBasic" & ControlChars.CrLf & "Imports System" & ControlChars.CrLf & "Imports DevExpress.Data" & ControlChars.CrLf & "Imports DevExpress.XtraRichEdit" & ControlChars.CrLf & "Imports DevExpress.XtraRichEdit.API.Native" & ControlChars.CrLf & "Imports DevExpress.Snap" & ControlChars.CrLf & "Imports DevExpress.Snap.Core.API" & ControlChars.CrLf & "Imports DevExpress.Sparkline" & ControlChars.CrLf & "Imports System.Drawing" & ControlChars.CrLf & "Imports System.Windows.Forms" & ControlChars.CrLf & "Imports DevExpress.Utils" & ControlChars.CrLf & "Imports System.IO" & ControlChars.CrLf & "Imports System.Diagnostics" & ControlChars.CrLf & "Imports System.Xml" & ControlChars.CrLf & "Imports System.Data" & ControlChars.CrLf & "Imports System.Collections.Generic" & ControlChars.CrLf & "Imports System.Linq" & ControlChars.CrLf & "Imports System.Globalization" & ControlChars.CrLf & "Imports SnapDocument = DevExpress.Snap.Core.API.SnapDocument" & ControlChars.CrLf & "Imports SnapSparkline = DevExpress.Snap.Core.API.SnapSparkline" & ControlChars.CrLf & "Imports TableRow = DevExpress.XtraRichEdit.API.Native.TableRow" & ControlChars.CrLf & "Imports SnapList = DevExpress.Snap.Core.API.SnapList" & "Namespace SnapCodeResultViewer" & ControlChars.CrLf & "	Public Class ExampleItem" & ControlChars.CrLf & "		Public Shared Sub Process(ByVal server As SnapDocumentServer)" & ControlChars.CrLf & ControlChars.CrLf


        Private Const codeBeforeClasses_Renamed As String = ControlChars.CrLf & "		End Sub" & ControlChars.CrLf & "	End Class" & ControlChars.CrLf


        Private Const codeEnd_Renamed As String = ControlChars.CrLf & "End Namespace" & ControlChars.CrLf

        Protected Overrides ReadOnly Property CodeStart() As String
            Get
                Return codeStart_Renamed
            End Get
        End Property
        Protected Overrides ReadOnly Property CodeBeforeClasses() As String
            Get
                Return codeBeforeClasses_Renamed
            End Get
        End Property
        Protected Overrides ReadOnly Property CodeEnd() As String
            Get
                Return codeEnd_Renamed
            End Get
        End Property
    End Class
    #End Region

    Public MustInherit Class ExampleEvaluatorByTimer
        Implements IDisposable

        Private leakSafeCompileEventRouter As LeakSafeCompileEventRouter
        Private compileExampleTimer As System.Windows.Forms.Timer
        Private compileComplete As Boolean = True
        Private Const CompileTimeIntervalInMilliseconds As Integer = 2000

        Public Sub New(ByVal enableTimer As Boolean)
            Me.leakSafeCompileEventRouter = New LeakSafeCompileEventRouter(Me)

            If enableTimer Then
                Me.compileExampleTimer = New System.Windows.Forms.Timer()
                Me.compileExampleTimer.Interval = CompileTimeIntervalInMilliseconds

                AddHandler compileExampleTimer.Tick, AddressOf leakSafeCompileEventRouter.OnCompileExampleTimerTick 'OnCompileTimerTick
                Me.compileExampleTimer.Enabled = True
            End If
        End Sub
        Public Sub New()
            Me.New(True)
        End Sub

        #Region "Events"
        Public Event QueryEvaluate As CodeEvaluationEventHandler

        Protected Friend Overridable Function RaiseQueryEvaluate() As CodeEvaluationEventArgs
            If QueryEvaluateEvent IsNot Nothing Then
                Dim args As New CodeEvaluationEventArgs()
                RaiseEvent QueryEvaluate(Me, args)
                Return args
            End If
            Return Nothing
        End Function
        Public Event OnBeforeCompile As EventHandler

        Private Sub RaiseOnBeforeCompile()
            RaiseEvent OnBeforeCompile(Me, New EventArgs())
        End Sub

        Public Event OnAfterCompile As OnAfterCompileEventHandler

        Private Sub RaiseOnAfterCompile(ByVal result As Boolean)
            RaiseEvent OnAfterCompile(Me, New OnAfterCompileEventArgs() With {.Result = result})
        End Sub
        #End Region

        Public Sub CompileExample(ByVal sender As Object, ByVal e As EventArgs)
            If Not compileComplete Then
                Return
            End If
            Dim args As CodeEvaluationEventArgs = RaiseQueryEvaluate()
            If Not args.Result Then
                Return
            End If

            ForceCompile(args)
        End Sub
        Public Sub ForceCompile(ByVal args As CodeEvaluationEventArgs)
            compileComplete = False
            If Not String.IsNullOrEmpty(args.Code) Then
                CompileExampleAndShowPrintPreview(args)
            End If

            compileComplete = True
        End Sub
        Private Sub CompileExampleAndShowPrintPreview(ByVal args As CodeEvaluationEventArgs)
            Dim evaluationSucceed As Boolean = False
            Try
                RaiseOnBeforeCompile()

                evaluationSucceed = Evaluate(args)
            Finally
                RaiseOnAfterCompile(evaluationSucceed)
            End Try
        End Sub

        Public Function Evaluate(ByVal args As CodeEvaluationEventArgs) As Boolean
            Dim snapExampleCodeEvaluator As ExampleCodeEvaluator = GetExampleCodeEvaluator(args.Language)
            Return snapExampleCodeEvaluator.ExecuteCodeAndGenerateDocument(args)
        End Function

        Protected MustOverride Function GetExampleCodeEvaluator(ByVal language As ExampleLanguage) As ExampleCodeEvaluator

        Public Sub Dispose() Implements IDisposable.Dispose
            If compileExampleTimer IsNot Nothing Then
                compileExampleTimer.Enabled = False
                If leakSafeCompileEventRouter IsNot Nothing Then
                    RemoveHandler compileExampleTimer.Tick, AddressOf leakSafeCompileEventRouter.OnCompileExampleTimerTick 'OnCompileTimerTick
                End If
                compileExampleTimer.Dispose()
                compileExampleTimer = Nothing
            End If
        End Sub
    End Class

    #Region "RichEditExampleEvaluatorByTimer"
    Public Class RichEditExampleEvaluatorByTimer
        Inherits ExampleEvaluatorByTimer

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Function GetExampleCodeEvaluator(ByVal language As ExampleLanguage) As ExampleCodeEvaluator
            If language = ExampleLanguage.VB Then
                Return New SnapVbExampleCodeEvaluator()
            End If
            Return New SnapCSExampleCodeEvaluator()
        End Function
    End Class
    #End Region

    #Region "LeakSafeCompileEventRouter"
    Public Class LeakSafeCompileEventRouter
        Private ReadOnly weakControlRef As WeakReference

        Public Sub New(ByVal [module] As ExampleEvaluatorByTimer)
            'Guard.ArgumentNotNull(module, "module");
            Me.weakControlRef = New WeakReference([module])
        End Sub
        Public Sub OnCompileExampleTimerTick(ByVal sender As Object, ByVal e As EventArgs)
            Dim [module] As ExampleEvaluatorByTimer = DirectCast(weakControlRef.Target, ExampleEvaluatorByTimer)
            If [module] IsNot Nothing Then
                [module].CompileExample(sender, e)
            End If
        End Sub
    End Class
    Public Class CodeEvaluationEventArgs
        Inherits EventArgs

        Public Property Result() As Boolean
        Public Property Code() As String
        Public Property CodeClasses() As String
        Public Property Language() As ExampleLanguage
        Public Property EvaluationParameter() As Object
    End Class
    Public Delegate Sub CodeEvaluationEventHandler(ByVal sender As Object, ByVal e As CodeEvaluationEventArgs)

    Public Class OnAfterCompileEventArgs
        Inherits EventArgs

        Public Property Result() As Boolean
    End Class
    Public Delegate Sub OnAfterCompileEventHandler(ByVal sender As Object, ByVal e As OnAfterCompileEventArgs)
    #End Region
End Namespace
