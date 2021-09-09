$WWWRootLoc = Join-Path $PSScriptRoot .. src jarvis wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules
$LibRootLoc = Join-Path $PSScriptRoot .. lib

npx sass $(Join-Path $WWWRootLoc scss style.scss) $(Join-Path $WWWRootLoc dist style.css)
uglifycss --ugly-comments --output $(Join-Path $WWWRootLoc dist style.min.css) $(Join-Path $LibRootLoc highlight atom-one-light.min.css) $(Join-Path $WWWRootLoc dist style.css)

uglifyjs --compress -o $(Join-Path $WWWRootLoc dist script.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js) `
    $(Join-Path $NodeModulesLoc anchor-js anchor.js) `
    $(Join-Path $NodeModulesLoc "@fortawesome" fontawesome-free js solid.js) `
    $(Join-Path $NodeModulesLoc "@fortawesome" fontawesome-free js fontawesome.js) `
    $(Join-Path $LibRootLoc highlight highlight.min.js) `
    $(Join-Path $WWWRootLoc js script.js)