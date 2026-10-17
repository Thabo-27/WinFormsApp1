Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Media
Imports System.Windows.Forms

Public Class Form1

    ' Store messages using a generic collection.
    Private memory As New List(Of String)

    ' Delegate for selecting the chatbot response method.
    Private Delegate Function ChatResponse(message As String) As String
    Private responseHandler As ChatResponse

    Private greetingPlayer As SoundPlayer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Text = "ThaboCybersecurity Chatbot"

        ' Set up the conversation box.
        ChatInput.ReadOnly = True
        ChatInput.AppendText("ThaboCybersecurity: Hello! Welcome to my Cybersecurity Awareness Chatbot." & vbCrLf)
        ChatInput.AppendText("Ask me about passwords, phishing, malware, scams and online safety." & vbCrLf & vbCrLf)

        responseHandler = AddressOf GenerateResponse

        DisplayLogo()
        PlayGreeting()

    End Sub

    Private Sub DisplayLogo()

        ' IMAGE FILE: "images/logo.png"
        Dim logoPath As String = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "images",
            "logo.png")

        If File.Exists(logoPath) Then
            Try
                Using originalImage As Image = Image.FromFile(logoPath)
                    If PictureBox1.Image IsNot Nothing Then
                        PictureBox1.Image.Dispose()
                    End If

                    PictureBox1.Image = New Bitmap(originalImage)
                    PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
                End Using
            Catch ex As Exception
                ' The chatbot can still work if the logo cannot load.
            End Try
        End If

    End Sub

    Private Sub PlayGreeting()

        ' AUDIO FILE: "voice/Prog Recording.wav"
        Dim audioPath As String = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "voice",
            "Prog Recording.wav")

        If File.Exists(audioPath) Then
            Try
                greetingPlayer = New SoundPlayer(audioPath)
                greetingPlayer.Play()
            Catch ex As Exception
                ' The chatbot can still work without the recording.
            End Try
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SendMessage()
    End Sub

    Private Sub UserInput_KeyDown(sender As Object, e As KeyEventArgs) Handles UserInput.KeyDown

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            SendMessage()
        End If

    End Sub

    Private Sub SendMessage()

        Dim message As String = UserInput.Text.Trim()

        If message = "" Then
            MessageBox.Show("Please type a message first.")
            Return
        End If

        ChatInput.AppendText("You: " & message & vbCrLf)

        ' Save the user's message in conversation memory.
        memory.Add(message)

        Dim answer As String = responseHandler(message)

        ChatInput.AppendText("ThaboCybersecurity: " & answer & vbCrLf & vbCrLf)

        UserInput.Clear()
        UserInput.Focus()

        ChatInput.SelectionStart = ChatInput.Text.Length
        ChatInput.ScrollToCaret()

    End Sub

    Private Function GenerateResponse(message As String) As String

        Dim text As String = message.ToLowerInvariant()
        Dim answer As String = ""

        ' Detect sentiment and respond supportively.
        If ContainsAny(text, "sad", "upset", "worried",
                       "scared", "afraid", "frustrated") Then

            answer = "I understand that this may be worrying. " &
                     "Let's work through the issue step by step."

        ElseIf ContainsAny(text, "happy", "excited", "great",
                           "thank you", "thanks") Then

            answer = "I'm glad to hear that! Keep learning how to stay safe online."

        End If

        ' Recognise greetings.
        If text = "hi" OrElse text = "hello" OrElse text = "hey" Then
            Return "Hello! How can I help you stay safe online today?"
        End If

        ' Recall a previous message.
        If ContainsAny(text, "what did i tell you",
                       "remember my message", "what do you remember") Then

            If memory.Count > 1 Then
                Return "You previously told me: """ &
                       memory(memory.Count - 2) & """"
            Else
                Return "We have only just started chatting. Tell me something and I will try to remember it during this session."
            End If

        End If

        ' Cybersecurity keyword recognition.
        If ContainsAny(text, "password", "passphrase") Then

            answer &= " Use a long, unique password for every account. " &
                      "Consider a password manager and enable multi-factor authentication."

        ElseIf ContainsAny(text, "phishing", "fake email",
                           "suspicious link", "suspicious email") Then

            answer &= " Phishing is an attempt to trick you into revealing information or clicking harmful links. " &
                      "Check the sender and verify suspicious requests before responding."

        ElseIf ContainsAny(text, "malware", "computer virus", "trojan") Then

            answer &= " Malware is harmful software that can damage a device or steal information. " &
                      "Keep software updated and avoid unknown downloads."

        ElseIf ContainsAny(text, "ransomware") Then

            answer &= " Ransomware can encrypt files and demand payment. " &
                      "Keep secure backups, update your devices and report incidents."

        ElseIf ContainsAny(text, "scam", "fraud", "online scam") Then

            answer &= " Watch out for urgent payment requests, fake prizes and messages asking for personal details. " &
                      "Verify the source independently."

        ElseIf ContainsAny(text, "2fa", "multi-factor",
                           "two-factor", "authentication") Then

            answer &= " Multi-factor authentication adds an extra verification step to your account. " &
                      "Use it wherever possible."

        ElseIf ContainsAny(text, "privacy", "personal information",
                           "identity theft") Then

            answer &= " Protect your ID number, banking details and passwords. " &
                      "Only share personal information with trusted services when necessary."

        ElseIf ContainsAny(text, "wifi", "wi-fi", "public network") Then

            answer &= " Use trusted Wi-Fi networks, keep your router updated and be careful when accessing sensitive accounts on public networks."

        ElseIf ContainsAny(text, "social media", "facebook",
                           "instagram", "tiktok") Then

            answer &= " Use privacy settings, strong passwords and multi-factor authentication. " &
                      "Think carefully before sharing personal information."

        ElseIf ContainsAny(text, "help", "topics",
                           "what can you do") Then

            answer &= " I can help with passwords, phishing, malware, ransomware, scams, privacy, Wi-Fi security and social media safety."

        ElseIf answer = "" Then

            answer = "I do not recognise that topic yet. Try asking about passwords, phishing, malware, ransomware, scams or online privacy."

        End If

        Return answer

    End Function

    Private Function ContainsAny(text As String,
                                 ParamArray keywords() As String) As Boolean

        For Each keyword As String In keywords
            If text.Contains(keyword) Then
                Return True
            End If
        Next

        Return False

    End Function

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        If greetingPlayer IsNot Nothing Then
            greetingPlayer.Stop()
            greetingPlayer.Dispose()
        End If

        If PictureBox1.Image IsNot Nothing Then
            PictureBox1.Image.Dispose()
        End If

        MyBase.OnFormClosed(e)

    End Sub

End Class
