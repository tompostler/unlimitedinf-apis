# Go to the place with the stuff
$srcrep = "$env:USERPROFILE\Source\Repos";
New-Item -ItemType Directory -Path $srcrep -Force
cd $srcrep

# List of the repos
Add-Type -Language CSharp @"
public class Repo
{
    public string Name;
    public string RepoUri;
    public string Gitusername;
    public string Gituseremail;
}
"@
function New-Repo {
    param
    (
        [string]$Name,
        [string]$RepoUri,
        [string]$Gitusername,
        [string]$Gituseremail
    )
    $repo = New-Object Repo
    $repo.Name = $Name
    $repo.RepoUri = $RepoUri
    $repo.Gitusername = $Gitusername
    $repo.Gituseremail = $Gituseremail
    return $repo
}
$grepos = @(
    (New-Repo 'unlimitedinf-apis' 'https://github.com/tompostler/unlimitedinf-apis.git' 'Tom Postler' 'tom@postler.me'),
    (New-Repo 'unlimitedinf-tools' 'https://github.com/tompostler/tools.git' 'Tom Postler' 'tom@postler.me')
)

# Need git > 2.15?
$env:GIT_REDIRECT_STDERR = '2>&1'

# git clone all the grepos
for ($i=0; $i -lt $grepos.length; $i++) {
    $grepo = $grepos[$i]
    git clone $grepo.RepoUri $grepo.Name
    Push-Location $grepo.Name
    git config --local user.name $grepo.Gitusername
    git config --local user.email $grepo.Gituseremail
    Pop-Location
}
