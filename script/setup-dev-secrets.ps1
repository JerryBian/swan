
$BLOG_LOCAL_ENDPOINT = "http://localhost:5013"
$BLOG_REMOTE_ENDPOINT = "http://localhost:5013"
$API_LOCAL_ENDPOINT = "http://localhost:5011"
$FILE_REMOTE_ENDPOINT = "http://localhost:5013/file"
$ADMIN_USER_NAME = "test"
$ADMIN_EMAIL = "JerryBian@outlook.com"
$ASSET_LOCATION = "../../sample"
$ADMIN_CHINESE_NAME = "周杰伦"
$ADMIN_ENGLISH_NAME = "Jerry Bian"
$DATA_PROTECTION_KEY_PATH = "../../sample/data_protection"

##################
##              ##
##################
$BLOG_PROJECT = Join-Path $PSScriptRoot .. src blog
dotnet user-secrets init --project $BLOG_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $BLOG_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $BLOG_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $BLOG_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $BLOG_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $BLOG_PROJECT

$POSTS_PER_PAGE = "8"
dotnet user-secrets set "POSTS_PER_PAGE" "$POSTS_PER_PAGE" --project $BLOG_PROJECT

##################
##              ##
##################
$API_PROJECT = Join-Path $PSScriptRoot .. src api
dotnet user-secrets init --project $API_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $API_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $API_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $API_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $API_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $API_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $API_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $API_PROJECT

$SOURCE = "Local"
dotnet user-secrets set "SOURCE" "$SOURCE" --project $API_PROJECT

##################
##              ##
##################
$ADMIN_PROJECT = Join-Path $PSScriptRoot .. src admin
dotnet user-secrets init --project $ADMIN_PROJECT
dotnet user-secrets set "BLOG_LOCAL_ENDPOINT" "$BLOG_LOCAL_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "BLOG_REMOTE_ENDPOINT" "$BLOG_REMOTE_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "API_LOCAL_ENDPOINT" "$API_LOCAL_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_USER_NAME" "$ADMIN_USER_NAME" --project $ADMIN_PROJECT
dotnet user-secrets set "FILE_REMOTE_ENDPOINT" "$FILE_REMOTE_ENDPOINT" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_EMAIL" "$ADMIN_EMAIL" --project $ADMIN_PROJECT
dotnet user-secrets set "ASSET_LOCATION" "$ASSET_LOCATION" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_CHINESE_NAME" "$ADMIN_CHINESE_NAME" --project $ADMIN_PROJECT
dotnet user-secrets set "ADMIN_ENGLISH_NAME" "$ADMIN_ENGLISH_NAME" --project $ADMIN_PROJECT
dotnet user-secrets set "DATA_PROTECTION_KEY_PATH" "$DATA_PROTECTION_KEY_PATH" --project $ADMIN_PROJECT

$ADMIN_PASSWORD = "test"
dotnet user-secrets set "ADMIN_PASSWORD" "$ADMIN_PASSWORD" --project $ADMIN_PROJECT