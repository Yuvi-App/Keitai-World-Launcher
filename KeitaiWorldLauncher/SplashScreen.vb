Public Class SplashScreen
    Private phrases As String() = {
        "Stacking the boxes perfectly...",
        "Ensuring all pies are cooked...",
        "Summoning final boss...",
        "Rolling for loot...",
        "Buffing your stats...",
        "Respawning the fun...",
        "Rendering more pixels...",
        "Collecting magic mushrooms...",
        "Reviving your save file...",
        "Training AI sidekick..."
    }

    Private rand As New Random()
    Private WithEvents tmrLoad As New Timer()

    Public Sub StartLoading()
        ProgressBar1.Value = 0
        lblMessage.Text = phrases(rand.Next(phrases.Length))
        tmrLoad.Interval = 100
        tmrLoad.Start()
    End Sub

    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StartLoading()
    End Sub

    Private Sub tmrLoad_Tick(sender As Object, e As EventArgs) Handles tmrLoad.Tick
        If ProgressBar1.Value < 100 Then
            ProgressBar1.Value += 2

            If ProgressBar1.Value Mod 20 = 0 Then
                lblMessage.Text = phrases(rand.Next(phrases.Length))
            End If
        Else
            tmrLoad.Stop()
            Me.Close()
        End If
    End Sub
End Class
