$WWWRootLoc = Join-Path $PSScriptRoot .. src admin wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules


npx sass $(Join-Path $WWWRootLoc scss index.scss) $(Join-Path $WWWRootLoc index.css)
uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc index.min.css) $(Join-Path $WWWRootLoc index.css)
uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js)

# Error page
npx sass $(Join-Path $WWWRootLoc scss error.scss) $(Join-Path $WWWRootLoc error.css)