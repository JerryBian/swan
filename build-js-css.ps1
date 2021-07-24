
$AdminRoot = "src/admin/wwwroot"

function Build-SASS-Admin {
    sass "$AdminRoot/scss/app.scss" "$AdminRoot/index.css"
}

function Build-Minify-CSS-Admin {

}

function Build-Minify-JS-Admin {

}

Build-SASS-Admin