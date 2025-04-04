Public Class SplashScreen
    Inherits Form

    Private Shared splashThread As Threading.Thread
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
    Private WithEvents phraseTimer As New Timer()
    Private WithEvents fadeInTimer As New Timer()
    Private lblPhrase As Label

    ' ----- Public methods for Form1 -----
    Public Shared Sub ShowSplash()
        If splashThread Is Nothing OrElse Not splashThread.IsAlive Then
            splashThread = New Threading.Thread(Sub()
                                                    splashForm = New SplashScreen()
                                                    Application.Run(splashForm)
                                                End Sub)
            splashThread.SetApartmentState(Threading.ApartmentState.STA)
            splashThread.Start()
        End If
    End Sub

    Public Shared Sub CloseSplash()
        If splashForm IsNot Nothing AndAlso splashForm.IsHandleCreated Then
            splashForm.Invoke(Sub()
                                  splashForm.FadeOutAndClose()
                              End Sub)
        End If
    End Sub

    ' ----- Form Setup -----
    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.White
        Me.Size = New Size(500, 200)
        Me.Opacity = 0

        Dim lblBooting As New Label With {
            .Text = "Booting KWL...",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.Black,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Top,
            .Height = 50
        }
        Me.Controls.Add(lblBooting)

        lblPhrase = New Label With {
            .Text = loadingPhrases(0),
            .Font = New Font("Segoe UI", 12, FontStyle.Italic),
            .ForeColor = Color.DimGray,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Fill
        }
        Me.Controls.Add(lblPhrase)

        Dim progress As New ProgressBar With {
            .Style = ProgressBarStyle.Marquee,
            .Dock = DockStyle.Bottom,
            .Height = 10,
            .MarqueeAnimationSpeed = 30
        }
        Me.Controls.Add(progress)

        ' Start fade-in
        phraseTimer.Interval = 2000
        fadeInTimer.Interval = 50
        fadeInTimer.Start()
    End Sub

    ' ----- Timers -----
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

    Private Sub FadeOutAndClose()
        Dim fadeOutTimer As New Timer With {.Interval = 50}
        AddHandler fadeOutTimer.Tick,
            Sub()
                If Me.Opacity > 0 Then
                    Me.Opacity -= 0.05
                Else
                    fadeOutTimer.Stop()
                    Me.Close()
                End If
            End Sub
        fadeOutTimer.Start()
    End Sub
End Class
