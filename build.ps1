$WWWRootLoc = Join-Path $PSScriptRoot src wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot node_modules

npm install
ncu -u
npm install

npx sass $(Join-Path $WWWRootLoc css style.scss) $(Join-Path $WWWRootLoc style.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc style.min.css) `
$(Join-Path $WWWRootLoc style.css)
Write-Output "CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) `
$(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js) `
$(Join-Path $WWWRootLoc js _shared.js) `
$(Join-Path $NodeModulesLoc easymde dist easymde.min.js) `
$(Join-Path $WWWRootLoc js script.js)
Write-Output "JS completed"

Write-Output "All Done!"