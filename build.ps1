$WWWRootLoc = Join-Path $PSScriptRoot src web wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot node_modules

npm install
ncu -u
npm install

Copy-Item -Path $(Join-Path $NodeModulesLoc bootstrap-icons font fonts *) `
    -Destination $(Join-Path $WWWRootLoc fonts) -Force
Write-Output "[all]: Fonts copied"

npx sass $(Join-Path $WWWRootLoc css style.scss) $(Join-Path $WWWRootLoc style.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc style.min.css) `
$(Join-Path $WWWRootLoc style.css)
Write-Output "[normal]: CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) `
$(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
$(Join-Path $NodeModulesLoc anchor-js anchor.js) `
$(Join-Path $NodeModulesLoc \@highlightjs cdn-assets highlight.js) `
$(Join-Path $WWWRootLoc js _shared.js)
Write-Output "[normal]: JS completed"

npx sass $(Join-Path $WWWRootLoc css admin.scss) $(Join-Path $WWWRootLoc admin.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc admin.min.css) `
$(Join-Path $NodeModulesLoc easymde dist easymde.min.css) `
$(Join-Path $WWWRootLoc admin.css)
Write-Output "[admin]: CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc admin.min.js) `
$(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
$(Join-Path $WWWRootLoc js _shared.js) `
$(Join-Path $NodeModulesLoc easymde dist easymde.min.js) `
$(Join-Path $WWWRootLoc js admin.js)
Write-Output "[admin]: JS completed"

Write-Output "All Done!"