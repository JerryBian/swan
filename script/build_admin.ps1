$WWWRootLoc = Join-Path $PSScriptRoot .. src admin wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules


npx sass $(Join-Path $WWWRootLoc scss style.scss) $(Join-Path $WWWRootLoc style.css)

uglifycss --ugly-comments --output $(Join-Path $WWWRootLoc style.min.css) `
    $(Join-Path $WWWRootLoc highlight atom-one-light.min.css) `
    $(Join-Path $NodeModulesLoc easymde dist easymde.min.css) `
    $(Join-Path $NodeModulesLoc sweetalert2 dist sweetalert2.css) `
    $(Join-Path $WWWRootLoc style.css)

uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js) `
    $(Join-Path $WWWRootLoc highlight highlight.min.js) `
    $(Join-Path $NodeModulesLoc "@fortawesome" fontawesome-free js solid.js) `
    $(Join-Path $NodeModulesLoc "@fortawesome" fontawesome-free js fontawesome.js) `
    $(Join-Path $NodeModulesLoc sweetalert2 dist sweetalert2.js) `
    $(Join-Path $NodeModulesLoc easymde dist easymde.min.js)