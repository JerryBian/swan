$WWWRootLoc = Join-Path $PSScriptRoot .. src admin wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules


npx sass $(Join-Path $WWWRootLoc scss style.scss) $(Join-Path $WWWRootLoc style.css)
uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc style.min.css) $(Join-Path $WWWRootLoc style.css)
uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js)