#Install-Module @("Microsoft.Graph.Users", "Microsoft.Graph.Mail", "Microsoft.Graph.Authentication") -Scope CurrentUser
#Update-Module @("Microsoft.Graph.Users", "Microsoft.Graph.Mail", "Microsoft.Graph.Authentication")
Connect-MgGraph -Scopes "Mail.ReadWrite, User.Read" -NoWelcome

$outFolder = "C:\Users\trich\OneDrive - St John Ambulance\VOR\" # Set this to the folder you want to save to.
$currentUser = Invoke-MgGraphRequest -Method GET https://graph.microsoft.com/v1.0/me

$inboxId = 'AAMkAGU0NTA2ODczLTYyZWEtNGZhNS04MDQ0LTIwMDFlMDQzN2M0ZQAuAAAAAAB20NTeTHozS4sqw68MW0ExAQBWg0hGD4wFSaUCnLIUVcwOAAAA-DKkAAA='
$vorFolderId = 'AAMkAGU0NTA2ODczLTYyZWEtNGZhNS04MDQ0LTIwMDFlMDQzN2M0ZQAuAAAAAAB20NTeTHozS4sqw68MW0ExAQD7P9wa3zxsSruC4g1eWMZTAATImwK9AAA='

$vorEmails = Get-MgUserMailFolderMessage -MailFolderId $inboxId -UserId $currentUser.id -filter "Subject eq 'Daily VOR Report'" -All

foreach ($mail in $vorEmails)
{
    $attachment =  Get-MgUserMessageAttachment -UserId $currentUser.id -MessageId $mail.id 

    $filename = $outFolder + $mail.ReceivedDateTime.ToString("yyyy-MM-dd") + " VOR Report.xls"

    $content = $attachment.AdditionalProperties.contentBytes

    $contentBytes = [Convert]::FromBase64String($content)

    [IO.File]::WriteAllBytes($filename, $contentBytes)

    Write-Host $filename

    Move-MgUserMailFolderMessage -UserId $currentUser.id -MailFolderId $inboxId -MessageId $mail.id -DestinationId $vorFolderId
}
