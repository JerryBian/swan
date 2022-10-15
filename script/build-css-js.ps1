$WWWRootLoc = Join-Path $PSScriptRoot .. src wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules

ncu -u
npm install

Copy-Item -Path $(Join-Path $NodeModulesLoc bootstrap-icons font fonts *) `
    -Destination $(Join-Path $WWWRootLoc fonts)
Write-Output "[all]: Fonts copied"

npx sass $(Join-Path $WWWRootLoc css home.scss) $(Join-Path $WWWRootLoc home.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc home.min.css) `
    $(Join-Path $WWWRootLoc home.css)
Write-Output "[home]: CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc home.min.js) `
    $(Join-Path $NodeModulesLoc ga-lite dist ga-lite.js)
Write-Output "[home]: JS completed"

npx sass $(Join-Path $WWWRootLoc css blog.scss) $(Join-Path $WWWRootLoc blog.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc blog.min.css) `
    $(Join-Path $NodeModulesLoc \@highlightjs cdn-assets styles atom-one-light.min.css) `
    $(Join-Path $WWWRootLoc blog.css)
Write-Output "[blog]: CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc blog.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $NodeModulesLoc anchor-js anchor.js) `
    $(Join-Path $NodeModulesLoc ga-lite dist ga-lite.js) `
    $(Join-Path $NodeModulesLoc \@highlightjs cdn-assets highlight.js) `
    $(Join-Path $WWWRootLoc js shared.js) `
    $(Join-Path $WWWRootLoc js blog.js)
Write-Output "[blog]: JS completed"

npx sass $(Join-Path $WWWRootLoc css read.scss) $(Join-Path $WWWRootLoc read.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc read.min.css) `
    $(Join-Path $WWWRootLoc read.css)
Write-Output "[read]: CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc read.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $NodeModulesLoc anchor-js anchor.js) `
    $(Join-Path $NodeModulesLoc ga-lite dist ga-lite.js) `
    $(Join-Path $WWWRootLoc js shared.js) `
    $(Join-Path $WWWRootLoc js read.js)
Write-Output "[read]: JS completed"

npx sass $(Join-Path $WWWRootLoc css admin.scss) $(Join-Path $WWWRootLoc admin.css)
uglifycss --ugly-comments `
    --output $(Join-Path $WWWRootLoc admin.min.css) `
    $(Join-Path $NodeModulesLoc \@stackoverflow stacks dist css stacks.css) `
    $(Join-Path $NodeModulesLoc \@stackoverflow stacks-editor dist styles.css) `
    $(Join-Path $WWWRootLoc admin.css)
Write-Output "[admin]: CSS completed"

uglifyjs --compress -o $(Join-Path $WWWRootLoc admin.min.js) `
    $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.bundle.js) `
    $(Join-Path $NodeModulesLoc \@highlightjs cdn-assets highlight.js) `
    $(Join-Path $NodeModulesLoc \@stackoverflow stacks dist js stacks.js) `
    $(Join-Path $NodeModulesLoc \@stackoverflow stacks-editor dist app.bundle.js) `
    $(Join-Path $WWWRootLoc js shared.js) `
    $(Join-Path $WWWRootLoc js admin.js)
Write-Output "[admin]: JS completed"