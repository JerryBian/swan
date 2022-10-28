$WWWRootLoc = Join-Path $PSScriptRoot .. src wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules

ncu -u
npm install

Copy-Item -Path $(Join-Path $NodeModulesLoc bootstrap-icons font fonts *) `
    -Destination $(Join-Path $WWWRootLoc fonts)
Write-Output "[all]: Fonts copied"

$jobs = @()

# home page
$jobs += Start-ThreadJob -Name home_css -ScriptBlock {
    npx sass $(Join-Path $using:WWWRootLoc css home.scss) $(Join-Path $using:WWWRootLoc home.css)
    uglifycss --ugly-comments `
        --output $(Join-Path $using:WWWRootLoc home.min.css) `
    $(Join-Path $using:WWWRootLoc home.css)
    Write-Output "[home]: CSS completed"
}

$jobs += Start-ThreadJob -Name home_js -ScriptBlock {
    uglifyjs --compress -o $(Join-Path $using:WWWRootLoc home.min.js) `
    $(Join-Path $using:NodeModulesLoc ga-lite dist ga-lite.js)
    Write-Output "[home]: JS completed"
}

# blog page
$jobs += Start-ThreadJob -Name blog_css -ScriptBlock {
    npx sass $(Join-Path $using:WWWRootLoc css blog.scss) $(Join-Path $using:WWWRootLoc blog.css)
    uglifycss --ugly-comments `
        --output $(Join-Path $using:WWWRootLoc blog.min.css) `
    $(Join-Path $using:NodeModulesLoc \@highlightjs cdn-assets styles atom-one-light.min.css) `
    $(Join-Path $using:WWWRootLoc blog.css)
    Write-Output "[blog]: CSS completed"
}

$jobs += Start-ThreadJob -Name blog_js -ScriptBlock {
    uglifyjs --compress -o $(Join-Path $using:WWWRootLoc blog.min.js) `
    $(Join-Path $using:NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $using:NodeModulesLoc anchor-js anchor.js) `
    $(Join-Path $using:NodeModulesLoc ga-lite dist ga-lite.js) `
    $(Join-Path $using:NodeModulesLoc \@highlightjs cdn-assets highlight.js) `
    $(Join-Path $using:WWWRootLoc js shared.js) `
    $(Join-Path $using:WWWRootLoc js blog.js)
    Write-Output "[blog]: JS completed"
}

# read page
$jobs += Start-ThreadJob -Name read_css -ScriptBlock {
    npx sass $(Join-Path $using:WWWRootLoc css read.scss) $(Join-Path $using:WWWRootLoc read.css)
    uglifycss --ugly-comments `
        --output $(Join-Path $using:WWWRootLoc read.min.css) `
    $(Join-Path $using:WWWRootLoc read.css)
    Write-Output "[read]: CSS completed"
}

$jobs += Start-ThreadJob -Name read_js -ScriptBlock {
    uglifyjs --compress -o $(Join-Path $using:WWWRootLoc read.min.js) `
    $(Join-Path $using:NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $using:NodeModulesLoc anchor-js anchor.js) `
    $(Join-Path $using:NodeModulesLoc ga-lite dist ga-lite.js) `
    $(Join-Path $using:WWWRootLoc js shared.js) `
    $(Join-Path $using:WWWRootLoc js read.js)
    Write-Output "[read]: JS completed"
}

# admin page
$jobs += Start-ThreadJob -Name admin_css -ScriptBlock {
    npx sass $(Join-Path $using:WWWRootLoc css admin.scss) $(Join-Path $using:WWWRootLoc admin.css)
    uglifycss --ugly-comments `
        --output $(Join-Path $using:WWWRootLoc admin.min.css) `
    $(Join-Path $using:NodeModulesLoc \@stackoverflow stacks dist css stacks.css) `
    $(Join-Path $using:NodeModulesLoc \@stackoverflow stacks-editor dist styles.css) `
    $(Join-Path $using:WWWRootLoc admin.css)
    Write-Output "[admin]: CSS completed"
}

$jobs += Start-ThreadJob -Name admin_js -ScriptBlock {
    uglifyjs --compress -o $(Join-Path $using:WWWRootLoc admin.min.js) `
    $(Join-Path $using:NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $using:NodeModulesLoc \@highlightjs cdn-assets highlight.js) `
    $(Join-Path $using:NodeModulesLoc \@stackoverflow stacks dist js stacks.js) `
    $(Join-Path $using:NodeModulesLoc \@stackoverflow stacks-editor dist app.bundle.js) `
    $(Join-Path $using:WWWRootLoc js shared.js) `
    $(Join-Path $using:WWWRootLoc js admin.js)
    Write-Output "[admin]: JS completed"
}

Wait-job -Job $jobs
foreach ($job in $jobs) {
    Receive-Job -Job $job
}

Write-Output "All Done!"