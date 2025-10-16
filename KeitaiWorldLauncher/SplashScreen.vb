Imports System.Threading
Imports System.Threading.Tasks
Imports ReaLTaiizor.Controls
Imports ReaLTaiizor.Enum.Poison
Imports ReaLTaiizor.Extension.Poison
Imports ReaLTaiizor.Forms

Public Class SplashScreen
    Inherits PoisonForm

    Private Shared splashThread As Thread
    Private Shared splashForm As SplashScreen

    Private loadingPhrases As String() = {
        "Spawning loot goblins...",
        "Loading next universe...",
        "Forging swords of destiny...",
        "Charging mana cores...",
        "Summoning epic bosses...",
        "Calibrating AI overlords...",
        "Synchronizing timelines...",
        "Rendering pixel perfection...",
        "Baking textures in the sun...",
        "Feeding dragons... carefully.",
        "Tuning the battle music...",
        "Leveling up loading screen...",
        "Polishing legendary armor...",
        "Animating idle animations...",
        "Adjusting gravity settings...",
        "Unscrambling potion recipes...",
        "Waking up ancient spirits...",
        "Spinning up the server hamsters...",
        "Sharpening digital blades...",
        "Checking if the cake is a lie...",
        "Unpacking magical scrolls...",
        "Sorting inventory by chaos...",
        "Slapping rubber ducks for luck...",
        "Applying coffee to devs...",
        "Scouting for side quests...",
        "Summoning a slightly better wizard...",
        "Rebooting multiverse core...",
        "Locating lost save files...",
        "Teaching NPCs to blink...",
        "Deploying cloud unicorns...",
        "Consulting the oracle...",
        "Rebinding keys to destiny...",
        "Disabling cheat codes...",
        "Polishing pixel dust...",
        "Rolling for initiative...",
        "Breeding mountable slimes..."
    }

    Private currentPhraseIndex As Integer = 0
    Private WithEvents phraseTimer As New System.Windows.Forms.Timer()
    Private WithEvents fadeInTimer As New System.Windows.Forms.Timer()
    Private lblBooting As PoisonLabel
    Private lblPhrase As PoisonLabel
    Private spinner As PoisonProgressSpinner

    ' ────────── Public Methods ──────────
    Public Shared Sub ShowSplash()
        If splashThread Is Nothing OrElse Not splashThread.IsAlive Then
            splashThread = New Thread(Sub()
                                          splashForm = New SplashScreen()
                                          Application.Run(splashForm)
                                      End Sub)
            splashThread.SetApartmentState(Threading.ApartmentState.STA)
            splashThread.Start()
        End If
    End Sub

    Public Shared Async Function CloseSplashAsync() As Task
        If splashForm IsNot Nothing AndAlso Not splashForm.IsDisposed AndAlso splashForm.IsHandleCreated Then
            Dim fadeOutTask As Task = Nothing
            splashForm.Invoke(Sub() fadeOutTask = splashForm.FadeOutAndCloseAsync())
            If fadeOutTask IsNot Nothing Then Await fadeOutTask
        End If
    End Function

    ' ────────── Setup ──────────
    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Light theme + teal accent
        Me.Style = ColorStyle.Silver
        Me.Theme = Theme.Light
        Me.TextAlign = FormTextAlignType.Center
        Me.ShadowType = FormShadowType.AeroShadow
        Me.Text = "Keitai World Launcher"
        Me.Movable = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Width = 520
        Me.Height = 320
        Me.Opacity = 0
        Me.ControlBox = False
        Me.AllowDrop = False


        ' Title
        lblBooting = New PoisonLabel() With {
            .Text = "Booting KWL...",
            .FontSize = PoisonLabelSize.Tall,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Top,
            .Height = 50,
            .Style = Me.Style,
            .Theme = Me.Theme
        }
        Me.Controls.Add(lblBooting)

        ' Spinner (centered horizontally)
        spinner = New PoisonProgressSpinner() With {
            .Style = Me.Style,
            .Theme = Me.Theme,
            .Spinning = True,
            .Maximum = 100,
            .Value = 10,
            .Size = New Drawing.Size(42, 42)
        }
        spinner.Location = New Drawing.Point((Me.ClientSize.Width - spinner.Width) \ 2, 180)
        Me.Controls.Add(spinner)

        ' Phrase label
        lblPhrase = New PoisonLabel() With {
            .Text = loadingPhrases(0),
            .FontSize = PoisonLabelSize.Medium,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Bottom,
            .Height = 50,
            .Style = Me.Style,
            .Theme = Me.Theme
        }
        Me.Controls.Add(lblPhrase)

        ' Keep spinner centered on resize
        AddHandler Me.Resize, Sub()
                                  spinner.Location = New Drawing.Point((Me.ClientSize.Width - spinner.Width) \ 2, 70)
                              End Sub

        ' Timers
        phraseTimer.Interval = 2000
        fadeInTimer.Interval = 50
        fadeInTimer.Start()
    End Sub

    ' ────────── Timers ──────────
    Private Sub fadeInTimer_Tick(sender As Object, e As EventArgs) Handles fadeInTimer.Tick
        If Me.Opacity < 1 Then
            Me.Opacity += 0.05
        Else
            fadeInTimer.Stop()
            phraseTimer.Start()
        End If
    End Sub

    Private Sub phraseTimer_Tick(sender As Object, e As EventArgs) Handles phraseTimer.Tick
        currentPhraseIndex = (currentPhraseIndex + 1) Mod loadingPhrases.Length
        lblPhrase.Text = loadingPhrases(currentPhraseIndex)
    End Sub

    ' ────────── Fade Out ──────────
    Public Async Function FadeOutAndCloseAsync() As Task
        While Me.Opacity > 0
            Me.Opacity -= 0.05
            Await Task.Delay(50)
        End While
        Me.Close()
    End Function
End Class
