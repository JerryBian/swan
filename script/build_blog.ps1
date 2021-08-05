$WWWRootLoc = Join-Path $PSScriptRoot .. src blog wwwroot
$NodeModulesLoc = Join-Path $PSScriptRoot .. node_modules

npx sass $(Join-Path $WWWRootLoc scss style.scss) $(Join-Path $WWWRootLoc style.css)
#uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc style.min.css) $(Join-Path $WWWRootLoc style.css)

uglifyjs --compress -o $(Join-Path $WWWRootLoc script.min.js) $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js) $(Join-Path $NodeModulesLoc anchor-js anchor.js)

# Index page
# npx sass $(Join-Path $WWWRootLoc scss index.scss) $(Join-Path $WWWRootLoc index.css)
# uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc index.min.css) $(Join-Path $WWWRootLoc index.css)
# uglifyjs --compress -o $(Join-Path $WWWRootLoc index.min.js) $(Join-Path $NodeModulesLoc bootstrap dist js bootstrap.js) 

# # Archive page
# npx sass $(Join-Path $WWWRootLoc scss archive.scss) $(Join-Path $WWWRootLoc archive.css)
# uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc archive.min.css) $(Join-Path $WWWRootLoc archive.css)
# uglifyjs --compress -o $(Join-Path $WWWRootLoc archive.min.js) $(Join-Path $NodeModulesLoc anchor-js anchor.js)

# # Post page
# npx sass $(Join-Path $WWWRootLoc scss post.scss) $(Join-Path $WWWRootLoc post.css)
# uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc post.min.css) $(Join-Path $WWWRootLoc post.css)
# uglifyjs --compress -o $(Join-Path $WWWRootLoc post.min.js) $(Join-Path $NodeModulesLoc anchor-js anchor.js)

# # Error page
# npx sass $(Join-Path $WWWRootLoc scss error.scss) $(Join-Path $WWWRootLoc error.css)
# uglifycss --expand-vars --ugly-comments --output $(Join-Path $WWWRootLoc error.min.css) $(Join-Path $WWWRootLoc error.css)