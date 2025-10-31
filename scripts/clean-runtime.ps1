param(
    [string]$PublishDir = "publish"
)

Get-ChildItem $PublishDir -Recurse | Where-Object {
    (
        ($_.Name -like "System.*.dll") -or
        ($_.Extension -eq ".so")
    ) -and
    ($_.Name -notlike "System.Diagnostics.*.dll") -and
    ($_.Name -notlike "System.IO.*.dll") -and
    ($_.Name -notlike "System.Text.*.dll")
} | Remove-Item -Force
