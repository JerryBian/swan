$WWWRootLoc = Join-Path $PSScriptRoot .. src admin wwwroot

# Index page
npx sass $(Join-Path $WWWRootLoc scss app.scss) $(Join-Path $WWWRootLoc index.css)

# Error page
npx sass $(Join-Path $WWWRootLoc scss error.scss) $(Join-Path $WWWRootLoc error.css)