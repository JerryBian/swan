$WWWRootLoc = Join-Path $PSScriptRoot .. src web wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules

Copy-Item -Path $(Join-Path $NodeModulesLoc bootstrap-icons font fonts *) `
    -Destination $(Join-Path $WWWRootLoc fonts)
Write-Output "[all]: Fonts copied"

npx sass $(Join-Path $WWWRootLoc css home.scss) $(Join-Path $WWWRootLoc home.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc home.min.css) `
    $(Join-Path $WWWRootLoc home.css)
Write-Output "[home]: CSS completed"

npx sass $(Join-Path $WWWRootLoc css style.scss) $(Join-Path $WWWRootLoc style.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc style.min.css) `
    $(Join-Path $NodeModulesLoc \@highlightjs cdn-assets styles atom-one-light.min.css) `
    $(Join-Path $WWWRootLoc style.css)
Write-Output "CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $NodeModulesLoc anchor-js anchor.js) `
    $(Join-Path $NodeModulesLoc \@highlightjs cdn-assets highlight.js) `
    $(Join-Path $WWWRootLoc js script.js)
Write-Output "JS completed"