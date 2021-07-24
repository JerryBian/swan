$WWWRootLoc = Join-Path $PSScriptRoot .. src blog wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules

# Index page
npx sass $(Join-Path $WWWRootLoc scss index.scss) $(Join-Path $WWWRootLoc index.css)
uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc index.min.css) $(Join-Path $WWWRootLoc index.css)

# Archive page
npx sass $(Join-Path $WWWRootLoc scss archive.scss) $(Join-Path $WWWRootLoc archive.css)
uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc archive.min.css) $(Join-Path $WWWRootLoc archive.css)
uglifyjs --compress -o $(Join-Path $WWWRootLoc archive.min.js) $(Join-Path $NodeModulesLoc anchor-js anchor.js)

# Post page
npx sass $(Join-Path $WWWRootLoc scss post.scss) $(Join-Path $WWWRootLoc post.css)
uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc post.min.css) $(Join-Path $WWWRootLoc post.css)
uglifyjs --compress -o $(Join-Path $WWWRootLoc post.min.js) $(Join-Path $NodeModulesLoc anchor-js anchor.js)