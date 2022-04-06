$WWWRootLoc = Join-Path $PSScriptRoot .. src admin wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules
$LibRootLoc = Join-Path $PSScriptRoot .. lib

npx sass $(Join-Path $WWWRootLoc scss style.scss) $(Join-Path $WWWRootLoc dist style.css)

uglifycss --ugly-comments --output $(Join-Path $WWWRootLoc dist style.min.css) `
    $(Join-Path $LibRootLoc highlight atom-one-light.css) `
    $(Join-Path $NodeModulesLoc easymde dist easymde.min.css) `
    $(Join-Path $NodeModulesLoc sweetalert2 dist sweetalert2.css) `
    $(Join-Path $WWWRootLoc dist style.css)

uglifyjs --compress -o $(Join-Path $WWWRootLoc dist script.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js) `
    $(Join-Path $LibRootLoc highlight highlight.min.js) `
    $(Join-Path $NodeModulesLoc "@fortawesome" fontawesome-free js solid.js) `
    $(Join-Path $NodeModulesLoc "@fortawesome" fontawesome-free js fontawesome.js) `
    $(Join-Path $NodeModulesLoc sweetalert2 dist sweetalert2.js) `
    $(Join-Path $NodeModulesLoc easymde dist easymde.min.js) `
    $(Join-Path $NodeModulesLoc chart.js dist chart.js) `
    $(Join-Path $WWWRootLoc js script.js)